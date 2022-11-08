using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class FireProjectiles : MonoBehaviour
    {
        [SerializeField] private Transform projectileLaunchOffset;

        [SerializeField] private RangedEnemyProjectile enemyProjectilePrefab;

        [SerializeField] private float fireRate = 1f;

        private bool allowFiring = true;

        private void Update()
        {
            if (allowFiring)
            {
                Instantiate(enemyProjectilePrefab, projectileLaunchOffset.position, projectileLaunchOffset.transform.rotation);
                allowFiring = false;
                StartCoroutine(FireRate());
            }
        }

        IEnumerator FireRate()
        {
            if (!allowFiring)
            {
                yield return new WaitForSeconds(fireRate);
                allowFiring = true;
            }
        }
    }
}
