using Microsoft.Xna.Framework;

namespace CNA_Graphics
{
    class Camera : Component
    {
        private Matrix _projection;

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
