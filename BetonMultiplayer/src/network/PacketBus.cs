using System;
using System.Collections.Generic;

namespace BetonMultiplayer
{
    public class PacketBus
    {
        public delegate void Callback(uint sender, Packet packet);

        public Dictionary<PacketType, List<Callback>> events = new Dictionary<PacketType, List<Callback>>();

        public void ProcessPacket(uint sender, Packet packet)
        {
            List<Callback> callbacks;
            events.TryGetValue(packet.type, out callbacks);

            packet.Decode();

            foreach(Callback callback in callbacks)
            {
                callback(sender, packet);
            }
        }

        public void Register(PacketType packetType, Callback callback) 
        {
            List<Callback> packetCallbacks;
            events.TryGetValue(packetType, out packetCallbacks);

            if (packetCallbacks == null)
            {
                List<Callback> callbacks = new List<Callback>{callback};
                events.Add(packetType, callbacks);
            } 
            else
            {
                packetCallbacks.Add(callback);
            }
        }
    }
}