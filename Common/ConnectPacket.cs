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
        public string name;
        public ConnectPacket(IPEndPoint endPoint, Guid guid, string name)
        {
            this.endPoint = endPoint.ToString();
            this.guid = guid;
            this.name = name;
            packetType = PacketType.CLIENT_CONNECT;
        }
    }
}
