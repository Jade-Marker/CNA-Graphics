using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    class Texture : Component
    {
        Texture2D _texture;
        public Texture2D texture { get { return _texture; } private set { _texture = value; } }

        public Texture(Texture2D texture)
        {
            this.texture = texture;
        }
    }
}
