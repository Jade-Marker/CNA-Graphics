using Microsoft.Xna.Framework.Graphics;

namespace CNA_Graphics
{
    class Mesh : Component
    {
        public Model model { get; private set; }


        public Mesh(Model model)
        {
            this.model = model;
        }
    }
}
