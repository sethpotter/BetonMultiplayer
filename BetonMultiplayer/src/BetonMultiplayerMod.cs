using Steamworks;
using Steamworks.Data;
using System;
using UnityEngine;
using MelonLoader;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace BetonMultiplayer
{
    public class BetonMultiplayerMod : MelonMod
    {
        public static BetonNetwork Network;
        //public static BetonNetwork TestNetwork;
        private bool drawMultiplayerMenu = false;
        private string ipAddress = "127.0.0.1";
        private string port = "8211";
        private string color = "1.0,1.0,1.0";

        public static List<Player> players = new List<Player>();

        /**
         * Managers: GameObject - hierarchy
         * - InputManager
         * - SaveManager
         * - SettingsManager
         * - MusicManager
         * - SteamManager
         * - ReplayManager
         * - GameModeManager
         * - GhostManager
         * - DLCLoader
         */
        public static GameModeManager gameModeManager;
        public static GhostManager ghostManager;

        public override void OnInitializeMelon()
        {
            Network = new BetonNetwork();
            //TestNetwork = new BetonNetwork();
            ServerEvents.AddPacketListeners();
            ClientEvents.AddPacketListeners();
            MelonEvents.OnGUI.Subscribe(DrawMenu);
        }

        private (NetAddress ip, ushort port) verifyAddress()
        {
            ushort validPort = 8211;
            NetAddress ip = NetAddress.From("127.0.0.1", validPort);

            try
            {
                validPort = ushort.Parse(port);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                port = "Invalid";
            }
            
            try
            {
                ip = NetAddress.From(ipAddress, validPort);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ipAddress = "Invalid";
            }

            return (ip, validPort);
        }

        private void DrawMenu()
        {
            if(!drawMultiplayerMenu) return;

            GUI.Box(new Rect(0, 25, 150, 125), "");

            // If Client Connected
            if (Network.Client != null && Network.Client.Connected)
            {
                if (GUI.Button(new Rect(5, 35, 140, 25), "Disconnect"))
                {
                    Network.Close();
                }
            }
            // Not Connected
            else
            {
                if(Network.Client != null && Network.Client.Connecting)
                {
                    GUI.enabled = false;
                }
                else
                {
                    GUI.enabled = true;
                }

                // If Not Hosting
                if (!Network.host)
                {
                    ipAddress = GUI.TextField(new Rect(5, 35, 90, 25), ipAddress);
                    port = GUI.TextField(new Rect(95, 35, 50, 25), port);

                  

                    if (GUI.Button(new Rect(5, 62, 140, 25), "Host"))
                    {
                        var tuple = verifyAddress();
                        if (ipAddress.Equals("Invalid") || port.Equals("Invalid"))
                            return;
                        Network.Host(tuple.port);
                        //TestNetwork.Connect(tuple.ip);
                    }

                    if (GUI.Button(new Rect(5, 89, 140, 25), "Connect"))
                    {
                        var tuple = verifyAddress();
                        if (ipAddress.Equals("Invalid") || port.Equals("Invalid"))
                            return;
                        Network.Connect(tuple.ip);
                    }
                }
                else
                // If Hosting
                {
                    if (GUI.Button(new Rect(5, 35, 140, 25), "Stop"))
                    {
                        Network.Close();
                    }
                }
            }

            color = GUI.TextField(new Rect(5, 116, 140, 25), color);
            UnityEngine.Color? col = MiscUtil.floatStringToColor(color);
            if (col.HasValue)
            {
                gameModeManager.player.GetComponentInChildren<Light>().color = col.Value;
                PlayerColorPacket playerColorPacket = new PlayerColorPacket(new Player(SteamClient.Name), col.Value);
                
                if(Network.host)
                {
                    Network.ServerBroadcastPacket(playerColorPacket, 99999999);
                }
                else
                {
                    Network.SendPacketToSocketServer(playerColorPacket);
                }
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // This is called multiple times. Maybe look into a better solution
            gameModeManager = GameObject.Find("Managers").GetComponent<GameModeManager>();
            ghostManager = GameObject.Find("Managers").GetComponent<GhostManager>();
        }

        public override void OnUpdate()
        {
            Keyboard kb = Keyboard.current;

            if(kb.mKey.wasPressedThisFrame)
            {
                drawMultiplayerMenu = !drawMultiplayerMenu;
            }

            /*if (kb.tKey.wasPressedThisFrame)
            {
                PlayerMovePacket playerMovePacket = new PlayerMovePacket(new Player(SteamClient.Name), gameModeManager.player.transform);
                TestNetwork.SendPacketToSocketServer(playerMovePacket);
            }*/

            Network.Update();
            //TestNetwork.Update();
        }

        public override void OnFixedUpdate()
        {
            PlayerMovePacket playerMovePacket = new PlayerMovePacket(new Player(SteamClient.Name), gameModeManager.player.transform);

            // SERVER-SIDE
            if (Network.host)
            {
                // TODO Sender does not matter here.
                // We need to tell the clients that the host is moving.
                uint serverIdentity = 9999999;
                Network.ServerBroadcastPacket(playerMovePacket, serverIdentity);
            }
            // CLIENT-SIDE
            else
            {
                // If connected.
                if (Network.Client != null)
                {
                    Network.SendPacketToSocketServer(playerMovePacket);
                }
            }
        }
    }
}
