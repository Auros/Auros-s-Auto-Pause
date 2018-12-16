using CustomUI.GameplaySettings;
using CustomUI.MenuButton;
using CustomUI.Settings;
using IllusionPlugin;
using System.Collections;
using System.Linq;
using System.Media;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AurosAutoPause
{
    public class Plugin : IPlugin
    {
        public static bool modEnable = false;

        public string Name => "Auros's AutoPause";
        public string Version => "1.4.1";

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name == "GameCore")
            {
                SharedCoroutineStarter.instance.StartCoroutine(DelayedEnable());
                GameObject gameObject = new GameObject("Auros's AutoPause");
                Pauser pause = gameObject.AddComponent<Pauser>();
                pause.Awake();
                System.Console.WriteLine("[AutoPause] Pauser component loaded");
            }
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "Menu")
            {
                modEnable = false;
                var settingsSubmenu = SettingsUI.CreateSubMenu("AutoPause");
                var en = settingsSubmenu.AddBool("AutoPause Enabled");
                en.GetValue += delegate { return ModPrefs.GetBool("Auros's AutoPause", "Enabled", true, true); };
                en.SetValue += delegate (bool value) { ModPrefs.SetBool("Auros's AutoPause", "Enabled", value); };

                var pq = settingsSubmenu.AddBool("FPS Pause");
                pq.GetValue += delegate { return ModPrefs.GetBool("Auros's AutoPause", "FPSCheckerOn", false, true); };
                pq.SetValue += delegate (bool value) { ModPrefs.SetBool("Auros's AutoPause", "FPSCheckerOn", value); };

                float[] fpsValues = { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90 };
                var fpsThreshold = settingsSubmenu.AddList("FPS Threshold", fpsValues);
                fpsThreshold.GetValue += delegate { return ModPrefs.GetFloat("Auros's AutoPause", "FPSThreshold", 40, true); };
                fpsThreshold.SetValue += delegate (float value) { ModPrefs.SetFloat("Auros's AutoPause", "FPSThreshold", value); };
                fpsThreshold.FormatValue += delegate (float value) { return string.Format("{0:0}", value); };
                System.Console.WriteLine("[AutoPause] Settings Created");
                //System.Console.Read();

                //SharedCoroutineStarter.instance.StartCoroutine(DelayedEnable());
            }
        }

        public void OnApplicationQuit()
        {
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

        private IEnumerator DelayedEnable()
        {
            yield return new WaitForSeconds(2f);
            modEnable = true;
        }

            public void OnLevelWasInitialized(int level)
        {
        }
    }
}
