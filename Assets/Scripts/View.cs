using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazySamurai.RadialShooter
{
    public class View : MonoBehaviour
    {
        public Button playButton;
        public RectTransform gameOverPanel;
        public RectTransform winPanel;
        public TextMeshProUGUI score;
        public TextMeshProUGUI timer;
    }
}