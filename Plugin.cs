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
            // Discard references to controllers / managers.
            _HEL = null;
            _GMM = null;
            _PMM = null;
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

        //This is where the plugin starts!

        PlayerController _HEL;
        PlayerController HEL
        {
            get
            {
                if (_HEL == null)
                    _HEL = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
                    
                return _HEL;
            }
        }
        
        GamePauseManager _GMM;
        GamePauseManager GMM
        {
            get
            {
                if (_GMM == null)
                    _GMM = Resources.FindObjectsOfTypeAll<GamePauseManager>().FirstOrDefault();
                    
                return _GMM;
            }
        }
        
        GameplayManager _PMM;
        GameplayManager PMM
        {
            get
            {
                if (_PMM == null)
                    _PMM = Resources.FindObjectsOfTypeAll<GameplayManager>().FirstOrDefault();
                    
                return _PMM;
            }
        }

        //Defining the time to repeat the OnUpdate Function
        private float nextActionTime = 0.0f;
        public float period = 0.1f;

        //Previous Saber Values
        private static Vector3 PreviousLeftSaberHandleLocation;
        private static Vector3 PreviousLeftSaberHandleLocation2;

        //Repeat The Thing
        public void OnUpdate()
        {
            //Slowing The Repeat Thing
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;

                //Finding Saber Location
                if (HEL == null)
                    return;
                Saber SaberThatIsLeft = HEL.leftSaber;
                Saber SaberThatIsRight = HEL.rightSaber;
                Vector3 LeftSaberHandleLocation = SaberThatIsLeft.handlePos;
                Vector3 RightSaberHandleLocation = SaberThatIsRight.handlePos;

                //When the game is paused, saber position freezes. This if statement is to make sure that when the game is unpaused, it doesn't take the value which set off the tracking issue in the first place (if that makes any sense)
                if (GMM != null && GMM.pause == true)
                {
                    PreviousLeftSaberHandleLocation = RightSaberHandleLocation;
                    PreviousLeftSaberHandleLocation2 = LeftSaberHandleLocation;
                }
                else
                {
                    //FPS CHECKER
                    float fps = 1.0f / Time.deltaTime;
                    if (fps < 40.0f)
                    {
                        if (PMM != null)
                        {
                            PMM.Pause();
                        }
                    }

                    //TRACKING DETECTOR
                    if (PreviousLeftSaberHandleLocation == LeftSaberHandleLocation || PreviousLeftSaberHandleLocation2 == RightSaberHandleLocation)
                    {
                        if (PMM != null)
                        {
                            PMM.Pause();
                        }
                    }

                    //Set Saber Locations To Previous Saber Location
                    PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
                    PreviousLeftSaberHandleLocation2 = RightSaberHandleLocation;
                }
            }
        }
        public void OnFixedUpdate()
        {
        }
    }
}
