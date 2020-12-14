using Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Server
{
    class Server
    {
        private UdpClient _udpListener;
        private TcpListener _tcpListener;
        private ConcurrentSet<Client> _clients;

        public Server(string ipAddress, int port)
        {
            _tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _udpListener = new UdpClient(port);

            Thread thread = new Thread(() => { UDPListen(); });
            thread.Start();
        }

        public void Start()
        {
            _clients = new ConcurrentSet<Client>();
            _tcpListener.Start();

            while(true)
            {
                Socket socket = _tcpListener.AcceptSocket();

                Client client = new Client(socket, Guid.NewGuid());
                _clients.TryAdd(client);

                Thread thread = new Thread(() => { TCPClientMethod(client); });
                thread.Start();
            }
        }

        public void Stop()
        {
            _tcpListener.Stop();
            _udpListener.Close();
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
                    packet = Packet.UDPReadPacket(_udpListener, formatter, out endPoint);

                    Console.WriteLine("Recieved packet from " + endPoint.ToString() + DateTime.Now);

                    switch (packet.packetType)
                    {
                        case PacketType.CLIENT_MOVE:
                            MovementPacket movementPacket = packet as MovementPacket;

                            foreach (Client client in _clients)
                            {
                                //Clients aren't aware of their guid, so set find the client and set the guid to their guid
                                if ((client.endPoint != null) && client.endPoint.ToString() == endPoint.ToString())
                                { 
                                    movementPacket.guid = client.guid;
                                }
                            }

                            foreach (Client client in _clients)
                            {
                                //Send to everyone other than the original sender
                                if ((client.endPoint != null) && (endPoint.ToString() != client.endPoint.ToString()))
                                    Packet.UDPSendPacket(_udpListener, client.endPoint, formatter, movementPacket);
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
                            List<Guid> users = new List<Guid>();
                            List<string> userNames = new List<string>();
                            ConnectPacket connectPacket = receivedMessage as ConnectPacket;
                            connectPacket.guid = client.guid;

                            client.SetName(connectPacket.name);

                            client.SetEndPoint(IPEndPoint.Parse(connectPacket.endPoint));
                            connectPacket.endPoint = "";    //blank out the endpoint for security
                            
                            foreach (Client currClient in _clients)
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
            _clients.TryRemove(client);

            foreach (Client currClient in _clients)
            {
                currClient.TCPSend(new DisconnectPacket(client.guid));
            }
        }
    }
}
