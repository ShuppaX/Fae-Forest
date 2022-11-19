using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RemixGame
{
    public class ChangeScene : MonoBehaviour
    {
        [SerializeField] private int indexOfLastLevel = 4;
        [SerializeField] private int indexOfFirstLevel = 0;
        [SerializeField] private float endOfLevelTime = 5.0f;

        private bool transitionStarted = false;

        private bool enemiesDead = false;

        private int activeSceneNumber;
        private int nextSceneNumber;

        private void Start()
        {
            activeSceneNumber = SceneManager.GetActiveScene().buildIndex;

            if (activeSceneNumber < indexOfLastLevel)
            {
                nextSceneNumber = activeSceneNumber + 1;
            }
            else
            {
                nextSceneNumber = indexOfFirstLevel;
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

            yield return new WaitForSeconds(endOfLevelTime);
            SceneManager.LoadScene(nextSceneNumber);
        }
    }
}
