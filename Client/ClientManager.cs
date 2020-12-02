using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private List<Entity> entities;
        private Model fishModel;
        private Texture2D fishTexture;
        private Dictionary<Guid, Entity> users;

        public override void End()
        {
            tcpClient.Close();
            udpClient.Close();
        }

        public ClientManager(CameraController player, List<Entity> entities, Model fishModel, Texture2D fishTexture)
        {
            this.player = player;
            this.entities = entities;
            this.fishModel = fishModel;
            this.fishTexture = fishTexture;

            users = new Dictionary<Guid, Entity>();
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
                Packet.TCPSendPacket(new ConnectPacket((IPEndPoint)udpClient.Client.LocalEndPoint, Guid.Empty), formatter, writer);

                Thread udpThread = new Thread(() =>
                {
                    UDPProcessServerResponse();
                });
                udpThread.Start();

                Thread tcpThread = new Thread(() =>
                {
                    TCPProcessServerResponse();
                });
                tcpThread.Start();
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
                            MovementPacket movement = packet as MovementPacket;

                            if(users.ContainsKey(movement.guid) && users[movement.guid] != null)
                            {
                                Transform transform = movement.GetTransform();
                                users[movement.guid].transform.position = transform.position;
                                users[movement.guid].transform.rotation = new Vector3(transform.rotation.Z, transform.rotation.Y + MathHelper.ToRadians(90), transform.rotation.X);
                                users[movement.guid].transform.scale = transform.scale;
                            }
                            break;
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Client UDP Read Method exception: " + e.Message);
            }
        }

        private void TCPProcessServerResponse()
        {
            Packet serverResponse;
            try
            {
                while ((serverResponse = Packet.TCPReadPacket(reader, formatter)) != null)
                {
                    Entity user;

                    switch (serverResponse.packetType)
                    {
                        case PacketType.CLIENT_LIST:
                            ClientListPacket clientList = serverResponse as ClientListPacket;
                            foreach (Guid client in clientList.userList)
                            {
                                user = new Entity(new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)),
                                    new List<Component>() {
                                    new Mesh(fishModel),
                                    new Texture(fishTexture),
                                    new Renderer()
                                });
                                entities.Add(user);
                                users.Add(client, user);
                            }

                            break;

                        case PacketType.CLIENT_CONNECT:
                            user = new Entity(new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)),
                            new List<Component>() {
                                new Mesh(fishModel),
                                new Texture(fishTexture),
                                new Renderer()
                            });
                            entities.Add(user);
                            users.Add((serverResponse as ConnectPacket).guid, user);
                            break;

                        case PacketType.CLIENT_DISCONNECT:
                            DisconnectPacket disconnectPacket = serverResponse as DisconnectPacket;
                            if(users.ContainsKey(disconnectPacket.guid))
                            {
                                user = users[disconnectPacket.guid];

                                entities.Remove(user);

                                users.Remove(disconnectPacket.guid);
                            }
                            break;
                    }
                }
            }
            catch (System.IO.IOException)
            {

            }
        }

        public override void Update(float deltaTime)
        {
            Packet.UDPSendPacket(udpClient, formatter, new MovementPacket(player.parent.transform, Guid.Empty));
        }
    }
}
