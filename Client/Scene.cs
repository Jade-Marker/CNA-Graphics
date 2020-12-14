using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CNA_Graphics
{
    public class Scene
    {
        private List<Entity> _entities;
        
        public Scene()
        {
            _entities = new List<Entity>();

        }

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        public List<Entity> GetEntities()
        {
            return _entities;
        }


        public void Start()
        {
            //_entities could be accessed by a different thread from ClientManager, so lock it while we're using it
            lock (_entities)
            {
                foreach (Entity entity in _entities)
                    entity.Start();
            }
        }

        public void Update(float deltaTime)
        {
            lock (_entities)
            {
                foreach (Entity entity in _entities)
                    entity.Update(deltaTime);
            }
        }

        public void Draw(Matrix view, Matrix projection, DepthStencilState depthStencilState, BlendState blendState, RasterizerState rasterizerState, GraphicsDevice graphicsDevice)
        {
            lock (_entities)
            {
                foreach (Entity entity in _entities)
                {
                    graphicsDevice.DepthStencilState = depthStencilState;
                    graphicsDevice.BlendState = blendState;
                    graphicsDevice.RasterizerState = rasterizerState;

                    entity.Draw(view, projection);
                }
            }
        }

        public void End()
        {
            lock (_entities)
            {
                foreach (Entity entity in _entities)
                    entity.End();
            }
        }
    }
}
