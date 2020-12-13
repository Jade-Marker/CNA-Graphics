﻿using Common;
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

        private float timer = 0.0f;
        private CameraController player;
        private List<Entity> entities;
        private Dictionary<Guid, Entity> users;

        private Model fishModel;
        private Texture2D fishTexture;
        private GraphicsDevice graphicsDevice;

        private SpriteFont playerNameFont;
        private string playerName;
        private Color textColor;

        private const float cPacketSendTimer = 1.0f;
        private const float cMovementFaultTolerance = 1.0f;

        public ClientManager(CameraController player, List<Entity> entities, Model fishModel, Texture2D fishTexture, SpriteFont playerNameFont, GraphicsDevice graphicsDevice, Color textColor)
        {
            this.player = player;
            this.entities = entities;
            this.fishModel = fishModel;
            this.fishTexture = fishTexture;
            this.playerNameFont = playerNameFont;
            this.graphicsDevice = graphicsDevice;
            this.textColor = textColor;

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
                Packet.TCPSendPacket(new ConnectPacket((IPEndPoint)udpClient.Client.LocalEndPoint, Guid.Empty, playerName), formatter, writer);
                Packet.UDPSendPacket(udpClient, formatter, new MovementPacket(player.parent.transform, Guid.Empty));

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
        
        public override void End()
        {
            tcpClient.Close();
            udpClient.Close();
            stream.Close();
            writer.Close();
            reader.Close();
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

                                if((users[movement.guid].transform.position - transform.position).LengthSquared() > cMovementFaultTolerance)
                                    users[movement.guid].transform.position = transform.position;

                                users[movement.guid].transform.rotation = new Vector3(transform.rotation.Z, transform.rotation.Y + MathHelper.ToRadians(90), transform.rotation.X);
                                users[movement.guid].transform.scale = transform.scale;
                                users[movement.guid].transform.velocity = transform.velocity;
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
                            for(int i = 0; i <clientList.userList.Count; i++)
                                InstantiateNewClient(clientList.userList[i], clientList.usernames[i]);
                            break;

                        case PacketType.CLIENT_CONNECT:
                            ConnectPacket connectPacket = serverResponse as ConnectPacket;

                            InstantiateNewClient(connectPacket.guid, connectPacket.name);
                            break;

                        case PacketType.CLIENT_DISCONNECT:
                            DisconnectPacket disconnectPacket = serverResponse as DisconnectPacket;
                            if(users.ContainsKey(disconnectPacket.guid))
                            {
                                user = users[disconnectPacket.guid];

                                lock (entities)
                                {
                                    entities.Remove(user);
                                }

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

        private void InstantiateNewClient(Guid guid, string name)
        {
            Entity user = new Entity(new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)),
            new List<Component>() {
                new Mesh(fishModel),
                new Texture(fishTexture),
                new Renderer(),
                new ClientName(playerNameFont, graphicsDevice, player.parent, name, textColor)
            });
            user.Start();

            lock (entities)
            {
                entities.Add(user);
            }
            users.Add(guid, user);
        }

        public override void Update(float deltaTime)
        {
            timer += deltaTime;
            if (player.hasMoved || player.hasRotated || timer >= cPacketSendTimer)
            {
                Packet.UDPSendPacket(udpClient, formatter, new MovementPacket(player.parent.transform, Guid.Empty));
                timer = 0;
            }

            foreach (Entity entity in users.Values)
            {
                entity.transform.Move(deltaTime);
            }
        }

        public void SetName(string name)
        {
            playerName = name;
        }
    }
}
