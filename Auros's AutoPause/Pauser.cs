using IllusionPlugin;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AurosAutoPause
{
    public class Pauser : MonoBehaviour
    {
        public static float threshold = 40.0f;
        public static bool yeet = true;

        public void Awake()
        {
            threshold = ModPrefs.GetFloat(name, "FPSThreshold", threshold, true);
            yeet = ModPrefs.GetBool(name, "FPSCheckerOn", yeet, true);
        }

        public void Start()
        {
        }

        //Defining the time to repeat the OnUpdate Function
        private float nextActionTime = 0.0f;
        public float period = 0.1f;

        //Previous Saber and FPS Values
        private static Vector3 PreviousLeftSaberHandleLocation;
        private static Vector3 PreviousLeftSaberHandleLocation2;
        private static float despacito;

        public void Update()
        {
            //Slowing The Repeat Thing
            PlayerController HEL = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;

                //Loading In Things I Need
                GamePauseManager GMM = Resources.FindObjectsOfTypeAll<GamePauseManager>().FirstOrDefault();
                GameplayManager PMM = Resources.FindObjectsOfTypeAll<GameplayManager>().FirstOrDefault();

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
                    if (fps < threshold && fps < despacito && yeet == true)
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
