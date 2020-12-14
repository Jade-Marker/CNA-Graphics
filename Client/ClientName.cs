using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CNA_Graphics
{
    class ClientName:Component
    {
        private SpriteFont _font;
        private Entity _player;
        private string _name;
        private Color _textColor;

        private BasicEffect _basicEffect;
        private SpriteBatch _spriteBatch;

        public ClientName(SpriteFont font, GraphicsDevice graphicsDevice, Entity player, string name, Color textColor)
        {
            _font = font;
            _player = player;
            _name = name;
            _textColor = textColor;

            _spriteBatch = new SpriteBatch(graphicsDevice);
            _basicEffect = new BasicEffect(graphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };
        }

        public override void Draw(Matrix view, Matrix projection)
        {
            //This code is based on the example found here: http://www.shawnhargreaves.com/blog/spritebatch-billboards-in-a-3d-world.html
            Vector3 position = parent.transform.position + new Vector3(0, 1, 0);
            _basicEffect.World = Matrix.CreateScale(parent.transform.scale) * Matrix.CreateConstrainedBillboard(position, _player.transform.position, Vector3.Down, null, null);
            _basicEffect.View = view;
            _basicEffect.Projection = projection;

            _spriteBatch.Begin(0, null, null, DepthStencilState.DepthRead, RasterizerState.CullNone, _basicEffect);
            _spriteBatch.DrawString(_font, _name, Vector2.Zero, _textColor, 0, _font.MeasureString(_name) /2, 0.025f, 0, 0);
            _spriteBatch.End();
        }
    }
}
