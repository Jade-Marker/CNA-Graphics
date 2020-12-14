namespace CNA_Graphics
{
    class CameraManager
    {
        private static CameraManager instance = new CameraManager();

        private Camera _mainCamera;

        public static Camera MainCamera { get { return instance._mainCamera; } set { instance._mainCamera = value; } }

        private CameraManager()
        {
            _mainCamera = null;
        }
    }
}
