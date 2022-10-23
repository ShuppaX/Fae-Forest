using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float Speed = 4f;

        private Rigidbody2D projectileRb;

        private void Awake()
        {
            projectileRb = gameObject.GetComponent<Rigidbody2D>();
        }

        // This is used for the movement of the projectile
        private void FixedUpdate()
        {
            projectileRb.velocity = transform.right * Speed;
        }

        // This is used to destroy the projectile if it collides with something
        // that it can collide with.
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Enemy projectile collided with something!");
            Destroy(gameObject);
        }
    }
}
