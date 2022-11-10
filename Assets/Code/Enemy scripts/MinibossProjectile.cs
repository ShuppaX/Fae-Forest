using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class MinibossProjectile : MonoBehaviour
    {
        [SerializeField] private float Speed = 4f;

        [SerializeField] private GameObject miniboss;

        private Rigidbody2D projectileRb;

        private bool mbFacingLeft;

       

        private void Awake()
        {
            projectileRb = gameObject.GetComponent<Rigidbody2D>();

            if (miniboss.GetComponent<Miniboss>().IsFacingLeft)
            {
                mbFacingLeft = true;
            } else if (!miniboss.GetComponent<Miniboss>().IsFacingLeft)
            {
                mbFacingLeft = false;
            }
        }

        // This is used for the movement of the projectile and the direction is taken from
        // the initial position of the projectile spawn
        private void FixedUpdate()
        {
            if (!mbFacingLeft)
            {
                projectileRb.velocity = transform.right * Speed;
                Debug.Log(projectileRb.velocity);
            }

            if (mbFacingLeft)
            {
                projectileRb.velocity = -transform.right * Speed;
                Debug.Log(projectileRb.velocity);

            }
        }

        // This is used to destroy the projectile if it collides with something
        // that it can collide with.
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(gameObject);
        }
    }
}
