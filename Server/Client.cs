﻿using Common;
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
        private Socket _socket;
        public IPEndPoint endPoint;
        private NetworkStream _stream;
        private BinaryReader _reader;
        private BinaryWriter _writer;
        private BinaryFormatter _formatter;
        private object _readLock;
        private object _writeLock;

        public Client(Socket socket)
        {
            _readLock = new object();
            _writeLock = new object();

            _socket = socket;

            _stream = new NetworkStream(_socket);
            _formatter = new BinaryFormatter();

            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_stream);
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

        public void Send(Packet message)
        {
            lock (_writeLock)
            {
                Packet.TCPSendPacket(message, _formatter, _writer);
            }
        }
    }
}
