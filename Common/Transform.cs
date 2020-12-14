using Microsoft.Xna.Framework;
using System;

namespace Common
{
    [Serializable]
    public class Transform
    {
        public Vector3 position { get; private set; }
        public Vector3 rotation { get; private set; }
        public Vector3 scale { get; private set; }
        public Vector3 velocity { get; private set; }

        public Transform(Vector3 position, Vector3 rotation, Vector3 scale, Vector3 velocity)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.velocity = velocity;
        }

        public Matrix CreateWorldMatrix()
        {
            Matrix world =
                Matrix.CreateScale(scale) *
                Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) *
                Matrix.CreateTranslation(position);

            return world;
        }

        public void Move(float deltaTime)
        {
            position += velocity * deltaTime;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public void SetRotation(Vector3 rotation)
        {
            this.rotation = rotation;
        }

        public void SetScale(Vector3 scale)
        {
            this.scale = scale;
        }

        public void SetVelocity(Vector3 velocity)
        {
            this.velocity = velocity;
        }
    }
}
