using IllusionPlugin;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;
using System.Media;
using BS_Utils.Utilities;
using UnityEngine.UI;
using CustomUI.BeatSaber;
using TMPro;

namespace AutoPause
{
    public class Pauser : MonoBehaviour
    {
        public static float threshold = 40.0f;
        public static bool fpsCheckEnable = false;
        public static float updatePeriod = 0.2f;
        public static bool iniModEnable = true;
        public static bool iniVoicesEnabled = true;
        public static string gameStateCheck;
        public static bool lefthanded;
        public static SoundPlayer FPSTracker = new SoundPlayer(Properties.Resources.fps);
        public static SoundPlayer ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);

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

        public static void OnLoad()
        {
            new GameObject("AutoPause | Main").AddComponent<Pauser>();
        }

        public void Awake()
        {
            iniVoicesEnabled = ModPrefs.GetBool(name, "Voices", iniVoicesEnabled, true);
            iniModEnable = ModPrefs.GetBool(name, "Enabled", iniModEnable, true);
            threshold = ModPrefs.GetFloat(name, "FPSThreshold", threshold, true);
            fpsCheckEnable = ModPrefs.GetBool(name, "FPSCheckerOn", fpsCheckEnable, true);
            updatePeriod = ModPrefs.GetFloat(name, "ResponseTime", updatePeriod, true);

            gameStateCheck = BS_Utils.Gameplay.Gamemode.GameMode;
            
            _playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
            _gamePlayManager = Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().FirstOrDefault();
            Log.AutoPause("Pauser Awakened");

            FPSTracker = new SoundPlayer(Properties.Resources.fps);
            ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);

            Setup();
        }

        public void Setup()
        {
            if (gameStateCheck != "One Saber")
                gameState = 1;
            else if (gameStateCheck == "One Saber" && lefthanded == false)
                gameState = 4;
            else if (gameStateCheck == "One Saber" && lefthanded == true)
                gameState = 5;
        }

        public void LeftHandCheck()
        {
            lefthanded = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.playerSpecificSettings.leftHanded;
        }

        private float nextActionTime = 0.0f;

        //Previous Saber and FPS Values
        private static Vector3 PreviousLeftSaberHandleLocation;
        private static Vector3 PreviousRightSaberHandleLocation;
        private static float prevFps;
        private static float lastPauseTime = 0f;

        public static int gameState;

        public void Update()
        {
            if (Time.time > nextActionTime && Plugin.modEnable == true && iniModEnable == true && Plugin.isIsolated == false)
            {
                nextActionTime += updatePeriod;

                Saber SaberThatIsLeft = playerController.leftSaber;
                Saber SaberThatIsRight = playerController.rightSaber;
                Vector3 LeftSaberHandleLocation = SaberThatIsLeft.handlePos;
                Vector3 RightSaberHandleLocation = SaberThatIsRight.handlePos;

                if (gamePlayManager != null && gamePlayManager.gameState == StandardLevelGameplayManager.GameState.Paused)
                {
                    PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
                    PreviousRightSaberHandleLocation = RightSaberHandleLocation;
                    lastPauseTime = Time.time + 2f;
                }

                if (gamePlayManager.gameState != StandardLevelGameplayManager.GameState.Paused)
                {
                    switch (gameState)
                    {
                        case 1:
                            Default(LeftSaberHandleLocation, RightSaberHandleLocation);
                            break;

                        case 2:
                            //OneSaber(RightSaberHandleLocation);
                            break;

                        case 3:
                            //LeftHanded(LeftSaberHandleLocation);
                            break;
                    }

                    FPSDrop();
                }
            }
        }

        public void Default(Vector3 LeftSaberHandleLocation, Vector3 RightSaberHandleLocation)
        {
            if (PreviousLeftSaberHandleLocation == LeftSaberHandleLocation || PreviousRightSaberHandleLocation == RightSaberHandleLocation)
            {
                TrackingError();
            }

            PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
            PreviousRightSaberHandleLocation = RightSaberHandleLocation;

            if (LeftSaberHandleLocation.x > 1.4 || LeftSaberHandleLocation.x < -1.4 || RightSaberHandleLocation.x > 1.4 || RightSaberHandleLocation.x < -1.4 || LeftSaberHandleLocation.z > 1.3 || LeftSaberHandleLocation.z < -1.3 || RightSaberHandleLocation.z > 1.3 || RightSaberHandleLocation.z < -1.3 || LeftSaberHandleLocation.y < -0.1f || RightSaberHandleLocation.y < -0.1f)
            {
                SaberCatch();
            }
        }

        public void OneSaber(Vector3 RightSaberHandleLocation)
        {
            if (PreviousRightSaberHandleLocation == RightSaberHandleLocation)
            {
                TrackingError();
            }

            PreviousRightSaberHandleLocation = RightSaberHandleLocation;

            if (RightSaberHandleLocation.x > 1.4 || RightSaberHandleLocation.x < -1.4 || RightSaberHandleLocation.z > 1.3 || RightSaberHandleLocation.z < -1.3 || RightSaberHandleLocation.y < -0.1f)
            {
                SaberCatch();
            }
        }

        public void LeftHanded(Vector3 LeftSaberHandleLocation)
        {
            if (PreviousLeftSaberHandleLocation == LeftSaberHandleLocation)
            {
                TrackingError();
            }

            PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
            

            if (LeftSaberHandleLocation.x > 1.4 || LeftSaberHandleLocation.x < -1.4 || LeftSaberHandleLocation.z > 1.3 || LeftSaberHandleLocation.z < -1.3 || LeftSaberHandleLocation.y < -0.1f)
            {
                SaberCatch();
            }
        }

        public void FPSDrop()
        {
            float fps = 1.0f / Time.deltaTime;
            if (fps < threshold && fps < prevFps && fpsCheckEnable == true && Time.time > lastPauseTime)
            {
                if (gamePlayManager != null)
                {
                    PauseSignalForFPS();
                    gamePlayManager.HandlePauseTriggered();
                    if (iniVoicesEnabled == true)
                    {
                        FPSTracker.Play();
                    }
                    Log.AutoPause("FPS Tracker Detected");
                }
            }

            prevFps = fps;
        }

        public void TrackingError()
        {
            if (gamePlayManager != null)
            {
                PauseSignalForSaberSkip();
                gamePlayManager.HandlePauseTriggered();
                PossiblyPlayTrackingErrorVoice();
                Log.AutoPause("Tracking Error Detected");
            }
        }

        public void SaberCatch()
        {
            if (gamePlayManager != null)
            {
                PauseSignalForFlyAway();
                gamePlayManager.HandlePauseTriggered();
                PossiblyPlayTrackingErrorVoice();
                Log.AutoPause("Catching Saber...");
            }
        }

        public void PossiblyPlayTrackingErrorVoice()
        {
            if (iniVoicesEnabled == true)
            {
                ReaxtsNerfGun.Play();
            }
        }

        

        private IEnumerator ResetText()
        {
            yield return new WaitForSecondsRealtime(4f);
            TextObject._alertType.text = "";
        }

        public void PauseSignalForFPS()
        {
            //Attach text to Pause Menu to say that FPS was the issue
            TextObject._alertType.text = "FPS Drop Detected";
            TextObject._alertType.color = Color.yellow;
            SharedCoroutineStarter.instance.StartCoroutine(ResetText());
        }

        public void PauseSignalForSaberSkip()
        {
            //Attach text to Pause Menu to say that Saber Skipping was the issue
            TextObject._alertType.text = "Saber Tracking Error Detected";
            TextObject._alertType.color = Color.green;
            SharedCoroutineStarter.instance.StartCoroutine(ResetText());
        }

        public void PauseSignalForFlyAway()
        {
            //Attach text to Pause Menu to say that Saber Bounds were violated
            TextObject._alertType.text = "A Saber Left The Play Space";
            TextObject._alertType.color = Color.cyan;
            SharedCoroutineStarter.instance.StartCoroutine(ResetText());
        }
    }
}