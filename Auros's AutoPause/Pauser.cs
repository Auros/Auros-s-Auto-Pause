using IllusionPlugin;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;
using System.Media;

namespace AurosAutoPause
{
    public class Pauser : MonoBehaviour
    {
        public static float threshold = 40.0f;
        public static bool fpsCheckEnable = false;
        public static float updatePeriod = 0.1f;
        public static bool iniModEnable = true;

        PlayerController _playerController;
        PlayerController playerController
        {
            get
            {
                if (_playerController == null)
                    _playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();

                return _playerController;
            }
        }

        GamePauseManager _gamePauseManager;
        GamePauseManager gamePauseManager
        {
            get
            {
                if (_gamePauseManager == null)
                    _gamePauseManager = Resources.FindObjectsOfTypeAll<GamePauseManager>().FirstOrDefault();

                return _gamePauseManager;
            }
        }

        StandardLevelGameplayManager _gamePlayManager;
        StandardLevelGameplayManager gamePlayManager
        {
            get
            {
                if (_gamePlayManager == null)
                    _gamePlayManager = Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().FirstOrDefault();

                return _gamePlayManager;
            }
        }


        public void Awake()
        {
            iniModEnable = ModPrefs.GetBool(name, "Enabled", iniModEnable, true);
            threshold = ModPrefs.GetFloat(name, "FPSThreshold", threshold, true);
            fpsCheckEnable = ModPrefs.GetBool(name, "FPSCheckerOn", fpsCheckEnable, true);
            updatePeriod = ModPrefs.GetFloat(name, "ResponseTime", updatePeriod, true);

            _playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
            _gamePauseManager = Resources.FindObjectsOfTypeAll<GamePauseManager>().FirstOrDefault();
            System.Console.WriteLine("[AutoPause] Pauser Awakened");
        }

        public void Start()
        {
        }

        private float nextActionTime = 0.0f;

        //Previous Saber and FPS Values
        private static Vector3 PreviousLeftSaberHandleLocation;
        private static Vector3 PreviousRightSaberHandleLocation;
        private static float prevFps;
        private static float lastPauseTime = 0f;

        public void Update()
        {
            
            //System.Console.WriteLine("[AutoPause] Update Called, " + SceneManager.GetActiveScene().name);
            //Slowing The Repeat Thing
            if (Time.time > nextActionTime && Plugin.modEnable == true && iniModEnable == true)
            {
                //System.Console.WriteLine("[AutoPause] Update Slowed");
               
                nextActionTime += updatePeriod;

                //Finding Saber Location
                if (playerController == null)
                {
                    //System.Console.WriteLine("[AutoPause] Pauser HEL Null");
                    return;
                }
                Saber SaberThatIsLeft = playerController.leftSaber;
                Saber SaberThatIsRight = playerController.rightSaber;
                Vector3 LeftSaberHandleLocation = SaberThatIsLeft.handlePos;
                Vector3 RightSaberHandleLocation = SaberThatIsRight.handlePos;

                //gamePauseManager.PauseGame();
                //When the game is paused, saber position freezes. This if statement is to make sure that when the game is unpaused, it doesn't take the value which set off the tracking issue in the first place (if that makes any sense)
                if (gamePauseManager != null && gamePauseManager.pause == true)
                {
                    PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
                    PreviousRightSaberHandleLocation = RightSaberHandleLocation;
                    lastPauseTime = Time.time + 2f;
                    //System.Console.WriteLine("[AutoPause] Game Paused");
                }
                else
                {
                    //System.Console.WriteLine("[AutoPause] Pause Checks");
                    //FPS CHECKER
                    float fps = 1.0f / Time.deltaTime;

                    if (fps < threshold && fps < prevFps && fpsCheckEnable == true && Time.time > lastPauseTime)
                    {
                        if (gamePlayManager != null)
                        {
                            gamePlayManager.Pause();
                            SoundPlayer DickMe = new SoundPlayer(Properties.Resources.fps);
                            DickMe.Play();
                            System.Console.WriteLine("[AutoPause] FPS Checker Has Just Been Activated");
                        }
                    }

                    //TRACKING DETECTOR
                    if (PreviousLeftSaberHandleLocation == LeftSaberHandleLocation || PreviousRightSaberHandleLocation == RightSaberHandleLocation)
                    {
                        if (gamePlayManager != null)
                        {
                            gamePlayManager.Pause();
                            gamePauseManager.PauseGame();
                            SoundPlayer ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);
                            ReaxtsNerfGun.Play();
                            System.Console.WriteLine("[AutoPause] Tracking Detector Has Just Been Activated");
                        }
                    }

                    //Set Saber Locations To Previous Saber Location and do FPS value thing
                    PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
                    PreviousRightSaberHandleLocation = RightSaberHandleLocation;
                    prevFps = fps;

                    //SABER FLY AWAYYYYYYYYYYYYYY
                    if (LeftSaberHandleLocation.x > 1.4 || LeftSaberHandleLocation.x < -1.4 || RightSaberHandleLocation.x > 1.4 || RightSaberHandleLocation.x < -1.4 || LeftSaberHandleLocation.z > 1.3 || LeftSaberHandleLocation.z < -1.3 || RightSaberHandleLocation.z > 1.3 || RightSaberHandleLocation.z < -1.3 || LeftSaberHandleLocation.y < -0.1f || RightSaberHandleLocation.y < -0.1f)
                    {
                        if (gamePlayManager != null)
                        {
                            gamePlayManager.Pause();
                            SoundPlayer ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);
                            ReaxtsNerfGun.Play();
                            System.Console.WriteLine("[AutoPause] Saber Fly Away Has Just Been Activated");
                        }
                    }
                }
            }
        }
    }
}
