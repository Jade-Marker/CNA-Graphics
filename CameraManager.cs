using System;
using System.Collections.Generic;
using System.Text;

namespace CNA_Graphics
{
    class CameraManager
    {
        private Camera _mainCamera;

        public static Camera MainCamera { get { return instance._mainCamera; } set { instance._mainCamera = value; } }

        private CameraManager()
        {
            _mainCamera = null;
        }

        static CameraManager instance = new CameraManager();

    }
}
