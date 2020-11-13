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
        Texture textureComponent;

        public override void Initialise(Entity parent)
        {
            meshComponent = (Mesh)parent.GetComponent<Mesh>();
            textureComponent = (Texture)parent.GetComponent<Texture>();

            base.Initialise(parent);
        }

        public override void Draw(Matrix view, Matrix projection)
        {
            if (meshComponent != null)
            {
                Matrix world = parent.transform.CreateWorldMatrix();

                foreach (ModelMesh mesh in meshComponent.model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;
                        if (textureComponent != null)
                        {
                            effect.Texture = textureComponent.texture;
                            effect.TextureEnabled = true;
                        }
                        else
                        {
                            effect.TextureEnabled = false;
                        }
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
