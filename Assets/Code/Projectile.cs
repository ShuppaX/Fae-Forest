using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float Speed = 4.5f;

        private Rigidbody2D projectileRb;

        private void Awake()
        {
            projectileRb = gameObject.GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            projectileRb.velocity = -transform.right * (Speed * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(gameObject);
        }
    }
}
