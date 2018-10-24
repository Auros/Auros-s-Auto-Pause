using System;
using UnityEngine;
using BeatSaberUI;
using IllusionPlugin;

namespace AurosAutoPause
{
    internal class PauseUI : MonoBehaviour
    {
        public static void CreateSettingsUI()
        {
            var subMenu = SettingsUI.CreateSubMenu("AutoPause");

            var en = subMenu.AddBool("AutoPause Enabled"); // Passing in the option label
            en.GetValue += delegate { return ModPrefs.GetBool("Auros's AutoPause", "Enabled", true, true); };
            en.SetValue += delegate (bool value) { ModPrefs.SetBool("Auros's AutoPause", "Enabled", value); };

            var pq = subMenu.AddBool("FPS Pause"); // Passing in the option label
            pq.GetValue += delegate { return ModPrefs.GetBool("Auros's AutoPause", "FPSCheckerOn", false, true); };
            pq.SetValue += delegate (bool value) { ModPrefs.SetBool("Auros's AutoPause", "FPSCheckerOn", value); };
        }
    }
}