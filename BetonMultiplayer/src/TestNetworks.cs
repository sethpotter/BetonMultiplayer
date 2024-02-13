using Steamworks;
using Steamworks.Data;

namespace BetonMultiplayer
{
    public class TestNetworks
    {
        public static BetonNetwork Test1;
        public static BetonNetwork Test2;
        public static BetonNetwork Test3;

        public static void Init()
        {
            Test1 = new BetonNetwork();
            Test2 = new BetonNetwork();
            Test3 = new BetonNetwork();
            AddPacketListeners();
        }

        public static void Update()
        {
            Test1.Update();
            Test2.Update();
            Test3.Update();
        }

        public static void Join()
        {
            Test1.Connect(NetAddress.From("127.0.0.1", 8211));
            Test2.Connect(NetAddress.From("127.0.0.1", 8211));
            Test3.Connect(NetAddress.From("127.0.0.1", 8211));
            //Spawn();
        }

        public static void AddPacketListeners()
        {
            Test1.ClientBus.Register(PacketType.PlayerSpawnPacket, ClientEvents.ProcessAddGhost);
            Test2.ClientBus.Register(PacketType.PlayerSpawnPacket, ClientEvents.ProcessAddGhost);
            Test3.ClientBus.Register(PacketType.PlayerSpawnPacket, ClientEvents.ProcessAddGhost);
        }

        public static void Spawn()
        {
            PlayerSpawnPacket one = new PlayerSpawnPacket("1");
            Test1.SendPacketToSocketServer(one);
            PlayerSpawnPacket two = new PlayerSpawnPacket("2");
            Test2.SendPacketToSocketServer(two);
            PlayerSpawnPacket three = new PlayerSpawnPacket("3");
            Test3.SendPacketToSocketServer(three);
        }
    }
}