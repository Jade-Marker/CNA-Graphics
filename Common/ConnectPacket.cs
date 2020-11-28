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

        public ConnectPacket(IPEndPoint endPoint)
        {
            this.endPoint = endPoint.ToString();
            packetType = PacketType.CLIENT_CONNECT;
        }
    }
}
