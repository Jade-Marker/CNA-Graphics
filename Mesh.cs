using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    class Mesh : Component
    {
        Model _model;
        Texture2D _texture;

        public Model model { get { return _model; } private set { _model = value; } }
        public Texture2D texture { get { return _texture; } private set { _texture = value; } }


        public Mesh(Model model, Texture2D texture)
        {
            this.model = model;
            this.texture = texture;
        }
    }
}
