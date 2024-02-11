namespace BetonMultiplayer
{
    public class Packet : IReadablePacket
    {
        public PacketType type;
        public byte[] data;

        public Packet(PacketType type, byte[] data) 
        { 
            this.type = type;
            this.data = data;
        }

        public Packet(PacketType type) 
        {
            this.type = type;
        }

        public virtual void Encode()
        { 

        }

        public virtual void Decode() 
        { 

        }
    }
}
