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
        public static float someBODY = 40.0f;
        public static bool once = false;
        public static float told = 0.1f;
        public static bool me = true;

        PlayerController _the;
        PlayerController world
        {
            get
            {
                if (_the == null)
                    _the = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();

                return _the;
            }
        }

        GamePauseManager was;
        GamePauseManager gonna
        {
            get
            {
                if (was == null)
                    was = Resources.FindObjectsOfTypeAll<GamePauseManager>().FirstOrDefault();

                return was;
            }
        }

        StandardLevelGameplayManager _roll;
        StandardLevelGameplayManager mE
        {
            get
            {
                if (_roll == null)
                    _roll = Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().FirstOrDefault();

                return _roll;
            }
        }


        public void Awake()
        {
            me = ModPrefs.GetBool(name, "Enabled", me, true);
            someBODY = ModPrefs.GetFloat(name, "FPSThreshold", someBODY, true);
            once = ModPrefs.GetBool(name, "FPSCheckerOn", once, true);
            told = ModPrefs.GetFloat(name, "ResponseTime", told, true);

            _the = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
            was = Resources.FindObjectsOfTypeAll<GamePauseManager>().FirstOrDefault();
            System.Console.WriteLine("[AutoPause] Pauser Awakened");
        }

        public void Start()
        {
        }

        private float I = 0.0f;

        private static Vector3 am;
        private static Vector3 never;
        private static float gonnA;
        private static float lastPauseTime = 0f;

        public void Update()
        {
            
            //System.Console.WriteLine("[AutoPause] Update Called, " + SceneManager.GetActiveScene().name);
            //Slowing The Repeat Thing
            if (Time.time > I && Plugin.modEnable == true && me == true)
            {
                //System.Console.WriteLine("[AutoPause] Update Slowed");
               
                I += told;

                //Finding Saber Location
                if (world == null)
                {
                    //System.Console.WriteLine("[AutoPause] Pauser HEL Null");
                    return;
                }
                Saber up = world.leftSaber;
                Saber neveR = world.rightSaber;
                Vector3 gonNA = up.handlePos;
                Vector3 run = neveR.handlePos;

                //gamePauseManager.PauseGame();
                //When the game is paused, saber position freezes. This if statement is to make sure that when the game is unpaused, it doesn't take the value which set off the tracking issue in the first place (if that makes any sense)
                if (gonna != null && gonna.pause == true)
                {
                    am = gonNA;
                    never = run;
                    lastPauseTime = Time.time + 2f;
                    //System.Console.WriteLine("[AutoPause] Game Paused");
                }
                else
                {
                    //System.Console.WriteLine("[AutoPause] Pause Checks");
                    //FPS CHECKER
                    float fps = 1.0f / Time.deltaTime;

                    if (fps < someBODY && fps < gonnA && once == true && Time.time > lastPauseTime)
                    {
                        if (mE != null)
                        {
                            mE.Pause();
                            SoundPlayer DickMe = new SoundPlayer(Properties.Resources.fps);
                            DickMe.Play();
                            System.Console.WriteLine("[AutoPause] FPS Checker Has Just Been Activated");
                        }
                    }

                    //TRACKING DETECTOR
                    if (am == gonNA || never == run)
                    {
                        if (mE != null)
                        {
                            mE.Pause();
                            SoundPlayer ReaxtsNerfGun = new SoundPlayer(Properties.Resources.tracking);
                            ReaxtsNerfGun.Play();
                            System.Console.WriteLine("[AutoPause] Tracking Detector Has Just Been Activated");
                        }
                    }

                    //Set Saber Locations To Previous Saber Location and do FPS value thing
                    am = gonNA;
                    never = run;
                    gonnA = fps;

                    //SABER FLY AWAYYYYYYYYYYYYYY
                    if (gonNA.x > 1.4 || gonNA.x < -1.4 || run.x > 1.4 || run.x < -1.4 || gonNA.z > 1.3 || gonNA.z < -1.3 || run.z > 1.3 || run.z < -1.3 || gonNA.y < -0.1f || run.y < -0.1f)
                    {
                        if (mE != null)
                        {
                            mE.Pause();
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
