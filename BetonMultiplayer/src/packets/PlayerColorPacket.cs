using UnityEngine;

namespace BetonMultiplayer
{
    public class PlayerColorPacket : Packet
    {
        public Player player;
        public Color color;

        public PlayerColorPacket() : base(PacketType.PlayerColorPacket) { }

        public PlayerColorPacket(byte[] bytes) : base(PacketType.PlayerColorPacket, bytes) { }

        public PlayerColorPacket(Player player, Color color) : base(PacketType.PlayerColorPacket)
        {
            this.player = player;
            this.color = color;
            Encode();
        }

        public override void Encode()
        {
            string encodeString = player.name + "|";
            encodeString += SerializeColor(color);
            this.data = NetworkUtil.EncodeStringToBytes(encodeString);
        }

        public override void Decode()
        {
            string decodeString = NetworkUtil.DecodeBytesToString(this.data);
            string[] parts = decodeString.Split('|');

            foreach (Player player in BetonMultiplayerMod.players)
            {
                if (player.name.Equals(parts[0]))
                {
                    this.player = player;
                    break;
                }
            }

            this.color = DeserializeColor(parts[1]);
        }

        private string SerializeColor(Color color)
        {
            return color.r + "," + color.g + "," + color.b;
        }

        private Color DeserializeColor(string color)
        {
            string[] parts = color.Split(',');
            float r = float.Parse(parts[0]);
            float g = float.Parse(parts[1]);
            float b = float.Parse(parts[2]);
            return new Color(r, g, b);
        }
    }
}
