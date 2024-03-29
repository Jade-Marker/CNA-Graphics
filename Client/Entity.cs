﻿using Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CNA_Graphics
{
    public class Entity
    {
        private List<Component> _components;
        public Transform transform { get; private set; }

        public Entity(Transform transform, List<Component> components)
        {
            this.transform = transform;
            _components = components;

            foreach (Component component in _components)
                component.Initialise(this);  
        }

        public void Start()
        {
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
