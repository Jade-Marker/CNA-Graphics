using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace CNA_Graphics
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private DepthStencilState _depthStencilLessThan = new DepthStencilState() { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less };

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
            Scene movementScene = new Scene();
            Scene inputScene = new Scene();
            Color textColor = Color.Red;
            Texture2D fishTexture = Content.Load<Texture2D>("fishTexture");
            Model fishModel = Content.Load<Model>("fish");
            Model plane = Content.Load<Model>("plane");
            Texture2D seabedTexture = Content.Load<Texture2D>("seabed");
            Texture2D sandTexture = Content.Load<Texture2D>("Sand");


            Camera cameraComponent = new Camera(Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, _graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight, 0.1f, 100.0f));
            CameraController player = new CameraController(_graphics, this);
            movementScene.AddEntity(new Entity(new Transform(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(1, 1, 1), new Vector3(0, 0, 0)),
                new List<Component>() {
                    cameraComponent, player
                }));

            movementScene.AddEntity(new Entity(
                new Transform(new Vector3(0, -1, 0), new Vector3(MathHelper.ToRadians(-90), MathHelper.ToRadians(90), 0), new Vector3(10, 10, 10), new Vector3(0, 0, 0)),
                new List<Component>() {
                    new Mesh(plane),
                    new Texture(sandTexture),
                    new Renderer()
                }));

            movementScene.AddEntity(new Entity(
                new Transform(new Vector3(0, 3, 10), new Vector3(0, MathHelper.ToRadians(180), MathHelper.ToRadians(90)), new Vector3(5, 10, 10), new Vector3(0, 0, 0)),
                new List<Component>() {
                    new Mesh(plane),
                    new Texture(seabedTexture),
                    new Renderer()
                }));

            movementScene.AddEntity(new Entity(
                new Transform(new Vector3(0, 3, -10), new Vector3(0, 0, MathHelper.ToRadians(90)), new Vector3(5, 10, 10), new Vector3(0, 0, 0)),
                new List<Component>() {
                    new Mesh(plane),
                    new Texture(seabedTexture),
                    new Renderer()
                }));

            movementScene.AddEntity(new Entity(
                new Transform(new Vector3(10, 3, 0), new Vector3(0, MathHelper.ToRadians(-90), MathHelper.ToRadians(90)), new Vector3(5, 10, 10), new Vector3(0, 0, 0)),
                new List<Component>() {
                    new Mesh(plane),
                    new Texture(seabedTexture),
                    new Renderer()
                }));

            movementScene.AddEntity(new Entity(
                new Transform(new Vector3(-10, 3, 0), new Vector3(0, MathHelper.ToRadians(90), MathHelper.ToRadians(90)), new Vector3(5, 10, 10), new Vector3(0, 0, 0)),
                new List<Component>() {
                    new Mesh(plane),
                    new Texture(seabedTexture),
                    new Renderer()
                }));

            movementScene.AddEntity(new Entity(
                new Transform(new Vector3(0, 6, 0), new Vector3(MathHelper.ToRadians(90), 0, 0), new Vector3(10, 10, 10), new Vector3(0, 0, 0)),
                new List<Component>() {
                    new Mesh(plane),
                    new Texture(Content.Load<Texture2D>("water")),
                    new Renderer()
                }));

            CameraManager.MainCamera = cameraComponent;

            ClientManager clientManager = new ClientManager(player, movementScene.GetEntities(), fishModel, fishTexture, Content.Load<SpriteFont>("PlayerName"), GraphicsDevice, textColor);
            movementScene.AddEntity(new Entity(
                new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)),
                new List<Component>() {
                    clientManager
                }));
            clientManager.SetName("Player");


            inputScene.AddEntity(new Entity(
                new Transform(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)),
                new List<Component>() {
                    new NameGetter(GraphicsDevice, Content.Load<SpriteFont>("PlayerName"), "MovementScene", clientManager, sandTexture, textColor)
                }));

            SceneManager.RegisterScene("InputScene", inputScene);
            SceneManager.RegisterScene("MovementScene", movementScene);

            SceneManager.ChangeScene("InputScene");
        }

        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) && IsActive)
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            SceneManager.currentScene.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.DarkBlue, 1.0f, 0);

            Matrix view = CameraManager.MainCamera.GetViewMatrix();
            Matrix projection = CameraManager.MainCamera.GetProjectionMatrix();

            SceneManager.currentScene.Draw(view, projection, _depthStencilLessThan, BlendState.Opaque, RasterizerState.CullCounterClockwise, GraphicsDevice);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            SceneManager.currentScene.End();

            base.OnExiting(sender, args);
        }
    }
}
