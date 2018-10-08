using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace AurosAutoPause
{
    public class Plugin : IPlugin
    {
        public string Name => "Auros's AutoPause";
        public string Version => "0.1.0";
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
            GameplayManager PMM = Resources.FindObjectsOfTypeAll<GameplayManager>().FirstOrDefault();
            if (PMM != null)
            {
                PMM.ResumeFromPause();
            }
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public float despacito;
        //ignore that ^^^

        private float nextActionTime = 0.0f;
        public float period = 0.1f;

        private static Vector3 text;
        private static Vector3 text2;

        public void OnUpdate()
        {
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;

                PlayerController HEL = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
                if (HEL == null)
                    return;
                Saber SaberThatIsLeft = HEL.leftSaber;
                Saber SaberThatIsRight = HEL.rightSaber;

                Vector3 LeftSaberHandleLocation = SaberThatIsLeft.handlePos;
                Vector3 RightSaberHandleLocation = SaberThatIsRight.handlePos;

                GamePauseManager GMM = Resources.FindObjectsOfTypeAll<GamePauseManager>().FirstOrDefault();
                if (GMM != null && GMM.pause == true)
                {
                    text = RightSaberHandleLocation;
                    text2 = LeftSaberHandleLocation;
                }
                else
                {
                    //FPS CHECKER

                    float fps = 1.0f / Time.deltaTime;
                    if (fps < 40.0f)
                    {
                        GameplayManager PMM = Resources.FindObjectsOfTypeAll<GameplayManager>().FirstOrDefault();
                        if (PMM != null)
                        {
                            PMM.Pause();
                        }
                    }

                    //TRACKING DETECTOR

                    if (text == LeftSaberHandleLocation || text2 == RightSaberHandleLocation)
                    {
                        GameplayManager PMM = Resources.FindObjectsOfTypeAll<GameplayManager>().FirstOrDefault();
                        if (PMM != null)
                        {
                            PMM.Pause();
                        }
                    }

                    text = LeftSaberHandleLocation;
                    text2 = RightSaberHandleLocation;

                }
            }
        }

        public void OnFixedUpdate()
        {
        }
    }
}
