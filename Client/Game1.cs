﻿using Common;
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

        Texture2D fishTexture;
        Model fishModel;

        Scene movementScene;
        Scene inputScene;

        DepthStencilState depthStencilLessThan = new DepthStencilState() { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            movementScene = new Scene();
            inputScene = new Scene();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Color textColor = Color.Red;
            fishTexture = Content.Load<Texture2D>("fishTexture");
            fishModel = Content.Load<Model>("fish");

            List<Component> components1 = new List<Component>();
            components1.Add(new Mesh(fishModel));
            components1.Add(new Texture(fishTexture));
            components1.Add(new Renderer());
            components1.Add(new Rotator());
            Entity fish1 = new Entity(new Transform(new Vector3(-2, 0, 0), new Vector3(MathHelper.ToRadians(90), 0, 0), new Vector3(1, 1, 1), new Vector3(0,0,0)), components1);

            List<Component> components2 = new List<Component>();
            components2.Add(new Mesh(fishModel));
            components2.Add(new Texture(fishTexture));
            components2.Add(new Renderer());
            Entity fish2 = new Entity(new Transform(new Vector3(2, 0, -2), new Vector3(0, MathHelper.ToRadians(-90), 0), new Vector3(1, 1, 1), new Vector3(0, 0, 0)), components2);

            movementScene.AddEntity(fish1);
            movementScene.AddEntity(fish2);

            List<Component> components3 = new List<Component>();
            Camera cameraComponent = new Camera(Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, (float)_graphics.PreferredBackBufferWidth / (float)_graphics.PreferredBackBufferHeight, 0.1f, 100.0f));
            components3.Add(cameraComponent);
            CameraController player = new CameraController(_graphics, this);
            components3.Add(player);
            Entity camera = new Entity(new Transform(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(1, 1, 1), new Vector3(0, 0, 0)), components3);
            movementScene.AddEntity(camera);

            movementScene.AddEntity(new Entity(
                new Transform(new Vector3(0, -1, 0), new Vector3(MathHelper.ToRadians(-90), MathHelper.ToRadians(90), 0), new Vector3(10, 10, 10), new Vector3(0, 0, 0)),
                new List<Component>() {
                    new Mesh(Content.Load<Model>("plane")),
                    new Texture(Content.Load<Texture2D>("grass")),
                    new Renderer()
                })) ;

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
                    new NameGetter(GraphicsDevice, Content.Load<SpriteFont>("PlayerName"), "MovementScene", clientManager, Content.Load<Texture2D>("grass"), textColor)
                }));

            SceneManager.RegisterScene("InputScene", inputScene);
            SceneManager.RegisterScene("MovementScene", movementScene);

            SceneManager.ChangeScene("InputScene");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            SceneManager.currentScene.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.CornflowerBlue, 1.0f, 0);

            Matrix view = CameraManager.MainCamera.GetViewMatrix();
            Matrix projection = CameraManager.MainCamera.GetProjectionMatrix();

            SceneManager.currentScene.Draw(view, projection, depthStencilLessThan, BlendState.Opaque, RasterizerState.CullCounterClockwise, GraphicsDevice);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            SceneManager.currentScene.End();

            base.OnExiting(sender, args);
        }
    }
}
