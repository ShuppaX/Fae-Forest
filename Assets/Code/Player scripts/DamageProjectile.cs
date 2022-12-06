using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class DamageProjectile : MonoBehaviour
    {
        [SerializeField] private float Speed = 4f;
        [SerializeField] private float projectileDespawnTime = 0.5f;
        [SerializeField] private string impactSoundName;

        private Rigidbody2D projectileRb;
        private Character player;
        private AudioManager audioManager;

        private bool playerFacingRight;
        private bool timerPassed = false;

        // Initializing the projectileRb with the gameobjects rigidbody
        // also used to check if the player character is facing left or right
        // which is used to determine the direction of movement for the
        // projectile
        private void Awake()
        {
            projectileRb = gameObject.GetComponent<Rigidbody2D>();
            player = FindObjectOfType<Character>();
            audioManager = FindObjectOfType<AudioManager>();

            if (player == null)
            {
                Debug.LogError("The projectile didn't find a player character.");
            }

            if (player.FacingRight)
            {
                playerFacingRight = true;
            }
            else if (!player.FacingRight)
            {
                playerFacingRight = false;
            }

            StartCoroutine(DespawnTimer());
        }

        // The projectile is destroyed if the timercoroutine has passed.
        private void Update()
        {
            if (timerPassed)
            {
                Destroy(gameObject);
            }
        }

        // This is used for the movement of the projectile
        // The if statement determines the direction of movement for the projectile
        private void FixedUpdate()
        {
            if (playerFacingRight)
            {
                projectileRb.velocity = transform.right * Speed;
            }
            else if (!playerFacingRight)
            {
                projectileRb.velocity = -transform.right * Speed;
            }
        }

        // This is used to destroy the projectile if it collides with something
        // that it can collide with.
        private void OnCollisionEnter2D(Collision2D collision)
        {
            audioManager.PlaySfx(impactSoundName);
            Destroy(gameObject);
        }

        // Simple despawn timer coroutine to track determine if the projectile has been instantiated
        // for enough time.
        IEnumerator DespawnTimer()
        {
            if (!timerPassed)
            {
                yield return new WaitForSeconds(projectileDespawnTime);
                timerPassed = true;
            }
        }
    }
}