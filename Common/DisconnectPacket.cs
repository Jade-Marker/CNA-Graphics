using System;

namespace Common
{
    [Serializable]
    public class DisconnectPacket:Packet
    {
        public Guid guid;

        public DisconnectPacket(Guid guid)
        {
            this.guid = guid;
            packetType = PacketType.CLIENT_DISCONNECT;
        }
    }
}
