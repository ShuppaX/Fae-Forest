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
        [SerializeField] private float endOfLevelTime = 5.0f;

        private string tutorial = "TutorialCheck";

        private bool transitionStarted = false;

        private bool enemiesDead = false;

        private int activeSceneNumber;
        private int nextSceneNumber;

        private ScoreManager scoreManager;

        private void Awake()
        {
            scoreManager = FindObjectOfType<ScoreManager>();

            if (scoreManager == null)
            {
                Debug.LogError(gameObject.name + " couldn't find the score manager!");
            }
        }

        private void Start()
        {
            activeSceneNumber = SceneManager.GetActiveScene().buildIndex;

            if (activeSceneNumber < indexOfFinalLevel)
            {
                nextSceneNumber = activeSceneNumber + 1;
            }
            else
            {
                nextSceneNumber = indexOfFirstLevel;
            }

            if (activeSceneNumber == indexOfTutorialLevel)
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

        private void CheckForEnemies()
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                enemiesDead = true;
            }
        }

        IEnumerator NextScene()
        {
            transitionStarted = true;

            scoreManager.SaveScore();

            yield return new WaitForSeconds(endOfLevelTime);
            SceneManager.LoadScene(nextSceneNumber);
        }
    }
}
