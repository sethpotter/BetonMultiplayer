using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace BetonMultiplayer
{
    public class ServerEvents
    {
        public static void ProcessMoveGhost(uint sender, Packet packet)
        {
            if (packet is PlayerMovePacket playerMovePacket)
            {
                playerMovePacket.player?.Move(playerMovePacket.position, playerMovePacket.rotation);
            }
        }

        public static void ProcessAddGhost(uint sender, Packet packet)
        {
            if (packet is PlayerSpawnPacket playerSpawnPacket)
            {
                Player player = new Player(playerSpawnPacket.playerName, sender);
                BetonMultiplayerMod.players.Add(player);
                player.Init();
                Debug.Log("SERVER EVENTS: Added Player! " + player.name);
            }
        }

        public static void ProcessChangeColor(uint sender, Packet packet)
        {
            if (packet is PlayerColorPacket playerColorPacket)
            {
                if (playerColorPacket.player != null)
                {
                    playerColorPacket.player.body.GetComponentInChildren<Light>().color = playerColorPacket.color;
                    playerColorPacket.player.body.GetComponent<MeshRenderer>().material.color = playerColorPacket.color;
                }
            }
        }

        public static void AddPacketListeners()
        {
            BetonMultiplayerMod.Network.ServerBus.Register(PacketType.PlayerSpawnPacket, ProcessAddGhost);
            BetonMultiplayerMod.Network.ServerBus.Register(PacketType.PlayerMovePacket, ProcessMoveGhost);
            BetonMultiplayerMod.Network.ServerBus.Register(PacketType.PlayerColorPacket, ProcessChangeColor);
        }

        public static void OnJoin(Connection connection)
        {
            Debug.Log("Server-OnJoin");

            // Tell the client that's joining all the players that are currently online
            foreach(Player player in BetonMultiplayerMod.players)
            {
                if (connection.Id == player.connectionId)
                    continue;
                PlayerSpawnPacket spawnClientPlayer = new PlayerSpawnPacket(player.name);
                BetonMultiplayerMod.Network.SendPacketToConnection(spawnClientPlayer, connection);
            }

            PlayerSpawnPacket spawnHostPlayer = new PlayerSpawnPacket(SteamClient.Name);
            BetonMultiplayerMod.Network.SendPacketToConnection(spawnHostPlayer, connection);
        }

        public static void OnDisconnect(Connection connection) 
        {
            Player toRemove = null;
            foreach (Player player in BetonMultiplayerMod.players)
            {
                if(player.connectionId == connection.Id)
                {
                    toRemove = player;
                    Object.Destroy(player.body);
                    break;
                }
            }
            if (toRemove != null)
            {
                BetonMultiplayerMod.players.Remove(toRemove);
            }
        }
    }
}