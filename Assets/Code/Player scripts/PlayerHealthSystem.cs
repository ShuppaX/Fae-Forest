using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RemixGame
{
    public class PlayerHealthSystem : MonoBehaviour
    {
        [Header("Variables")]
        [SerializeField] private int playerMaxHealth = 3;
        [SerializeField] private int firstLevelsIndex = 1;
        [SerializeField] private int healthDefaultValue = 3;

        [Header("Health indicators")]
        [SerializeField] private GameObject[] healthIndicators;

        [Header("Death menu / indicator")]
        [SerializeField] private GameObject deathIndicator;

        private string storedHealth = "StoredHealth";
        private bool firstLevelStarting;
        private Scene currentScene;
        private int damageToTake = 1;
        private int playerCurrentHealth;

        public int PlayerCurrentHealth 
        {
            get { return playerCurrentHealth; }
        }

        private void Awake()
        {
            currentScene = SceneManager.GetActiveScene();

            if (currentScene.buildIndex == firstLevelsIndex)
            {
                firstLevelStarting = true;
            }

            if (!firstLevelStarting)
            {
                playerCurrentHealth = PlayerPrefs.GetInt(storedHealth, healthDefaultValue);
            }

            if (healthIndicators == null)
            {
                Debug.LogError("Health indicators are missing!");
            }

            CheckIfFirstLevel();
        }

        private void Update()
        {
            CheckAmountOfHealth();
        }

        // This method is used to check if the current scene is the first level
        // just to make sure that the health indicators are set active.
        private void CheckIfFirstLevel()
        {
            if (firstLevelStarting)
            {
                foreach (GameObject gameObject in healthIndicators)
                {
                    gameObject.SetActive(true);
                }

                playerCurrentHealth = playerMaxHealth;

                PlayerPrefs.SetInt(storedHealth, playerCurrentHealth);

                Debug.Log("First level starting, the current stored health is: " + PlayerPrefs.GetInt(storedHealth));

                firstLevelStarting = false;
            }
        }

        // Method to reduce the players health and to store it to be used in other levels
        public void ReduceHealth()
        {
            healthIndicators[playerCurrentHealth - 1].SetActive(false);

            playerCurrentHealth -= damageToTake;

            PlayerPrefs.SetInt(storedHealth, playerCurrentHealth);

            Debug.Log("Stored players health to " + storedHealth + " with the amount of: " + playerCurrentHealth);
            Debug.Log("Stored health amount is: " + PlayerPrefs.GetInt(storedHealth));
        }

        // Method to check the players health and to trigger the death menu, if the
        // players health is 0
        private void CheckAmountOfHealth()
        {
            if (playerCurrentHealth == 3)
            {
                foreach (GameObject go in healthIndicators)
                {
                    go.SetActive(true);
                }
            }

            if (playerCurrentHealth == 2)
            {
                healthIndicators[2].SetActive(false);
            }

            if (playerCurrentHealth == 1)
            {
                healthIndicators[2].SetActive(false);
                healthIndicators[1].SetActive(false);
            }

            if (playerCurrentHealth == 0)
            {
                healthIndicators[2].SetActive(false);
                healthIndicators[1].SetActive(false);
                healthIndicators[0].SetActive(false);

                Time.timeScale = 0;

                deathIndicator.SetActive(true);
            }
        }
    }
}
