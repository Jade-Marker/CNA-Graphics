using Microsoft.Xna.Framework;

namespace CNA_Graphics
{
    abstract public class Component
    {
        public Entity parent { get; protected set; }
        public void Initialise(Entity parent)
        {
            this.parent = parent;
        }

        public virtual void Start() { }

        public virtual void Update(float deltaTime) { }
        public virtual void Draw(Matrix view, Matrix projection) { }
        public virtual void End() { }
    }
}
