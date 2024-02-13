using Steamworks.Data;
using Steamworks;
using System;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;

namespace BetonMultiplayer
{
    // SERVER-SIDE
    public class SteamSocketManager : SocketManager
    {
        public override void OnConnecting(Connection connection, ConnectionInfo data)
        {
            base.OnConnecting(connection, data);
            Debug.Log($"Server: Connecting... {data.Identity}");
        }

        public override void OnConnected(Connection connection, ConnectionInfo data)
        {
            Debug.Log($"Server: Joined {data.Identity}");
            ServerEvents.OnJoin(connection);

            // Tell the client they've connected.
            base.OnConnected(connection, data);
        }

        public override void OnDisconnected(Connection connection, ConnectionInfo data)
        {
            base.OnDisconnected(connection, data);
            Debug.Log($"Server: Disconnected {data.Identity}");
            //ServerEvents.OnDisconnect(connection);
        }

        public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            byte[] dataBytes = new byte[size];
            Marshal.Copy(data, dataBytes, 0, size);
            byte[] type = dataBytes.Take(4).ToArray();
            PacketType packetType = (PacketType) NetworkUtil.DecodeBytesToInt(type);
            Packet packet = Packets.CreatePacket(packetType, dataBytes.Skip(4).ToArray());

            Debug.Log($"Server: Got Message - {identity} - " + packetType);

            BetonMultiplayerMod.Network.ServerBus.ProcessPacket(connection.Id, packet);
            BetonMultiplayerMod.Network.ServerBroadcastPacket(packet, connection.Id);
        }
    }

    // CLIENT-SIDE
    public class SteamConnectionManager : ConnectionManager
    {
        public override void OnConnecting(ConnectionInfo info)
        {
            base.OnConnecting(info);
            Debug.Log("Client: On Connecting");
        }

        public override void OnConnected(ConnectionInfo info)
        {
            Debug.Log("Client: Connected");
            ClientEvents.OnJoin(info);
            base.OnConnected(info);
        }

        public override void OnDisconnected(ConnectionInfo info)
        {
            base.OnDisconnected(info);
            Debug.Log("Client: Disconnected");
            ClientEvents.OnDisconnect();
        }

        public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            byte[] dataBytes = new byte[size];
            Marshal.Copy(data, dataBytes, 0, size);
            byte[] type = dataBytes.Take(4).ToArray();
            PacketType packetType = (PacketType) NetworkUtil.DecodeBytesToInt(type);
            Packet packet = Packets.CreatePacket(packetType, dataBytes.Skip(4).ToArray());

            Debug.Log("Client: Got Message - " + packetType);

            uint serverIdentity = 0;
            BetonMultiplayerMod.Network.ClientBus.ProcessPacket(serverIdentity, packet);
        }
    }

    public class BetonNetwork
    {
        public SteamSocketManager Server;
        public SteamConnectionManager Client;
        public PacketBus ServerBus;
        public PacketBus ClientBus;

        public bool host;

        public BetonNetwork()
        {
            ServerBus = new PacketBus();
            ClientBus = new PacketBus();
        }

        // BOTH
        public void Update()
        {
            SteamClient.RunCallbacks();
            try
            {
                Server?.Receive();
                Client?.Receive();
            }
            catch(Exception e)
            {
                Debug.Log("Error receiving data on server/client");
                Debug.LogError(e);
            }
        }

        // BOTH
        public void Close()
        {
            try
            {
                Server?.Close();
                Client?.Close();
                host = false;
                Server = null;
                Client = null;
                Debug.Log("Server & Client: Closed.");
            }
            catch
            {
                Debug.Log("Error closing socket server / connection manager");
            }
        }

        // UTILITY
        public void Host(ushort port)
        {
            var address = NetAddress.AnyIp(port);
            Debug.Log("Host Start: " + address.ToString());
            Server = SteamNetworkingSockets.CreateNormalSocket<SteamSocketManager>(address);
            host = true;
        }

        // UTILITY - Not host
        public bool Connect(NetAddress address)
        {
            if (host)
                return false;

            Debug.Log("Trying to connect to: " + address.ToString());
            try
            {
                Client = SteamNetworkingSockets.ConnectNormal<SteamConnectionManager>(address);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        // SERVER-SIDE
        public void ServerBroadcastPacket(Packet packet, uint sender)
        {
            packet.Decode();
            Connection[] connections = Server.Connected.ToArray();
            foreach (Connection conn in connections)
            {
                if (conn.Id == sender)
                    continue;
                SendPacketToConnection(packet, conn);
            }
        }

        // BOTH?
        public Result SendPacketToConnection(Packet packet, Connection connection)
        {
            packet.Encode();
            byte[] bytes = packet.data;
            byte[] type = NetworkUtil.EncodeIntToBytes((int)packet.type);
            byte[] prepend = new byte[type.Length + bytes.Length];
            Array.Copy(type, 0, prepend, 0, type.Length);
            Array.Copy(bytes, 0, prepend, type.Length, bytes.Length);

            bytes = prepend;
            int sizeOfMessage = bytes.Length;
            IntPtr intPtrMessage = Marshal.AllocHGlobal(sizeOfMessage);
            Marshal.Copy(bytes, 0, intPtrMessage, sizeOfMessage);
            Result result = connection.SendMessage(intPtrMessage, sizeOfMessage, SendType.Reliable);
            Marshal.FreeHGlobal(intPtrMessage);
            return result;
        }

        // CLIENT-SIDE
        public bool SendPacketToSocketServer(Packet packet)
        {
            if (host)
                return false;

            try
            {
                return SendPacketToConnection(packet, Client.Connection) == Result.OK;
            }
            catch (Exception e)
            {
                Debug.Log("Unable to send message to socket server");
                Debug.LogError(e);
                return false;
            }
        }
    }
}
