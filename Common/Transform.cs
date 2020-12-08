using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]
    public class Transform
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Vector3 velocity;

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
    }
}
