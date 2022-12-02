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
        [SerializeField] private int mainMenuIndex = 0;
        [SerializeField] private int healthDefaultValue = 3;
        [SerializeField] private float deathScreenTime = 5f;
        [SerializeField] private int healthIncrement = 1;

        [Header("Health indicators")]
        [SerializeField] private GameObject[] healthIndicators;

        [Header("Death menu / indicator")]
        [SerializeField] private GameObject deathIndicator;

        private string storedHealth = "StoredHealth";
        private int damageToTake = 1;
        private int playerCurrentHealth;
        private int playerHealthAtAwake;

        public int PlayerCurrentHealth 
        {
            get { return playerCurrentHealth; }
        }

        private void Awake()
        {
            playerCurrentHealth = PlayerPrefs.GetInt(storedHealth, healthDefaultValue);
            playerHealthAtAwake = playerCurrentHealth;

            if (healthIndicators == null)
            {
                Debug.LogError("Health indicators are missing!");
            }
        }

        private void Update()
        {
            CheckAmountOfHealth();
        }

        // Method to reduce the players health and to store it to be used in other levels
        public void ReduceHealth()
        {
            healthIndicators[playerCurrentHealth - 1].SetActive(false);

            playerCurrentHealth -= damageToTake;

            PlayerPrefs.SetInt(storedHealth, playerCurrentHealth);
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

                StartCoroutine(SendToMainMenu());
            }
        }

        // Death screen with a delay that sends the player back to the main menu and sets timescale back to 1.
        IEnumerator SendToMainMenu()
        {
            yield return new WaitForSecondsRealtime(deathScreenTime);

            PlayerPrefs.SetInt(storedHealth, playerMaxHealth);

            SceneManager.LoadScene(mainMenuIndex);

            Time.timeScale = 1;
        }

        // Method to add health to the player, if they clear the level without taking any damage.
        public void AddHealth()
        {
            if (playerCurrentHealth == playerHealthAtAwake)
            {
                if (playerCurrentHealth < playerMaxHealth)
                {
                    playerCurrentHealth += healthIncrement;

                    PlayerPrefs.SetInt(storedHealth, playerCurrentHealth);
                }
            }
        }
    }
}
