using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    class Camera : Component
    {
        Matrix _projection;

        public Camera(Matrix projection)
        {
            _projection = projection;
        }

        public Matrix GetViewMatrix()
        {
            Matrix view = Matrix.Invert(parent.transform.CreateWorldMatrix());
            return view;
        }

        public Matrix GetProjectionMatrix()
        {
            return _projection;
        }
    }
}
