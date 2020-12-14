using Microsoft.Xna.Framework;
using System;

namespace Common
{
    [Serializable]
    public class MovementPacket:Packet
    {
        [Serializable]
        private struct Vector
        {
            private float x, y, z;

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

        private Vector _position;
        private Vector _rotation;
        private Vector _scale;
        private Vector _velocity;

        public Transform GetTransform()
        {
            Transform transform = new Transform(Vector.ToVec3(_position), Vector.ToVec3(_rotation), Vector.ToVec3(_scale), Vector.ToVec3(_velocity));
            return transform;
        }

        public Guid guid;
        public MovementPacket(Transform transform, Guid guid)
        {
            //Monogame's Vector3 type doesn't serialise, so to serialise them we need to convert them to a custom Vector type that can serialise
            _position = Vector.FromVec3(transform.position);
            _rotation = Vector.FromVec3(transform.rotation);
            _scale = Vector.FromVec3(transform.scale);
            _velocity = Vector.FromVec3(transform.velocity);

            this.guid = guid;

            packetType = PacketType.CLIENT_MOVE;
        }
    }
}
