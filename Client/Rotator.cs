using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    class Rotator : Component
    {
        public override void Update(float deltaTime)
        {
            parent.transform.rotation.Y += MathHelper.ToRadians(30.0f) * deltaTime;
        }
    }
}
