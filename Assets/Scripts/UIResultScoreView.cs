﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Linq;

namespace Unity1Week
{
    public class UIResultScoreView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI scoreText;

        void Start()
        {
            scoreText.text = "0";
        }

        public void UpdateScore(float score)
        {
            scoreText.text = $"{score}";
        }
    }
}