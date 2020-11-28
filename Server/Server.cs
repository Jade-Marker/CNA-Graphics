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

                Client client = new Client(socket);
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
                Packet packet = Packet.UDPReadPacket(udpListener, formatter);

                switch (packet.packetType)
                {
                    case PacketType.CLIENT_MOVE:
                        MovementPacket movementPacket = packet as MovementPacket;
                        Transform transform = movementPacket.GetTransform();
                        Console.WriteLine("Transform position: " + "(" + transform.position.X + ", " + transform.position.Y + ", " + transform.position.Z + ")");
                        break;
                }
            }
        }

        private void TCPClientMethod(Client client)
        {
            Packet receivedMessage;
            while ((receivedMessage = client.Read()) != null)
            {
                switch (receivedMessage.packetType)
                {
                    case PacketType.CLIENT_CONNECT:
                        client.endPoint = IPEndPoint.Parse(((ConnectPacket)receivedMessage).endPoint);
                        break;

                    case PacketType.CLIENT_DISCONNECT:
                        break;
                }
            }

            client.Close();
            clients.TryRemove(client);
        }
    }
}
