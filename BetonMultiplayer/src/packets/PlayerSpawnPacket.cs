using UnityEngine;

namespace BetonMultiplayer
{
    public class PlayerSpawnPacket : Packet
    {
        public string playerName;

        public PlayerSpawnPacket() : base(PacketType.PlayerSpawnPacket) {}

        public PlayerSpawnPacket(byte[] bytes) : base(PacketType.PlayerSpawnPacket, bytes) { }

        public PlayerSpawnPacket(string playerName) : base(PacketType.PlayerSpawnPacket)
        {
            this.playerName = playerName;
        }

        public override void Encode()
        {
            this.data = NetworkUtil.EncodeStringToBytes(playerName);
        }

        public override void Decode()
        {
            string decodeString = NetworkUtil.DecodeBytesToString(this.data);
            this.playerName = decodeString;
        }
    }
}
