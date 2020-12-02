using System;
using System.Collections.Generic;
using System.Text;

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
