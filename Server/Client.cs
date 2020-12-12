using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Server
{
    class Client
    {
        public IPEndPoint endPoint;
        public Guid guid;

        private Socket _socket;
        private NetworkStream _stream;
        private BinaryReader _reader;
        private BinaryWriter _writer;
        private BinaryFormatter _formatter;
        private object _readLock;
        private object _writeLock;

        public string name;

        public Client(Socket socket, Guid guid)
        {
            _readLock = new object();
            _writeLock = new object();

            _socket = socket;

            _stream = new NetworkStream(_socket);
            _formatter = new BinaryFormatter();

            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_stream);

            this.guid = guid;
        }

        public void Close()
        {
            _stream.Close();
            _reader.Close();
            _writer.Close();
            _socket.Close();
        }

        public Packet Read()
        {
            lock (_readLock)
            {
                return Packet.TCPReadPacket(_reader, _formatter);
            }
        }

        public void TCPSend(Packet message)
        {
            lock (_writeLock)
            {
                Packet.TCPSendPacket(message, _formatter, _writer);
            }
        }
    }
}
