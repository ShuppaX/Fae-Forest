using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class PlayerProjectileActions : MonoBehaviour
    {
        [SerializeField] private string compositeTag = "CompositeProjectile";
        [SerializeField] private string magicTag = "MagicProjectile";

        private bool stopActions = false;

        public bool StopActions
        {
            get { return stopActions; }
        }

        // Simple OnCollision detection for objects with tags
        // to destroy the gameobject or freeze the object
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals(compositeTag))
            {
                Destroy(gameObject);
            } else if (collision.gameObject.tag.Equals(magicTag))
            {
                stopActions = true;
            }
        }
    }
}
