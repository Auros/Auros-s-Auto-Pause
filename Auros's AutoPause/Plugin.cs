using BS_Utils.Gameplay;
using CustomUI.Settings;
using IllusionInjector;
using IllusionPlugin;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using CountersPlus.Custom;
using System;

namespace AurosAutoPause
{
    public class Plugin : IPlugin
    {
        public static bool modEnable = false;
        public static string selectedCharacteristic;
        private static StandardLevelSceneSetupDataSO _mainGameSceneSetupData = null;
        internal static bool isIsolated = false;


        public string Name => "Auros's AutoPause";
        public string Version => "1.5.0";

        

        void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name == "GameCore")
            {
                isIsolated = BS_Utils.Gameplay.Gamemode.IsIsolatedLevel;
                if (_mainGameSceneSetupData == null)
                {
                    _mainGameSceneSetupData = Resources.FindObjectsOfTypeAll<StandardLevelSceneSetupDataSO>().FirstOrDefault();
                }
                
                if (_mainGameSceneSetupData != null)
                {
                    if (!isIsolated)
                    {
                        modEnable = false;
                        SharedCoroutineStarter.instance.StartCoroutine(DelayedEnable());
                        GameObject gameObject = new GameObject("Auros's AutoPause");
                        Pauser pause = gameObject.AddComponent<Pauser>();
                        pause.Awake();
                        Console.WriteLine("[AutoPause] Pauser component loaded");
                    }
                    else
                        Console.WriteLine("Isolated");


                }
                else
                {
                }


                //modEnable = false;
                //SharedCoroutineStarter.instance.StartCoroutine(DelayedEnable());
                //GameObject gameObject = new GameObject("Auros's AutoPause");
                //Pauser pause = gameObject.AddComponent<Pauser>();
                //pause.Awake();
                //Console.WriteLine("[AutoPause] Pauser component loaded");
            }
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "Menu")
            {
                Gamemode.Init();
                modEnable = false;
                var settingsSubmenu = SettingsUI.CreateSubMenu("AutoPause");
                var modenabled = settingsSubmenu.AddBool("AutoPause Enabled");
                modenabled.GetValue += delegate { return ModPrefs.GetBool("Auros's AutoPause", "Enabled", true, true); };
                modenabled.SetValue += delegate (bool value) { ModPrefs.SetBool("Auros's AutoPause", "Enabled", value); };

                var pausebool = settingsSubmenu.AddBool("FPS Pause");
                pausebool.GetValue += delegate { return ModPrefs.GetBool("Auros's AutoPause", "FPSCheckerOn", false, true); };
                pausebool.SetValue += delegate (bool value) { ModPrefs.SetBool("Auros's AutoPause", "FPSCheckerOn", value); };

                var voices = settingsSubmenu.AddBool("Voices");
                voices.GetValue += delegate { return ModPrefs.GetBool("Auros's AutoPause", "Voices", true, true); };
                voices.SetValue += delegate (bool value) { ModPrefs.SetBool("Auros's AutoPause", "Voices", value); };

                float[] fpsValues = { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90 };
                var fpsThreshold = settingsSubmenu.AddList("FPS Threshold", fpsValues);
                fpsThreshold.GetValue += delegate { return ModPrefs.GetFloat("Auros's AutoPause", "FPSThreshold", 40, true); };
                fpsThreshold.SetValue += delegate (float value) { ModPrefs.SetFloat("Auros's AutoPause", "FPSThreshold", value); };
                fpsThreshold.FormatValue += delegate (float value) { return string.Format("{0:0}", value); };
                Console.WriteLine("[AutoPause] Settings Created");
            }
        }

        private IEnumerator DelayedEnable()
        {
            yield return new WaitForSeconds(.5f);
            modEnable = true;
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
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

        

        public void OnLevelWasInitialized(int level)
        {
        }
    }
}
