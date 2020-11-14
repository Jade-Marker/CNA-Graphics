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

        public override void Initialise(Entity parent)
        { 

            base.Initialise(parent);
        }

        public override void Update(float deltaTime)
        {
            Vector3 movement = new Vector3(0, 0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                movement.Z -= deltaTime;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                movement.Z += deltaTime;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                movement.X -= deltaTime;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                movement.X += deltaTime;
            }

            if (movement != Vector3.Zero)
            {
                movement = Vector3.Normalize(movement) * cMoveSpeed;
                parent.transform.position += movement;
            }
        }
    }
}
