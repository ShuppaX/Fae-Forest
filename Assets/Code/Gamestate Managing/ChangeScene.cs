using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RemixGame
{
    public class ChangeScene : MonoBehaviour
    {
        [SerializeField] private int indexOfFinalLevel = 4;
        [SerializeField] private int indexOfFirstLevel = 0;
        [SerializeField] private int indexOfTutorialLevel = 1;
        [SerializeField] private int indexOfEndingScreen;
        [SerializeField] private float endOfLevelTime = 5.0f;

        private string tutorial = "TutorialCheck";

        private bool transitionStarted = false;

        private bool enemiesDead = false;

        private int activeSceneIndex;
        private int nextSceneIndex;

        private ScoreManager scoreManager;
        private PlayerHealthSystem playerHealthSystem;

        private void Awake()
        {
            scoreManager = FindObjectOfType<ScoreManager>();
            playerHealthSystem = FindObjectOfType<PlayerHealthSystem>();

            if (scoreManager == null)
            {
                Debug.LogError(gameObject.name + " couldn't find the score manager!");
            }
        }

        private void Start()
        {
            activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (activeSceneIndex < indexOfFinalLevel)
            {
                nextSceneIndex = activeSceneIndex + 1;
            }
            else if (activeSceneIndex == indexOfFinalLevel)
            {
                nextSceneIndex = indexOfEndingScreen;
            }

            if (activeSceneIndex == indexOfTutorialLevel)
            {
                PlayerPrefs.SetInt(tutorial, 1);
            }
        }

        private void Update()
        {
            CheckForEnemies();

            if (enemiesDead && !transitionStarted)
            {
                StartCoroutine(NextScene());
            }
        }

        // Method to check if any enemies are left, if not toggle enemiesDead bool to be true.
        private void CheckForEnemies()
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                enemiesDead = true;
            }
        }

        // Method to transition to next scene, save the score of the current level and then
        // use the method to add health if necessary.
        IEnumerator NextScene()
        {
            transitionStarted = true;

            scoreManager.SaveScore();

            playerHealthSystem.AddHealth();

            yield return new WaitForSeconds(endOfLevelTime);
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
