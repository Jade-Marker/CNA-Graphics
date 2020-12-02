using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]
    public class MovementPacket:Packet
    {
        [Serializable]
        private struct Vector
        {
            float x, y, z;

            public Vector(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static Vector FromVec3(Vector3 vec3)
            {
                Vector vector = new Vector(vec3.X, vec3.Y, vec3.Z);
                return vector;
            }

            public static Vector3 ToVec3(Vector vec)
            {
                return new Vector3(vec.x, vec.y, vec.z);
            }
        }

        private Vector position;
        private Vector rotation;
        private Vector scale;

        public Transform GetTransform()
        {
            Transform transform = new Transform(Vector.ToVec3(position), Vector.ToVec3(rotation), Vector.ToVec3(scale));
            return transform;
        }

        public Guid guid;
        public MovementPacket(Transform transform, Guid guid)
        {
            position = Vector.FromVec3(transform.position);
            rotation = Vector.FromVec3(transform.rotation);
            scale = Vector.FromVec3(transform.scale);

            this.guid = guid;

            packetType = PacketType.CLIENT_MOVE;
        }
    }
}
