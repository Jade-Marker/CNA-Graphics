using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    class CameraController : Component
    {
        const float cMoveSpeed = 0.125f;
        const float cRotationSpeed = 0.1f;
        readonly float cMaxRotation = MathHelper.ToRadians(40);
        readonly float cMinRotation = MathHelper.ToRadians(-40);

        GraphicsDeviceManager _graphics;
        Game _game;

        public CameraController(GraphicsDeviceManager GraphicsDeviceManager, Game game)
        {
            _graphics = GraphicsDeviceManager;
            _game = game;
        }

        public override void Start()
        {
            _game.IsMouseVisible = false;
            Mouse.SetPosition(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        }

        public override void Update(float deltaTime)
        {
            if (_game.IsActive)
            {
                HandleMovement(deltaTime);
                HandleRotation(deltaTime);
            }
        }

        private void HandleRotation(float deltaTime)
        {
            Vector3 rotation = new Vector3(0, 0, 0);
            Vector2 deltaMouse = new Vector2(Mouse.GetState().X - _graphics.PreferredBackBufferWidth / 2, Mouse.GetState().Y - _graphics.PreferredBackBufferHeight / 2);

            rotation.Y -= deltaMouse.X * deltaTime * cRotationSpeed;
            rotation.X -= deltaMouse.Y * deltaTime * cRotationSpeed;
            parent.transform.rotation += rotation;

            if (parent.transform.rotation.X > cMaxRotation)
                parent.transform.rotation.X = cMaxRotation;

            if (parent.transform.rotation.X < cMinRotation)
                parent.transform.rotation.X = cMinRotation;

            Mouse.SetPosition(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        }

        private void HandleMovement(float deltaTime)
        {
            Vector3 movement = new Vector3(0, 0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                movement += Vector3.Transform(new Vector3(0, 0, -deltaTime), Matrix.CreateRotationY(parent.transform.rotation.Y));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                movement += Vector3.Transform(new Vector3(0, 0, deltaTime), Matrix.CreateRotationY(parent.transform.rotation.Y));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                movement += Vector3.Transform(new Vector3(-deltaTime, 0, 0), Matrix.CreateRotationY(parent.transform.rotation.Y));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                movement += Vector3.Transform(new Vector3(deltaTime, 0, 0), Matrix.CreateRotationY(parent.transform.rotation.Y));
            }

            if (movement != Vector3.Zero)
            {
                movement = Vector3.Normalize(movement) * cMoveSpeed;
                parent.transform.position += movement;
            }
        }
    }
}
