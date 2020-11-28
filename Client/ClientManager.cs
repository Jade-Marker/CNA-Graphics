using Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace CNA_Graphics
{
    class ClientManager:Component
    {
        private UdpClient udpClient;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private BinaryWriter writer;
        private BinaryReader reader;
        private BinaryFormatter formatter;
        private CameraController player;

        public ClientManager(CameraController player)
        {
            this.player = player;
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse(ipAddress), port);

                udpClient = new UdpClient();
                udpClient.Connect(IPAddress.Parse(ipAddress), port);

                stream = tcpClient.GetStream();
                writer = new BinaryWriter(stream);
                reader = new BinaryReader(stream);
                formatter = new BinaryFormatter();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public override void Start()
        {
            if (Connect("127.0.0.1", 4444))
            {
                Packet.TCPSendPacket(new ConnectPacket((IPEndPoint)udpClient.Client.LocalEndPoint), formatter, writer);

                Thread udpThread = new Thread(() =>
                {
                    UDPProcessServerResponse();
                });
                udpThread.Start();
            }
        }

        private void UDPProcessServerResponse()
        {
            try
            {
                IPEndPoint endPoint;
                while (true)
                {
                    Packet packet = Packet.UDPReadPacket(udpClient, formatter, out endPoint);

                    switch (packet.packetType)
                    {
                        case PacketType.CLIENT_MOVE:
                            Transform transform = ((MovementPacket)packet).GetTransform();
                            parent.transform.position = transform.position;
                            parent.transform.rotation = new Vector3(transform.rotation.Z, transform.rotation.Y + MathHelper.ToRadians(90), transform.rotation.X);
                            parent.transform.scale = transform.scale;
                            break;
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Client UDP Read Method exception: " + e.Message);
            }
        }

        public override void Update(float deltaTime)
        {
            Packet.UDPSendPacket(udpClient, formatter, new MovementPacket(player.parent.transform));
        }
    }
}
