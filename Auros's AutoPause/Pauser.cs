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

namespace AurosAutoPause
{
    public class Pauser : MonoBehaviour
    {
        public static float threshold = 40.0f;
        public static bool fpsCheckEnable = false;
        public static float updatePeriod = 0.1f;
        public static bool iniModEnable = true;
        public static bool iniVoicesEnabled = true;
        public static string gameStateCheck;
        public static bool lefthanded;


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

        StandardLevelSceneSetupDataSO _sceneSetup;
        StandardLevelSceneSetupDataSO sceneSetup
        {
            get
            {
                if (_sceneSetup == null)
                    _sceneSetup = Resources.FindObjectsOfTypeAll<StandardLevelSceneSetupDataSO>().FirstOrDefault();

                return _sceneSetup;
            }
        }


        public void Awake()
        {
            iniVoicesEnabled = ModPrefs.GetBool(name, "Voices", iniVoicesEnabled, true);
            iniModEnable = ModPrefs.GetBool(name, "Enabled", iniModEnable, true);
            threshold = ModPrefs.GetFloat(name, "FPSThreshold", threshold, true);
            fpsCheckEnable = ModPrefs.GetBool(name, "FPSCheckerOn", fpsCheckEnable, true);
            updatePeriod = ModPrefs.GetFloat(name, "ResponseTime", updatePeriod, true);

            _playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
            _gamePlayManager = Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().FirstOrDefault();
            System.Console.WriteLine("[AutoPause] Pauser Awakened");

            gameStateCheck = BS_Utils.Gameplay.Gamemode.GameMode;
            CreateText();
            
        }

        public void Start()
        {
        }

        public void LeftHandCheck()
        {
            lefthanded = _sceneSetup.gameplayCoreSetupData.playerSpecificSettings.leftHanded;
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
            if (Time.time > nextActionTime && Plugin.modEnable == true && iniModEnable == true && Plugin.isInMultiplayer == false)
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

                //HOLY SHIT FUCK THIS IN THE YOGURT HOLE I HATED DOING THIS PART 
                if (gameStateCheck != "One Saber")
                {
                    //gamePauseManager.PauseGame();
                    //When the game is paused, saber position freezes. This if statement is to make sure that when the game is unpaused, it doesn't take the value which set off the tracking issue in the first place (if that makes any sense)
                    if (gamePlayManager != null && gamePlayManager.gameState == StandardLevelGameplayManager.GameState.Paused)
                    {
                        PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
                        PreviousRightSaberHandleLocation = RightSaberHandleLocation;
                        lastPauseTime = Time.time + 2f;
                    }
                    else
                    {
                        //FPS CHECKER
                        float fps = 1.0f / Time.deltaTime;

                        if (fps < threshold && fps < prevFps && fpsCheckEnable == true && Time.time > lastPauseTime)
                        {
                            if (gamePlayManager != null)
                            {
                                PauseSignalForFPS();
                                gamePlayManager.HandlePauseTriggered();
                                if (iniVoicesEnabled == true)
                                {
                                    SoundPlayer DickMe = new SoundPlayer(Properties.Resources.fps);
                                    DickMe.Play();
                                }
                                System.Console.WriteLine("[AutoPause] FPS Checker Has Just Been Activated");
                            }
                        }

                        //TRACKING DETECTOR
                        if (PreviousLeftSaberHandleLocation == LeftSaberHandleLocation || PreviousRightSaberHandleLocation == RightSaberHandleLocation)
                        {
                            if (gamePlayManager != null)
                            {
                                PauseSignalForSaberSkip();
                                gamePlayManager.HandlePauseTriggered();
                                if (iniVoicesEnabled == true)
                                {
                                    SoundPlayer ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);
                                    ReaxtsNerfGun.Play();
                                }
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
                                PauseSignalForFlyAway();
                                gamePlayManager.HandlePauseTriggered();
                                if (iniVoicesEnabled == true)
                                {
                                    SoundPlayer ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);
                                    ReaxtsNerfGun.Play();
                                }
                                System.Console.WriteLine("[AutoPause] Saber Fly Away Has Just Been Activated");
                            }
                        }
                    }
                } else if (gameStateCheck == "One Saber" && lefthanded == false)
                {
                    //gamePauseManager.PauseGame();
                    //When the game is paused, saber position freezes. This if statement is to make sure that when the game is unpaused, it doesn't take the value which set off the tracking issue in the first place (if that makes any sense)
                    if (gamePlayManager != null && gamePlayManager.gameState == StandardLevelGameplayManager.GameState.Paused)
                    {
                        PreviousRightSaberHandleLocation = RightSaberHandleLocation;
                        lastPauseTime = Time.time + 2f;
                    }
                    else
                    {
                        //FPS CHECKER
                        float fps = 1.0f / Time.deltaTime;

                        if (fps < threshold && fps < prevFps && fpsCheckEnable == true && Time.time > lastPauseTime)
                        {
                            if (gamePlayManager != null)
                            {
                                PauseSignalForFPS();
                                gamePlayManager.HandlePauseTriggered();
                                if (iniVoicesEnabled == true)
                                {
                                    SoundPlayer DickMe = new SoundPlayer(Properties.Resources.fps);
                                    DickMe.Play();
                                }
                                System.Console.WriteLine("[AutoPause] FPS Checker Has Just Been Activated");
                            }
                        }

                        //TRACKING DETECTOR
                        if (PreviousRightSaberHandleLocation == RightSaberHandleLocation)
                        {
                            if (gamePlayManager != null)
                            {
                                PauseSignalForSaberSkip();
                                gamePlayManager.HandlePauseTriggered();
                                if (iniVoicesEnabled == true)
                                {
                                    SoundPlayer ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);
                                    ReaxtsNerfGun.Play();
                                }
                                System.Console.WriteLine("[AutoPause] Tracking Detector Has Just Been Activated");
                            }
                        }

                        //Set Saber Locations To Previous Saber Location and do FPS value thing
                        PreviousRightSaberHandleLocation = RightSaberHandleLocation;
                        prevFps = fps;

                        //SABER FLY AWAYYYYYYYYYYYYYY
                        if (RightSaberHandleLocation.x > 1.4 || RightSaberHandleLocation.x < -1.4 || RightSaberHandleLocation.z > 1.3 || RightSaberHandleLocation.z < -1.3 || RightSaberHandleLocation.y < -0.1f)
                        {
                            if (gamePlayManager != null)
                            {
                                PauseSignalForFlyAway();
                                gamePlayManager.HandlePauseTriggered();
                                if (iniVoicesEnabled == true)
                                {
                                    SoundPlayer ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);
                                    ReaxtsNerfGun.Play();
                                }
                                System.Console.WriteLine("[AutoPause] Saber Fly Away Has Just Been Activated");
                            }
                        }
                    }
                } else if (gameStateCheck == "One Saber" && lefthanded == true)
                {
                    //gamePauseManager.PauseGame();
                    //When the game is paused, saber position freezes. This if statement is to make sure that when the game is unpaused, it doesn't take the value which set off the tracking issue in the first place (if that makes any sense)
                    if (gamePlayManager != null && gamePlayManager.gameState == StandardLevelGameplayManager.GameState.Paused)
                    {
                        PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
                        lastPauseTime = Time.time + 2f;
                    }
                    else
                    {
                        //FPS Checker
                        float fps = 1.0f / Time.deltaTime;

                        if (fps < threshold && fps < prevFps && fpsCheckEnable == true && Time.time > lastPauseTime)
                        {
                            if (gamePlayManager != null)
                            {
                                PauseSignalForFPS();
                                gamePlayManager.HandlePauseTriggered();
                                if (iniVoicesEnabled == true)
                                {
                                    SoundPlayer DickMe = new SoundPlayer(Properties.Resources.fps);
                                    DickMe.Play();
                                }
                                System.Console.WriteLine("[AutoPause] FPS Checker Has Just Been Activated");
                            }
                        }

                        //Tracking Detector
                        if (PreviousLeftSaberHandleLocation == LeftSaberHandleLocation)
                        {
                            if (gamePlayManager != null)
                            {
                                PauseSignalForSaberSkip();
                                gamePlayManager.HandlePauseTriggered();
                                if (iniVoicesEnabled == true)
                                {
                                    SoundPlayer ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);
                                    ReaxtsNerfGun.Play();
                                }
                                System.Console.WriteLine("[AutoPause] Tracking Detector Has Just Been Activated");
                            }
                        }

                        //Set Saber Locations To Previous Saber Location and do FPS value thing
                        PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
                        prevFps = fps;

                        //Saber Fly Away
                        if (LeftSaberHandleLocation.x > 1.4 || LeftSaberHandleLocation.x < -1.4 || LeftSaberHandleLocation.z > 1.3 || LeftSaberHandleLocation.z < -1.3 || LeftSaberHandleLocation.y < -0.1f)
                        {
                            if (gamePlayManager != null)
                            {
                                PauseSignalForFlyAway();
                                gamePlayManager.HandlePauseTriggered();
                                System.Console.WriteLine("[AutoPause] Saber Fly Away Has Just Been Activated");
                                if (iniVoicesEnabled == true)
                                {
                                    SoundPlayer ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);
                                    ReaxtsNerfGun.Play();
                                }
                            }
                        }
                    }
                }
            }
        }

        GameObject alertObject;
        TextMeshPro _alertType;

        public void CreateText()
        {
            alertObject = new GameObject("PauseTypeAlert");
            alertObject.transform.Rotate(45f, 0f, 0f, 0f);
            _alertType = alertObject.AddComponent<TextMeshPro>();
            _alertType.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _alertType.fontSize = 3;
            _alertType.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 5f);
            _alertType.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
            _alertType.alignment = TextAlignmentOptions.Center;
            _alertType.rectTransform.position = new Vector3(0f, .1f, 1.5f);
        }

        private IEnumerator ResetText()
        {
            yield return new WaitForSecondsRealtime(4f);
            _alertType.text = "";
        }

        public void PauseSignalForFPS()
        {
            //Attach text to Pause Menu to say that FPS was the issue
            _alertType.text = "FPS Drop Detected";
            _alertType.color = Color.yellow;
            SharedCoroutineStarter.instance.StartCoroutine(ResetText());
        }

        public void PauseSignalForSaberSkip()
        {
            //Attach text to Pause Menu to say that Saber Skipping was the issue
            _alertType.text = "Saber Tracking Error Detected";
            _alertType.color = Color.green;
            SharedCoroutineStarter.instance.StartCoroutine(ResetText());
        }

        public void PauseSignalForFlyAway()
        {
            //Attach text to Pause Menu to say that Saber Bounds were violated
            _alertType.text = "A Saber Left The Play Space";
            _alertType.color = Color.cyan;
            SharedCoroutineStarter.instance.StartCoroutine(ResetText());
        }
    }
}