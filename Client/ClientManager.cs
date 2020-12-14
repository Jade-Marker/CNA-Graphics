using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace CNA_Graphics
{
    class ClientManager:Component
    {
        private UdpClient _udpClient;
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private BinaryWriter _writer;
        private BinaryReader _reader;
        private BinaryFormatter _formatter;

        private float _timer = 0.0f;
        private CameraController _player;
        private List<Entity> _entities;
        private Dictionary<Guid, Entity> _users;

        private Model _fishModel;
        private Texture2D _fishTexture;
        private GraphicsDevice _graphicsDevice;

        private SpriteFont _playerNameFont;
        private string _playerName;
        private Color _textColor;

        private const float cPacketSendTimer = 1.0f;
        private const float cMovementFaultTolerance = 1.0f;

        public ClientManager(CameraController player, List<Entity> entities, Model fishModel, Texture2D fishTexture, SpriteFont playerNameFont, GraphicsDevice graphicsDevice, Color textColor)
        {
            _player = player;
            _entities = entities;
            _fishModel = fishModel;
            _fishTexture = fishTexture;
            _playerNameFont = playerNameFont;
            _graphicsDevice = graphicsDevice;
            _textColor = textColor;

            _users = new Dictionary<Guid, Entity>();
        }

        public bool Connect(string ipAddress, int port)
        {
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(IPAddress.Parse(ipAddress), port);

                _udpClient = new UdpClient();
                _udpClient.Connect(IPAddress.Parse(ipAddress), port);

                _stream = _tcpClient.GetStream();
                _writer = new BinaryWriter(_stream);
                _reader = new BinaryReader(_stream);
                _formatter = new BinaryFormatter();
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
                Packet.TCPSendPacket(new ConnectPacket((IPEndPoint)_udpClient.Client.LocalEndPoint, Guid.Empty, _playerName), _formatter, _writer);
                Packet.UDPSendPacket(_udpClient, _formatter, new MovementPacket(_player.parent.transform, Guid.Empty));

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
            _tcpClient.Close();
            _udpClient.Close();
            _stream.Close();
            _writer.Close();
            _reader.Close();
        }

        private void UDPProcessServerResponse()
        {
            try
            {
                IPEndPoint endPoint;
                while (true)
                {
                    Packet packet = Packet.UDPReadPacket(_udpClient, _formatter, out endPoint);

                    switch (packet.packetType)
                    {
                        case PacketType.CLIENT_MOVE:
                            MovementPacket movement = packet as MovementPacket;

                            if(_users.ContainsKey(movement.guid) && _users[movement.guid] != null)
                            {
                                Transform transform = movement.GetTransform();

                                if((_users[movement.guid].transform.position - transform.position).LengthSquared() > cMovementFaultTolerance)
                                    _users[movement.guid].transform.SetPosition(transform.position);

                                //+90 in y as player model was oriented wrong without it
                                _users[movement.guid].transform.SetRotation(new Vector3(transform.rotation.Z, transform.rotation.Y + MathHelper.ToRadians(90), transform.rotation.X));
                                _users[movement.guid].transform.SetScale(transform.scale);
                                _users[movement.guid].transform.SetVelocity(transform.velocity);
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
                while ((serverResponse = Packet.TCPReadPacket(_reader, _formatter)) != null)
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
                            if(_users.ContainsKey(disconnectPacket.guid))
                            {
                                user = _users[disconnectPacket.guid];

                                //_entities gets looped through in the Scene class, so we don't want it to be altered while doing that
                                //So, lock _entities
                                lock (_entities)
                                {
                                    _entities.Remove(user);
                                }

                                _users.Remove(disconnectPacket.guid);
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
                new Mesh(_fishModel),
                new Texture(_fishTexture),
                new Renderer(),
                new ClientName(_playerNameFont, _graphicsDevice, _player.parent, name, _textColor)
            });
            user.Start();

            //_entities gets looped through in the Scene class, so we don't want it to be altered while doing that
            //So, lock _entities
            lock (_entities)
            {
                _entities.Add(user);
            }
            _users.Add(guid, user);
        }

        public override void Update(float deltaTime)
        {
            _timer += deltaTime;
            if (_player.hasMoved || _player.hasRotated || _timer >= cPacketSendTimer)
            {
                Packet.UDPSendPacket(_udpClient, _formatter, new MovementPacket(_player.parent.transform, Guid.Empty));
                _timer = 0;
            }

            foreach (Entity entity in _users.Values)
            {
                entity.transform.Move(deltaTime);
            }
        }

        public void SetName(string name)
        {
            _playerName = name;
        }
    }
}
