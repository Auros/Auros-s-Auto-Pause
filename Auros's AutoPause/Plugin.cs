using BS_Utils.Gameplay;
using CustomUI.Settings;
using IllusionInjector;
using IllusionPlugin;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

using System;

namespace AutoPause
{
    public class Plugin : IPlugin
    {
        public static bool modEnable = false;
        public static string selectedCharacteristic;
        internal static bool isIsolated = false;
        internal static bool isLeftHanded = false;
        public static float[] numnums = { .05f, .1f, .15f, .2f, .25f, .3f };

        public string Name => "AutoPause";
        public string Version => "1.5.1";

        

        void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name == "GameCore")
            {
                isIsolated = BS_Utils.Gameplay.Gamemode.IsIsolatedLevel;
                if (!isIsolated)
                {
                    modEnable = false;
                    SharedCoroutineStarter.instance.StartCoroutine(DelayedEnable());
                    Pauser.OnLoad();
                    Log.AutoPause("Pauser component loaded");
                }
                else
                    Log.AutoPause("Isolated");
            }
        }

        public bool firstTime = true;

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "MenuCore")
            {
                Gamemode.Init();
                modEnable = false;
                var settingsSubmenu = SettingsUI.CreateSubMenu("AutoPause");
                var modenabled = settingsSubmenu.AddBool("AutoPause Enabled");
                modenabled.GetValue += delegate { return ModPrefs.GetBool("AutoPause | Main", "Enabled", true, true); };
                modenabled.SetValue += delegate (bool value) { ModPrefs.SetBool("AutoPause | Main", "Enabled", value); };

                var pausebool = settingsSubmenu.AddBool("FPS Pause");
                pausebool.GetValue += delegate { return ModPrefs.GetBool("AutoPause | Main", "FPSCheckerOn", false, true); };
                pausebool.SetValue += delegate (bool value) { ModPrefs.SetBool("AutoPause | Main", "FPSCheckerOn", value); };

                var voices = settingsSubmenu.AddBool("Voices");
                voices.GetValue += delegate { return ModPrefs.GetBool("AutoPause | Main", "Voices", true, true); };
                voices.SetValue += delegate (bool value) { ModPrefs.SetBool("AutoPause | Main", "Voices", value); };

                float[] fpsValues = { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90 };
                var fpsThreshold = settingsSubmenu.AddList("FPS Threshold", fpsValues);
                fpsThreshold.GetValue += delegate { return ModPrefs.GetFloat("AutoPause | Main", "FPSThreshold", 40, true); };
                fpsThreshold.SetValue += delegate (float value) { ModPrefs.SetFloat("AutoPause | Main", "FPSThreshold", value); };
                fpsThreshold.FormatValue += delegate (float value) { return string.Format("{0:0}", value); };

                var reaction = settingsSubmenu.AddList("Reaction Time", numnums);
                reaction.GetValue += delegate { return ModPrefs.GetFloat("AutoPause | Main", "ResponseTime", .2f, true); };
                reaction.SetValue += delegate (float value) { ModPrefs.SetFloat("AutoPause | Main", "ResponseTime", value); };
                reaction.FormatValue += delegate (float value) { return string.Format("{0:0.00}", value); };

                Log.AutoPause("Settings Created");
            }

            if (arg0.name == "MenuCore" && firstTime == true)
            {
                firstTime = false;
                TextObject.OnLoad();
                ///Despacito 2
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
