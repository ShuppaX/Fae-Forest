using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RemixGame
{
    public class ChangeScene : MonoBehaviour
    {
        [SerializeField] private float endOfLevelTime = 5.0f;

        private bool transitionStarted = false;

        private bool enemiesDead = false;

        private int activeSceneNumber;
        private int nextSceneNumber;

        private void Start()
        {
            activeSceneNumber = SceneManager.GetActiveScene().buildIndex;

            if (activeSceneNumber < 5)
            {
                nextSceneNumber = activeSceneNumber + 1;
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
