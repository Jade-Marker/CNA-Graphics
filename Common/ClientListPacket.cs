using System;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class ClientListPacket:Packet
    {
        public List<Guid> userList;
        public List<string> usernames;

        public ClientListPacket(List<Guid> userList, List<string> usernames)
        {
            this.userList = userList;
            this.usernames = usernames;
            packetType = PacketType.CLIENT_LIST;
        }
    }
}
