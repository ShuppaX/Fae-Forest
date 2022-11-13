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

        [SerializeField] private string playerTag = "Player";

        private bool allowFiring = true;

        private GameObject player;

        private int playersCurrentHealth;

        private int difficultyIndex;

        private void Awake()
        {
            player = GameObject.FindWithTag(playerTag);
        }

        // The players current health is checked for the right firerate for the character
        // the lower the players health, the slower the firerate.
        private void Start()
        {
            if (player == null)
            {
                Debug.LogError("The shooter couldn't find an object with the tag " + playerTag + "!");
            }

            playersCurrentHealth = player.GetComponent<PlayerHealthSystem>().PlayerCurrentHealth;

            if (playersCurrentHealth != 0)
            {
                difficultyIndex = playersCurrentHealth - 1;
            }
        }

        // If firing is allowed the character will instantiate a projectile that is launched from the
        // offset position that is set for the character.
        private void Update()
        {
            if (allowFiring)
            {
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
                yield return new WaitForSeconds(fireRates[difficultyIndex]);
                allowFiring = true;
            }
        }
    }
}
