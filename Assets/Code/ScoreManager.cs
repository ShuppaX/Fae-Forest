using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RemixGame
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreDisplay;
        [SerializeField] private string scoreText = "Score: ";
        [SerializeField] private string scorePlayerPref = "Score";

        private int scoreValue;

        public int ScoreValue
        {
            get { return scoreValue; }
            set
            {
                scoreValue = value;
                scoreDisplay.text = scoreText + scoreValue;
            }
        }

        private void Awake()
        {
            scoreValue = PlayerPrefs.GetInt(scorePlayerPref, 0);
        }

        public void SaveScore()
        {
            PlayerPrefs.SetInt(scorePlayerPref, scoreValue);
        }
    }
}
