using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RemixGame
{
    public class EndScreenActions : MonoBehaviour
    {
        [Header("End screen game objects")]
        [SerializeField] private GameObject endTextAndCharsParent;
        [SerializeField] private GameObject creditsParent;

        [Header("Objects animators")]
        [SerializeField] private Animator endTextAndCharsAnim;
        [SerializeField] private Animator creditsAnim;

        [Header("Variables")]
        [SerializeField] private int timerToShowCredits = 5;
        [SerializeField] private int mainMenuIndex = 0;
        [SerializeField] private string songName = "Menu music";
        [SerializeField] private string selectSound = "Menu select 1";

        [Header("Anim parameters")]
        public const string endFadeParam = "Ending fade";

        private bool startFade = false;
        private bool fadeStarted = false;

        private AudioManager audioManager;

        private void Awake()
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
            audioManager.PlaySong(songName);

            endTextAndCharsParent.SetActive(true);
            StartCoroutine(FadeToCredits());
        }

        // Update is called once per frame
        void Update()
        {
            if (startFade && !fadeStarted)
            {
                fadeStarted = true;
                endTextAndCharsAnim.SetTrigger(endFadeParam);

                creditsParent.SetActive(true);
            }
        }

        // IEnumerator to have a set time for showing the ending screens end picture and text
        IEnumerator FadeToCredits()
        {
            yield return new WaitForSeconds(timerToShowCredits);
            startFade = true;
        }

        // Function to go back to main menu, used on a button
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(mainMenuIndex);
            audioManager.PlaySfx(selectSound);
        }
    }
}
