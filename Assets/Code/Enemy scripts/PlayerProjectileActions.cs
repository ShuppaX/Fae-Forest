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

        [SerializeField] private float timeForStoppedActions = 2.5f;

        private bool stopActions = false;

        private bool stopActionsActive = false;

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
                if (!stopActions)
                {
                    stopActions = true;
                    Debug.Log("Actions are stopped now!");
                }
                
                if (!stopActionsActive)
                {
                    StartCoroutine(ContinueActions());
                }
                stopActionsActive = true;
            }
        }

        // IEnumerator to turn the actions back on after a set period of time.
        IEnumerator ContinueActions()
        {
            yield return new WaitForSeconds(timeForStoppedActions);
            stopActions = false;
            Debug.Log("Actions are active again!");
            stopActionsActive = false;
        }
    }
}
