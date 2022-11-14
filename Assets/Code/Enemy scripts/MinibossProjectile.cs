using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class MinibossProjectile : MonoBehaviour
    {
        [SerializeField] private float Speed = 4f;

        [SerializeField] private GameObject miniboss;
        [SerializeField] private GameObject player1;

        private Rigidbody2D projectileRb;

        private bool mbFacingLeft;



        private void Start()
        {

            projectileRb = gameObject.GetComponent<Rigidbody2D>();
            Vector2 direction =
                new Vector2(
                    (FindObjectOfType<Character>().gameObject.transform.position.x - projectileRb.transform.position.x),
                    0);
            projectileRb.velocity = direction.normalized * Speed;
        }

        // This is used to destroy the projectile if it collides with something
        // that it can collide with.
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(gameObject);
        }
    }
}
