using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    public class GameObject
    {
        Model _model;
        Texture2D _texture;
        Transform _transform;

        public GameObject(Model model, Texture2D texture, Transform transform)
        {
            _model = model;
            _texture = texture;
            _transform = transform;
        }

        public void Update(float deltaTime)
        {
            _transform.rotation.Y += MathHelper.ToRadians(30.0f) * deltaTime;
        }

        public void Draw(Matrix view, Matrix projection)
        {
            Matrix world = _transform.CreateWorldMatrix();

            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.Texture = _texture;
                    effect.TextureEnabled = true;
                }
                mesh.Draw();
            }
        }
    }
}
