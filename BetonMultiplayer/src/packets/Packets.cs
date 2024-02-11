using System.Collections.Generic;

namespace BetonMultiplayer
{
    public enum PacketType
    {
        PlayerSpawnPacket = 0,
        PlayerMovePacket = 1,
        PlayerColorPacket = 2
    }

    public class Packets
    {
        public static Packet CreatePacket(PacketType type, byte[] bytes)
        {
            switch (type)
            {
                case PacketType.PlayerSpawnPacket: return new PlayerSpawnPacket(bytes);
                case PacketType.PlayerMovePacket: return new PlayerMovePacket(bytes);
                case PacketType.PlayerColorPacket: return new PlayerColorPacket(bytes);
                default: return null;
            }
        }
    }
}