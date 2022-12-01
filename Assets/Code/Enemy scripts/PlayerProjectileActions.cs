using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class PlayerProjectileActions : MonoBehaviour
    {
        [SerializeField] private string damageTag = "DamageProjectile";
        [SerializeField] private string magicTag = "MagicProjectile";
        [SerializeField] private float timeForStoppedActions = 2.5f;
        [SerializeField] private int[] scoreValues = { 200, 400, 500 };

        [Header("Animation parameters")]
        public const string DeathParam = "Death";

        private bool stopActions = false;
        private bool deathSequence = false;
        private bool stopActionsActive = false;

        private int healthIndex;

        private Animator animator;
        private PlayerHealthSystem playerHealthSystem;
        private ScoreManager scoreManager;
        private Rigidbody2D rb;

        public bool StopActions
        {
            get { return stopActions; }
        }

        public bool DeathSequence
        {
            get { return deathSequence; }
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            playerHealthSystem = FindObjectOfType<PlayerHealthSystem>();
            scoreManager = FindObjectOfType<ScoreManager>();

            CheckForNulls();

            CheckPlayersHealth();
        }

        private void Update()
        {
            CheckPlayersHealth();
        }

        private void CheckForNulls()
        {
            if (playerHealthSystem == null)
            {
                Debug.LogError(gameObject.name + "is missing the player health system!");
            }

            if (scoreManager == null)
            {
                Debug.LogError(gameObject.name + " is missing the score manager!");
            }
        }

        // Simple OnCollision detection for objects with tags
        // to destroy the gameobject or freeze the object
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals(damageTag) && !deathSequence)
            {
                deathSequence = true;
                animator.SetTrigger(DeathParam);
                Destroy(gameObject.GetComponent<CapsuleCollider2D>());
                AddScore();
            }
            else if (collision.gameObject.tag.Equals(magicTag))
            {
                if (!stopActions)
                {
                    stopActions = true;
                    Debug.Log("Actions are stopped now!");
                }
                
                if (!stopActionsActive)
                {
                    StartCoroutine(ContinueActions());
                }
                stopActionsActive = true;
            }
        }

        // IEnumerator to turn the actions back on after a set period of time.
        IEnumerator ContinueActions()
        {
            yield return new WaitForSeconds(timeForStoppedActions);
            stopActions = false;
            Debug.Log("Actions are active again!");
            stopActionsActive = false;
        }

        // Public method to destroy the object (used with animations)
        public void DestroyObject()
        {
            Destroy(gameObject);
        }

        // Method to add score with the set value
        private void AddScore()
        {
            scoreManager.ScoreValue += scoreValues[healthIndex];
        }

        // Method to check the players current health, which affects the score given
        // for a kill.
        private void CheckPlayersHealth()
        {
            healthIndex = playerHealthSystem.PlayerCurrentHealth - 1;
        }
    }
}
