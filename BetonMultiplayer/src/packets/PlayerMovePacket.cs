using UnityEngine;

namespace BetonMultiplayer
{
    public class PlayerMovePacket : Packet
    {
        public Player player;
        public Vector3 position;
        public Quaternion rotation;

        public PlayerMovePacket() : base(PacketType.PlayerMovePacket) { }

        public PlayerMovePacket(byte[] bytes) : base(PacketType.PlayerMovePacket, bytes) { }

        public PlayerMovePacket(Player player, Transform transform) : base(PacketType.PlayerMovePacket)
        {
            this.player = player;
            this.position = transform.position;
            this.rotation = transform.rotation;
            Encode();
        }

        public override void Encode()
        {
            string encodeString = player.name + "|";
            encodeString += SerializeVector3(position) + "|";
            encodeString += SerializeVector3(rotation.eulerAngles);
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

            this.position = DeserializeVector3(parts[1]);
            this.rotation = Quaternion.Euler(DeserializeVector3(parts[2]));
        }

        private string SerializeVector3(Vector3 vector)
        {
            return vector.x + "," + vector.y + "," + vector.z;
        }

        private Vector3 DeserializeVector3(string vector)
        {
            string[] parts = vector.Split(',');
            float x = float.Parse(parts[0]);
            float y = float.Parse(parts[1]);
            float z = float.Parse(parts[2]);
            return new Vector3(x, y, z);
        }
    }
}
