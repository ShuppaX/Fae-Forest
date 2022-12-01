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

        // Public score value with a setter to be able to change it straight
        // from the dying monsters
        public int ScoreValue
        {
            get { return scoreValue; }
            set
            {
                scoreValue = value;
                scoreDisplay.text = scoreText + scoreValue;
            }
        }

        // Getting the scoreValue on start.
        private void Start()
        {
            scoreValue = PlayerPrefs.GetInt(scorePlayerPref, 0);
            scoreDisplay.text = scoreText + scoreValue;
        }

        // Method to save the score, used before exiting the scene to another
        public void SaveScore()
        {
            PlayerPrefs.SetInt(scorePlayerPref, scoreValue);
        }
    }
}
