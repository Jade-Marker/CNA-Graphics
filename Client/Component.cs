using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    abstract public class Component
    {
        protected Entity parent;

        public virtual void Initialise(Entity parent)
        {
            this.parent = parent;
        }

        public virtual void Update(float deltaTime) { }
        public virtual void Draw(Matrix view, Matrix projection) { }
    }
}
