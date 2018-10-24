using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using VRUI;
using VRUIControls;
using TMPro;
using IllusionPlugin;
using HMUI;
using UnityEngine.Events;

namespace BeatSaberUI
{
    static class BSUIHelpers
    {
        public static bool isMenuScene(Scene scene)
        {
            return (scene.name == "Menu");
        }

        public static bool isGameScene(Scene scene)
        {
            return (scene.name == "StandardLevel");
        }
    }
}
