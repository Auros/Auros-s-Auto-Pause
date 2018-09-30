using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace AurosAutoPause
{
    public class Plugin : IPlugin
    {
        public string Name => "Auros's AutoPause";
        public string Version => "0.0.4";
        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
            float fps = 1.0f / Time.deltaTime;
            if (fps < 40.0)
            {
                GameplayManager PMM = (GameplayManager)Resources.FindObjectsOfTypeAll(typeof(GameplayManager))[0];
                PMM.Pause();
            }
        }

        public void OnFixedUpdate()
        {
        }
    }
}
