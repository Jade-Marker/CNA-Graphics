using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CNA_Graphics
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D goomba;
        Vector2 position;

        Texture2D fishTexture;
        Model model;

        float angle;

        DepthStencilState depthStencilLessThan = new DepthStencilState() { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            position = new Vector2(_graphics.PreferredBackBufferWidth/2, _graphics.PreferredBackBufferHeight/2);

            angle = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            goomba = Content.Load<Texture2D>("goomba");

            fishTexture = Content.Load<Texture2D>("fishTexture");
            model = Content.Load<Model>("fish");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.CornflowerBlue, 1.0f, 0);

            GraphicsDevice.DepthStencilState = depthStencilLessThan;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            angle += (float)gameTime.ElapsedGameTime.TotalSeconds * 30.0f;

            Matrix world = Matrix.CreateRotationY(MathHelper.ToRadians(angle));

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, (float)_graphics.PreferredBackBufferWidth / (float)_graphics.PreferredBackBufferHeight, 0.1f, 100.0f);

            Matrix view = Matrix.CreateTranslation(2, 0, -3);

            DrawModel(model, world, view, projection);
            DrawModel(model, world * Matrix.CreateTranslation(2, 0, -2), view, projection);

            _spriteBatch.Begin();

            _spriteBatch.Draw(goomba, position, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.Texture = fishTexture;
                    effect.TextureEnabled = true;
                }
                mesh.Draw();
            }
        }
    }
}
