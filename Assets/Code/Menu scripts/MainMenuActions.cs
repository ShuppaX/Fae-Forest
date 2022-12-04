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
        [SerializeField, Tooltip("The scene index of the first level.")] private int firstLevelIndex = 2;
        [SerializeField, Tooltip("The scene index of the tutorial level.")] private int tutorialLevelIndex = 1;

        [Header("Main menu gameobjects")]
        [SerializeField] private GameObject mainMenuButtons;
        [SerializeField] private GameObject mainMenuOptionsButton;
        [SerializeField] private GameObject tutorialButton;

        [Header("Options gameobjects")]
        [SerializeField, Tooltip("The options parent object.")] private GameObject options;
        [SerializeField] private GameObject optionsFirstButton;

        [Header("Main menu strings")]
        [SerializeField, Tooltip("The name of the main menu music.")] private string songName;
        [SerializeField, Tooltip("The name of the player pref used to store the score.")] private string scorePlayerPref = "Score";
        [SerializeField, Tooltip("The sound effect for pressing a button.")] private string selectSound = "Menu select 1";

        private AudioManager audioManager;
        private string storedHealth = "StoredHealth";
        private bool tutorialPlayed = false;
        private string tutorial = "TutorialCheck";
        private int tutorialCheck;
        private bool sfxPlayed = false;

        private void Awake()
        {
            audioManager = FindObjectOfType<AudioManager>();
            tutorialCheck = PlayerPrefs.GetInt(tutorial);

            CheckIfTutorialPlayed();
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
            audioManager.PlaySfx(selectSound);

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
            audioManager.PlaySfx(selectSound);

            options.SetActive(false);
            mainMenuButtons.SetActive(true);

            // Remove the currently selected object for the EventSystem
            EventSystem.current.SetSelectedGameObject(null);

            // Set a new selected object for the EventSystem
            EventSystem.current.SetSelectedGameObject(mainMenuOptionsButton);
        }

        // Method to quit the game.
        public void QuitGame()
        {
            audioManager.PlaySfx(selectSound);
            Application.Quit();
        }

        // Method to load the first level.
        public void StartFirstLevel()
        {
            audioManager.PlaySfx(selectSound);

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

        // Method to load the tutorial.
        public void StartTutorial()
        {
            audioManager.PlaySfx(selectSound);
            SceneManager.LoadScene(tutorialLevelIndex);
        }

        // Method that checks if the tutorial level has been played.
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

        // Method to play a SFX check so the user can hear what they are changing and by how much.
        public void PlaySFXCheck()
        {
            if (!sfxPlayed)
            {
                audioManager.PlaySfx(selectSound);
                sfxPlayed = true;
                StartCoroutine(DelayForSFXCheck());
            }
        }
        
        // Enumerator to have a delay for SFX check sound, so it doesn't play literally every time
        // the slider value is changed.
        IEnumerator DelayForSFXCheck()
        {
            if (sfxPlayed)
            {
                yield return new WaitForSeconds(0.5f);
                sfxPlayed = false;
            }
        }
    }
}
