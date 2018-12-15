using System;
using UnityEngine;
using IllusionPlugin;

namespace AurosAutoPause
{
    internal class PauseUI : MonoBehaviour
    {
        public static void CreateSettingsUI()
        {
            var subMenu = SettingsUI.CreateSubMenu("AutoPause");

            var en = subMenu.AddBool("AutoPause Enabled");
            en.GetValue += delegate { return ModPrefs.GetBool("Auros's AutoPause", "Enabled", true, true); };
            en.SetValue += delegate (bool value) { ModPrefs.SetBool("Auros's AutoPause", "Enabled", value); };

            var pq = subMenu.AddBool("FPS Pause");
            pq.GetValue += delegate { return ModPrefs.GetBool("Auros's AutoPause", "FPSCheckerOn", false, true); };
            pq.SetValue += delegate (bool value) { ModPrefs.SetBool("Auros's AutoPause", "FPSCheckerOn", value); };

            float[] fpsValues = { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90 };
            var fpsThreshold = subMenu.AddList("FPS Threshold", fpsValues);
            fpsThreshold.GetValue += delegate { return ModPrefs.GetFloat("Auros's AutoPause", "FPSThreshold", 40, true); };
            fpsThreshold.SetValue += delegate (float value) { ModPrefs.SetFloat("Auros's AutoPause", "FPSThreshold", value); };
            fpsThreshold.FormatValue += delegate (float value) { return string.Format("{0:0}", value); };
        }
    }
}