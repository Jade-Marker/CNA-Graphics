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
        private Entity player;
        private string name;
        private Color textColor;

        private BasicEffect basicEffect;
        private SpriteBatch spriteBatch;

        public ClientName(SpriteFont font, GraphicsDevice graphicsDevice, Entity player, string name, Color textColor)
        {
            this.font = font;
            this.player = player;
            this.name = name;
            this.textColor = textColor;

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
            spriteBatch.DrawString(font, name, Vector2.Zero, textColor, 0, font.MeasureString(name) /2, 0.025f, 0, 0);
            spriteBatch.End();
        }
    }
}
