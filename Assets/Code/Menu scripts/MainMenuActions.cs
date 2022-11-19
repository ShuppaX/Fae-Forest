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
        [SerializeField] private int firstLevelIndex = 1;

        [Header("Main menu gameobjects")]
        [SerializeField] private GameObject mainMenuButtons;
        [SerializeField] private GameObject mainMenuOptionsButton;

        [Header("Options gameobjects")]
        [SerializeField] private GameObject options;
        [SerializeField] private GameObject optionsFirstButton;

        [Header("Main menu song name")]
        [SerializeField] private string songName;

        private AudioManager audioManager;

        private void Awake()
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        private void Start()
        {
            if (audioManager == null)
            {
                Debug.LogError("An audio manager was not found!");
            }

            audioManager.PlaySong(songName);
            Debug.Log("Playing song with the name: " + songName);
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
            SceneManager.LoadScene(firstLevelIndex);
        }
    }
}
