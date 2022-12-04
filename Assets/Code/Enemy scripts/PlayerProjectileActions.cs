using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class PlayerProjectileActions : MonoBehaviour
    {
        [SerializeField, Tooltip("The tag of the players damaging projectile.")] private string damageTag = "DamageProjectile";
        [SerializeField, Tooltip("The tag of the players magic projectile.")] private string magicTag = "MagicProjectile";
        [SerializeField, Tooltip("The duration of the 'stun' effect.")] private float timeForStoppedActions = 2.5f;
        [SerializeField, Tooltip("Score granted when killed. 1hp value, 2hp value, 3hp value")] private int[] scoreValues = { 200, 400, 500 };
        [SerializeField, Tooltip("The death sound of the charcter.")] private string deathSoundName;

        [Header("Animation parameters")]
        public const string DeathParam = "Death";

        private bool stopActions = false;
        private bool deathSequence = false;
        private bool stopActionsActive = false;

        private int healthIndex;

        private Animator animator;
        private PlayerHealthSystem playerHealthSystem;
        private ScoreManager scoreManager;
        private AudioManager audioManager;

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
            playerHealthSystem = FindObjectOfType<PlayerHealthSystem>();
            scoreManager = FindObjectOfType<ScoreManager>();
            audioManager = FindObjectOfType<AudioManager>();

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
                audioManager.PlaySfx(deathSoundName);
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
