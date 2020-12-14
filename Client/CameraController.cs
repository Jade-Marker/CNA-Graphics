using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace CNA_Graphics
{
    class CameraController : Component
    {
        private const float cMoveSpeed = 5.0f;
        private const float cRotationSpeed = 0.1f;
        private const int cRotationThreshold = 5;
        private readonly float cMaxRotation = MathHelper.ToRadians(40);
        private readonly float cMinRotation = MathHelper.ToRadians(-40);

        private GraphicsDeviceManager _graphics;
        private Game _game;
        private Vector3 _oldMovement;

        public bool hasMoved { get; private set; }
        public bool hasRotated { get; private set; }

        public CameraController(GraphicsDeviceManager GraphicsDeviceManager, Game game)
        {
            _graphics = GraphicsDeviceManager;
            _game = game;
        }

        public override void Start()
        {
            _game.IsMouseVisible = false;
            Mouse.SetPosition(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            _oldMovement = Vector3.Zero;
        }

        public override void Update(float deltaTime)
        {
            MouseState state = Mouse.GetState();

            if (_game.IsActive && (state.X > 0) && (state.X < _graphics.PreferredBackBufferWidth) && (state.Y > 0) && (state.Y < _graphics.PreferredBackBufferHeight))
            {
                HandleMovement(deltaTime);
                HandleRotation(deltaTime);
            }
        }

        private void HandleRotation(float deltaTime)
        {
            Vector3 rotation = parent.transform.rotation;
            Vector2 deltaMouse = new Vector2(Mouse.GetState().X - _graphics.PreferredBackBufferWidth / 2, Mouse.GetState().Y - _graphics.PreferredBackBufferHeight / 2);

            rotation.Y -= deltaMouse.X * deltaTime * cRotationSpeed;
            rotation.X -= deltaMouse.Y * deltaTime * cRotationSpeed;

            if (rotation.X > cMaxRotation)
                rotation.X = cMaxRotation;

            if (rotation.X < cMinRotation)
                rotation.X = cMinRotation;

            parent.transform.SetRotation(rotation);

            Mouse.SetPosition(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

            hasRotated = (Math.Abs(deltaMouse.X) > cRotationThreshold || Math.Abs(deltaMouse.Y) > cRotationThreshold);
        }

        private void HandleMovement(float deltaTime)
        {
            Vector3 velocity = new Vector3(0, 0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                velocity += Vector3.Transform(new Vector3(0, 0, -1), Matrix.CreateRotationY(parent.transform.rotation.Y));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                velocity += Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(parent.transform.rotation.Y));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                velocity += Vector3.Transform(new Vector3(-1, 0, 0), Matrix.CreateRotationY(parent.transform.rotation.Y));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                velocity += Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationY(parent.transform.rotation.Y));
            }

            if (velocity != Vector3.Zero)
            {
                velocity = Vector3.Normalize(velocity) * cMoveSpeed;
                parent.transform.SetVelocity(velocity);
                parent.transform.Move(deltaTime);
            }
            else
            {
                parent.transform.SetVelocity(Vector3.Zero);
            }

            if (_oldMovement != velocity)
            {
                hasMoved = true;
                _oldMovement = velocity;
            }
            else
                hasMoved = false;
        }
    }
}
