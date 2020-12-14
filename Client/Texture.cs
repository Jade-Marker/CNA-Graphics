using Microsoft.Xna.Framework.Graphics;

namespace CNA_Graphics
{
    class Texture : Component
    {
        public Texture2D texture { get; private set; }

        public Texture(Texture2D texture)
        {
            this.texture = texture;
        }
    }
}
