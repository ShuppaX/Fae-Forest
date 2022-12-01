using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace RemixGame
{
    public class MainMenuActions : MonoBehaviour
    {
        [Header("Variables")]
        [SerializeField] private int firstLevelIndex = 2;
        [SerializeField] private int tutorialLevelIndex = 1;

        [Header("Main menu gameobjects")]
        [SerializeField] private GameObject mainMenuButtons;
        [SerializeField] private GameObject mainMenuOptionsButton;
        [SerializeField] private GameObject tutorialButton;

        [Header("Options gameobjects")]
        [SerializeField] private GameObject options;
        [SerializeField] private GameObject optionsFirstButton;

        [Header("Main menu strings")]
        [SerializeField] private string songName;
        [SerializeField] private string scorePlayerPref = "Score";

        private AudioManager audioManager;
        private string storedHealth = "StoredHealth";
        private bool tutorialPlayed = false;
        private string tutorial = "TutorialCheck";
        private int tutorialCheck;

        private void Awake()
        {
            audioManager = FindObjectOfType<AudioManager>();
            tutorialCheck = PlayerPrefs.GetInt(tutorial);

            CheckIfTutorialPlayed();

            Debug.Log("Tutorial check value is: " + tutorialCheck);
            Debug.Log("TutorialPlayed bool is: " + tutorialPlayed);
        }

        private void Start()
        {
            if (audioManager == null)
            {
                Debug.LogError("An audio manager was not found!");
            }

            audioManager.PlaySong(songName);
        }

        // Method to open options in main menu, turns off the main buttons from the
        // menu and displays options elements. Also selects the first button in
        // the options to be the selected button.
        public void OpenOptions()
        {
            mainMenuButtons.SetActive(false);
            options.SetActive(true);

            // Remove the currently selected object for the EventSystem
            EventSystem.current.SetSelectedGameObject(null);

            // Set a new selected object for the EventSystem
            EventSystem.current.SetSelectedGameObject(optionsFirstButton);
        }

        // Method to close options in main menu, turns off options elements and
        // displays the main menu elements. Also selects the options button from
        // main menu elements to be the selected button.
        public void CloseOptions()
        {
            options.SetActive(false);
            mainMenuButtons.SetActive(true);

            // Remove the currently selected object for the EventSystem
            EventSystem.current.SetSelectedGameObject(null);

            // Set a new selected object for the EventSystem
            EventSystem.current.SetSelectedGameObject(mainMenuOptionsButton);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void StartFirstLevel()
        {
            PlayerPrefs.SetInt(scorePlayerPref, 0);
            PlayerPrefs.SetInt(storedHealth, 3);

            if (tutorialPlayed)
            {
                SceneManager.LoadScene(firstLevelIndex);
            }
            else if (!tutorialPlayed)
            {
                SceneManager.LoadScene(tutorialLevelIndex);
            }
        }

        public void StartTutorial()
        {
            SceneManager.LoadScene(tutorialLevelIndex);
        }

        private void CheckIfTutorialPlayed()
        {
            if (tutorialCheck == 0)
            {
                tutorialPlayed = false;
            }
            else if (tutorialCheck == 1)
            {
                tutorialPlayed = true;
                tutorialButton.SetActive(true);
            }
        }
    }
}
