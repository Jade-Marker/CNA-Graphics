using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CNA_Graphics
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;

        Texture2D fishTexture;
        Model fishModel;

        List<GameObject> gameObjects = new List<GameObject>();

        DepthStencilState depthStencilLessThan = new DepthStencilState() { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            fishTexture = Content.Load<Texture2D>("fishTexture");
            fishModel = Content.Load<Model>("fish");

            GameObject fish1 = new GameObject(fishModel, fishTexture, new Transform(new Vector3(-2, 0, 0), new Vector3(MathHelper.ToRadians(90), 0, 0), new Vector3(1, 1, 1)));
            GameObject fish2 = new GameObject(fishModel, fishTexture, new Transform(new Vector3(2, 0, -2), new Vector3(0, 0, 0), new Vector3(1, 1, 3)));
        
            gameObjects.Add(fish1);
            gameObjects.Add(fish2);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;


            for (int i = 0; i < gameObjects.Count; i++)
                gameObjects[i].Update(deltaTime);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.CornflowerBlue, 1.0f, 0);

            GraphicsDevice.DepthStencilState = depthStencilLessThan;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;


            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, (float)_graphics.PreferredBackBufferWidth / (float)_graphics.PreferredBackBufferHeight, 0.1f, 100.0f);

            Matrix view = Matrix.CreateTranslation(2, 0, -3);

            for (int i = 0; i < gameObjects.Count; i++)
                gameObjects[i].Draw(view, projection);

            base.Draw(gameTime);
        }
    }
}
