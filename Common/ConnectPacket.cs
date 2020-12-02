using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Common
{
    [Serializable]
    public class ConnectPacket:Packet
    {
        public string endPoint;
        public Guid guid;
        public ConnectPacket(IPEndPoint endPoint, Guid guid)
        {
            this.endPoint = endPoint.ToString();
            this.guid = guid;
            packetType = PacketType.CLIENT_CONNECT;
        }
    }
}
