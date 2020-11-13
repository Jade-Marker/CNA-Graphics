using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    class Renderer : Component
    {
        Mesh meshComponent;

        public override void Initialise(Entity parent)
        {
            meshComponent = (Mesh)parent.GetComponent<Mesh>();

            base.Initialise(parent);
        }

        public override void Draw(Matrix view, Matrix projection)
        {
            Matrix world = parent.transform.CreateWorldMatrix();

            foreach (ModelMesh mesh in meshComponent.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.Texture = meshComponent.texture;
                    effect.TextureEnabled = true;
                }
                mesh.Draw();
            }
        }
    }
}
