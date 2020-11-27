using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    public class Transform
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        public Matrix CreateWorldMatrix()
        {
            Matrix world = 
                Matrix.CreateScale(scale) * 
                Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * 
                Matrix.CreateTranslation(position);

            return world;
        }
    }
}
