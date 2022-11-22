using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField] private float Speed = 4f;

        [SerializeField] private float projectileDespawnTime = 0.5f;

        private Rigidbody2D projectileRb;

        private GameObject player;

        private bool playerFacingRight;

        private bool timerPassed = false;

        public bool TimerPassed
        {
            get { return timerPassed; }
        }

        // Initializing the projectileRb with the gameobjects rigidbody
        // also used to check if the player character is facing left or right
        // which is used to determine the direction of movement for the
        // projectile
        protected virtual void Awake()
        {
            projectileRb = gameObject.GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogError("Object with the tag 'Player' was not found for the projectile.");
            }

            if (player.GetComponent<Character>().FacingRight)
            {
                playerFacingRight = true;
            }
            else if (!player.GetComponent<Character>().FacingRight)
            {
                playerFacingRight = false;
            }

            StartCoroutine(DespawnTimer());
        }

        // The projectile is destroyed if the timercoroutine has passed.
        protected virtual void Update()
        {
            if (timerPassed)
            {
                Destroy(gameObject);
            }
        }

        // This is used for the movement of the projectile
        // The if statement determines the direction of movement for the projectile
        protected virtual void FixedUpdate()
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
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
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
