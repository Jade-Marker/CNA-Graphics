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

        public Model model { get { return _model; } private set { _model = value; } }


        public Mesh(Model model)
        {
            this.model = model;
        }
    }
}
