using IllusionPlugin;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AurosAutoPause
{
    public class Pauser : MonoBehaviour
    {
        public static float threshold = 40.0f;
        public static bool yeet = false;
        public static float period = 0.1f;
        public static bool despa = true;
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

        IEnumerator Girlfriend()
        {
            yield return new WaitForSeconds(1);
        }


        public void Awake()
        {
            despa = ModPrefs.GetBool(name, "Enabled", despa, true);
            threshold = ModPrefs.GetFloat(name, "FPSThreshold", threshold, true);
            yeet = ModPrefs.GetBool(name, "FPSCheckerOn", yeet, true);
            period = ModPrefs.GetFloat(name, "ResponseTime", period, true);

            _HEL = null;
            _GMM = null;
            _PMM = null;

            StartCoroutine(Girlfriend());

        }

        public void Start()
        {
            PMM.ResumeFromPause();
        }

        private float nextActionTime = 0.0f;

        //Previous Saber and FPS Values
        private static Vector3 PreviousLeftSaberHandleLocation;
        private static Vector3 PreviousLeftSaberHandleLocation2;
        private static float despacito;

        public void Update()
        {
            //Slowing The Repeat Thing
            if (Time.time > nextActionTime && despa == true)
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
                    
                    if (fps < threshold && fps < despacito && yeet == true && Plugin.yote == true)
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

                    //Set Saber Locations To Previous Saber Location and do FPS value thing
                    PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
                    PreviousLeftSaberHandleLocation2 = RightSaberHandleLocation;
                    despacito = fps;
                }
            }
        }
    }
}
