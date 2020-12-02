using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]
    public class ClientListPacket:Packet
    {
        public List<Guid> userList;

        public ClientListPacket(List<Guid> userList)
        {
            this.userList = userList;
            packetType = PacketType.CLIENT_LIST;
        }
    }
}
