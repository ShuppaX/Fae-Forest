using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class FireProjectiles : MonoBehaviour
    {
        [SerializeField] private Transform projectileLaunchOffset;
        [SerializeField] private RangedEnemyProjectile enemyProjectilePrefab;
        [SerializeField] private float[] fireRates = { 1.30f, 1.15f, 1f };

        [Header("Animator parameters")]
        public const string ShootParam = "Shoot";

        private bool allowFiring = true;
        private PlayerHealthSystem playerHealthSystem;
        private int playersCurrentHealth;
        private int difficultyIndex;
        private float currentFireRate;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            playerHealthSystem = FindObjectOfType<PlayerHealthSystem>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // The players current health is checked for the right firerate for the character
        // the lower the players health, the slower the firerate.
        private void Start()
        {
            if (playerHealthSystem == null)
            {
                Debug.LogError("The " + gameObject.name + " couldn't find a PlayerHealthSystem!");
            }

            if (transform.localPosition.x > 0)
            {
                spriteRenderer.flipX = true;

                if (projectileLaunchOffset.transform.localPosition.x > 0)
                {
                    Vector3 position = projectileLaunchOffset.transform.localPosition;
                    position.x *= -1;
                    projectileLaunchOffset.transform.localPosition = position;
                }
            }
            else if (transform.localPosition.x < 0)
            {
                spriteRenderer.flipX = false;

                if (projectileLaunchOffset.transform.localPosition.x < 0)
                {
                    Vector3 position = projectileLaunchOffset.transform.localPosition;
                    position.x *= -1;
                    projectileLaunchOffset.transform.localPosition = position;
                }
            }

            playersCurrentHealth = playerHealthSystem.PlayerCurrentHealth;

            if (playersCurrentHealth != 0)
            {
                difficultyIndex = playersCurrentHealth - 1;
            }

            currentFireRate = fireRates[difficultyIndex];
        }

        // If firing is allowed the character will instantiate a projectile that is launched from the
        // offset position that is set for the character.
        private void Update()
        {
            if (allowFiring)
            {
                animator.SetTrigger(ShootParam);
                Instantiate(enemyProjectilePrefab, projectileLaunchOffset.position, projectileLaunchOffset.transform.rotation);
                allowFiring = false;
                StartCoroutine(FireRate());
            }
        }

        // Enumerator to define a firerate to the enemy which is shooting projectiles
        IEnumerator FireRate()
        {
            if (!allowFiring)
            {
                yield return new WaitForSeconds(currentFireRate);
                allowFiring = true;
            }
        }
    }
}
