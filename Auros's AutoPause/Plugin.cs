using BeatSaberUI;
using IllusionPlugin;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AurosAutoPause
{
    public class Plugin : IPlugin
    {
        public string Name => "Auros's AutoPause";
        public string Version => "0.1.3";

        private readonly string[] env = { "DefaultEnvironment", "BigMirrorEnvironment", "TriangleEnvironment", "NiceEnvironment" };
        public static bool yote = false;

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (scene.name == "Menu")
            {
                PauseUI.CreateSettingsUI();
            }
            yote = false;
            new GameObject("Auros's AutoPause").AddComponent<Pauser>();
           
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SharedCoroutineStarter.instance.StartCoroutine(DelayedYote());

        }

        public void OnApplicationQuit()
        {
            //sendnudestoreaxt
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnUpdate()
        {
        }
        public void OnFixedUpdate()
        {
        }
        private IEnumerator DelayedYote()
        {
            yield return new WaitForSeconds(1f);
            yote = true;
        }


            public void OnLevelWasInitialized(int level)
        {
        }

    }
}
