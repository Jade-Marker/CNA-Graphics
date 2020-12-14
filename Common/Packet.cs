using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common
{
    public enum PacketType
    {
        EMPTY,
        CLIENT_CONNECT,
        CLIENT_DISCONNECT,
        CLIENT_MOVE,
        CLIENT_LIST
    };

    [Serializable]
    public class Packet
    {
        public PacketType packetType { get; protected set; }

        public static Packet TCPReadPacket(BinaryReader reader, BinaryFormatter formatter)
        {
            int numberOfBytes;
            if ((numberOfBytes = reader.ReadInt32()) != -1)
            {
                byte[] buffer = reader.ReadBytes(numberOfBytes);
                MemoryStream memoryStream = new MemoryStream(buffer);

                return formatter.Deserialize(memoryStream) as Packet;
            }
            else
                return null;
        }

        public static void TCPSendPacket(Packet message, BinaryFormatter formatter, BinaryWriter writer)
        {
            MemoryStream memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, message);

            byte[] buffer = memoryStream.GetBuffer();
            writer.Write(buffer.Length);
            writer.Write(buffer);
            writer.Flush();
        }

        public static Packet UDPReadPacket(UdpClient udpClient, BinaryFormatter formatter, out IPEndPoint endPoint)
        {
            endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = udpClient.Receive(ref endPoint);
            MemoryStream memoryStream = new MemoryStream(bytes);

            Packet packet = formatter.Deserialize(memoryStream) as Packet;

            return packet;
        }

        public static void UDPSendPacket(UdpClient udpClient, BinaryFormatter formatter, Packet packet)
        {
            MemoryStream memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, packet);

            byte[] buffer = memoryStream.GetBuffer();

            udpClient.Send(buffer, buffer.Length);
        }

        public static void UDPSendPacket(UdpClient udpClient, IPEndPoint endPoint, BinaryFormatter formatter, Packet packet)
        {
            MemoryStream memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, packet);

            byte[] buffer = memoryStream.GetBuffer();

            udpClient.Send(buffer, buffer.Length, endPoint);
        }
    }
}
