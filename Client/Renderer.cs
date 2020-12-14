using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CNA_Graphics
{
    class Renderer : Component
    {
        private Mesh _meshComponent;
        private Texture _textureComponent;

        public override void Start()
        {
            _meshComponent = (Mesh)parent.GetComponent<Mesh>();
            _textureComponent = (Texture)parent.GetComponent<Texture>();
        }

        public override void Draw(Matrix view, Matrix projection)
        {
            if (_meshComponent != null)
            {
                Matrix world = parent.transform.CreateWorldMatrix();

                foreach (ModelMesh mesh in _meshComponent.model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;
                        if (_textureComponent != null)
                        {
                            effect.Texture = _textureComponent.texture;
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
