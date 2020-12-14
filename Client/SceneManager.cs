using System.Collections.Generic;

namespace CNA_Graphics
{
    public class SceneManager
    {
        public static Scene currentScene { get; private set; }

        private static Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();

        public static void RegisterScene(string name, Scene scene)
        {
            _scenes.Add(name, scene);
        }

        public static void ChangeScene(string name)
        {
            Scene scene;

            if (_scenes.TryGetValue(name, out scene))
            {
                if (currentScene != null)
                    currentScene.End();

                currentScene = scene;

                scene.Start();
            }
        }
    }
}
