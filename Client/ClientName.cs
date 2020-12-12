using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    class ClientName:Component
    {
        private SpriteFont font;
        private BasicEffect basicEffect;
        SpriteBatch spriteBatch;
        private Entity player;
        string name;

        public ClientName(SpriteFont font, GraphicsDevice graphicsDevice, Entity player, string name)
        {
            this.font = font;
            this.name = name;
            this.player = player;

            spriteBatch = new SpriteBatch(graphicsDevice);

            basicEffect = new BasicEffect(graphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };
        }

        public override void Draw(Matrix view, Matrix projection)
        {
            //This code is based on the example found here: http://www.shawnhargreaves.com/blog/spritebatch-billboards-in-a-3d-world.html
            Vector3 position = parent.transform.position + new Vector3(0, 1, 0);
            basicEffect.World = Matrix.CreateScale(parent.transform.scale) * Matrix.CreateConstrainedBillboard(position, player.transform.position, Vector3.Down, null, null);
            basicEffect.View = view;
            basicEffect.Projection = projection;

            spriteBatch.Begin(0, null, null, DepthStencilState.DepthRead, RasterizerState.CullNone, basicEffect);
            spriteBatch.DrawString(font, name, Vector2.Zero, Color.Black, 0, font.MeasureString(name) /2, 0.025f, 0, 0);
            spriteBatch.End();
        }
    }
}
