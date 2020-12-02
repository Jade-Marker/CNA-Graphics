using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CNA_Graphics
{
    public class Entity
    {
        Transform _transform;
        List<Component> _components;
        public Transform transform { get { return _transform; } private set { _transform = value; } }

        public Entity(Transform transform, List<Component> components)
        {
            _transform = transform;
            _components = components;

            foreach (Component component in _components)
                component.Initialise(this);

            foreach (Component component in _components)
                component.Start();
        }

        public void Update(float deltaTime)
        {
            foreach (Component component in _components)
                component.Update(deltaTime);
        }

        public void Draw(Matrix view, Matrix projection)
        {
            foreach (Component component in _components)
                component.Draw(view, projection);
        }

        public void End()
        {
            foreach (Component component in _components)
                component.End();
        }

        public Component GetComponent<T>()
        {
            Type type = typeof(T);
            foreach (Component component in _components)
            {
                if (component.GetType() == type)
                    return component;
            }

            return null;
        }
    }
}
