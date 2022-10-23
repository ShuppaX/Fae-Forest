using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class TempEnemyDestroy : MonoBehaviour
    {
        // Simple OnCollision detection for objects with the tag "Projectile"
        // to destroy the gameobject.
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("PlayerProjectile"))
            {
                Destroy(gameObject);
            }
        }
    }
}
