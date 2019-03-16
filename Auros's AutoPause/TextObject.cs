using CustomUI.BeatSaber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace AutoPause
{
    public class TextObject : MonoBehaviour
    {
        public static void OnLoad()
        {
            new GameObject("AutoPause | Text").AddComponent<TextObject>();
        }

        private Canvas _canvas;
        private static readonly Vector3 Position = new Vector3(0, 2.5f, 2.5f);
        private static readonly Vector3 Rotation = new Vector3(0, 0, 0);
        private static readonly Vector3 Scale = new Vector3(0.01f, 0.01f, 0.01f);
        private static readonly Vector2 CanvasSize = new Vector2(50, 50);

        public void Awake()
        {
            CreateText();
            DontDestroyOnLoad(this);
        }

        public static TMP_Text _alertType;

        public void CreateText()
        {
            try
            {
                gameObject.transform.localScale = Scale;
                _canvas = gameObject.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.WorldSpace;
                var rectTransform = _canvas.transform as RectTransform;
                rectTransform.sizeDelta = CanvasSize;

                _alertType = BeatSaberUI.CreateText(_canvas.transform as RectTransform, "", new Vector3(0f, .1f, 1.5f));
                _alertType.alignment = TextAlignmentOptions.Center;
                _alertType.overflowMode = TextOverflowModes.Overflow;
                _alertType.fontSize = 30f;
                _alertType.enableWordWrapping = false;
                _alertType.text = "";
                _alertType.rectTransform.position = new Vector3(0f, .1f, 1.5f);
                //gameObject.transform.Rotate(45f, 0f, 0f, 0f);
            }
            catch (Exception e)
            {
                Log.AutoPause("" + e);
            }
        }

    }
}
