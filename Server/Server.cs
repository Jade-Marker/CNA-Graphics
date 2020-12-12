using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        private UdpClient udpListener;
        private TcpListener tcpListener;
        private ConcurrentSet<Client> clients;

        public Server(string ipAddress, int port)
        {
            tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            udpListener = new UdpClient(port);

            Thread thread = new Thread(() => { UDPListen(); });
            thread.Start();
        }

        public void Start()
        {
            clients = new ConcurrentSet<Client>();
            tcpListener.Start();

            while(true)
            {
                Socket socket = tcpListener.AcceptSocket();

                Client client = new Client(socket, Guid.NewGuid());
                clients.TryAdd(client);

                Thread thread = new Thread(() => { TCPClientMethod(client); });
                thread.Start();
            }
        }

        public void Stop()
        {
            tcpListener.Stop();
            udpListener.Close();
        }

        private void UDPListen()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            while (true)
            {
                IPEndPoint endPoint;
                Packet packet;

                try
                {
                    packet = Packet.UDPReadPacket(udpListener, formatter, out endPoint);

                    Console.WriteLine("Recieved packet from " + endPoint.ToString() + DateTime.Now);

                    switch (packet.packetType)
                    {
                        case PacketType.CLIENT_MOVE:
                            MovementPacket movementPacket = packet as MovementPacket;

                            foreach (Client client in clients)
                            {
                                if ((client.endPoint != null) && client.endPoint.ToString() == endPoint.ToString())
                                    movementPacket.guid = client.guid;
                            }

                            foreach (Client client in clients)
                            {
                                if ((client.endPoint != null) && (endPoint.ToString() != client.endPoint.ToString()))
                                    Packet.UDPSendPacket(udpListener, client.endPoint, formatter, movementPacket);
                            }
                            break;
                    }
                }
                catch (System.Net.Sockets.SocketException)
                {
                }

            }
        }

        private void TCPClientMethod(Client client)
        {
            Packet receivedMessage;

            try
            {
                while ((receivedMessage = client.Read()) != null)
                {
                    switch (receivedMessage.packetType)
                    {
                        case PacketType.CLIENT_CONNECT:
                            ConnectPacket connectPacket = receivedMessage as ConnectPacket;
                            connectPacket.guid = client.guid;

                            client.name = connectPacket.name;

                            client.endPoint = IPEndPoint.Parse(connectPacket.endPoint);
                            connectPacket.endPoint = "";
                            List<Guid> users = new List<Guid>();
                            List<string> userNames = new List<string>();
                            foreach (Client currClient in clients)
                            {
                                if (currClient != client)
                                {
                                    currClient.TCPSend(connectPacket);
                                    users.Add(currClient.guid);
                                    userNames.Add(currClient.name);
                                }
                            }

                            client.TCPSend(new ClientListPacket(users, userNames));

                            break;
                    }
                }
            }
            catch (System.IO.EndOfStreamException)
            {
            }
            

            client.Close();
            clients.TryRemove(client);

            foreach (Client currClient in clients)
            {
                currClient.TCPSend(new DisconnectPacket(client.guid));
            }
        }
    }
}
