using RemixGame.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class PlayerProjectile : MonoBehaviour
    {
        [SerializeField] private float Speed = 4f;

        [SerializeField] private float projectileDespawnTime = 0.5f;

        private Rigidbody2D projectileRb;

        private GameObject player;

        private bool playerFacingRight;

        private bool timerPassed = false;

        // Initializing the projectileRb with the gameobjects rigidbody
        // also used to check if the player character is facing left or right
        // which is used to determine the direction of movement for the
        // projectile
        private void Awake()
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
            Destroy(gameObject);
        }

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
