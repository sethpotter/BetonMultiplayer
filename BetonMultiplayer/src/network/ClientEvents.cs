using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace BetonMultiplayer
{
    public class ClientEvents
    {
        public static void ProcessMoveGhost(uint sender, Packet packet)
        {
            if (packet is PlayerMovePacket playerMovePacket)
            {
                playerMovePacket.player.Move(playerMovePacket.position, playerMovePacket.rotation);
            }
        }

        public static void ProcessAddGhost(uint sender, Packet packet)
        {
            if (packet is PlayerSpawnPacket playerSpawnPacket)
            {
                Player player = new Player(playerSpawnPacket.playerName);
                BetonMultiplayerMod.players.Add(player);
                player.Init();
                Debug.Log("Added Player! " + player.name);
            }
        }

        public static void AddPacketListeners()
        {
            BetonMultiplayerMod.Network.ClientBus.Register(PacketType.PlayerSpawnPacket, ProcessAddGhost);
            BetonMultiplayerMod.Network.ClientBus.Register(PacketType.PlayerMovePacket, ProcessMoveGhost);
        }

        public static void OnJoin(ConnectionInfo info)
        {
            Debug.Log("OnJoin-" + SteamClient.Name);
            // Tell the host that we're joining and to spawn our character for everyone.
            PlayerSpawnPacket spawnClientPlayer = new PlayerSpawnPacket(SteamClient.Name);
            //BetonMultiplayerMod.TestNetwork.SendPacketToSocketServer(spawnClientPlayer);
            BetonMultiplayerMod.Network.SendPacketToSocketServer(spawnClientPlayer);
        }

        public static void OnDisconnect() 
        { 
            foreach(Player player in BetonMultiplayerMod.players)
            {
                Object.Destroy(player.body);
            }
            BetonMultiplayerMod.players.Clear();
        }
    }
}