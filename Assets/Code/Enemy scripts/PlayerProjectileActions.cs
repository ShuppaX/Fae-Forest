using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class PlayerProjectileActions : MonoBehaviour
    {
        [SerializeField] private string damageTag = "DamageProjectile";
        [SerializeField] private string magicTag = "MagicProjectile";
        [SerializeField] private float timeForStoppedActions = 2.5f;

        [Header("Animation parameters")]
        public const string DeathParam = "Death";

        private bool stopActions = false;
        private bool deathSequence = false;
        private bool stopActionsActive = false;

        private Animator animator;

        public bool StopActions
        {
            get { return stopActions; }
        }

        public bool DeathSequence
        {
            get { return deathSequence; }
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        // Simple OnCollision detection for objects with tags
        // to destroy the gameobject or freeze the object
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals(damageTag))
            {
                deathSequence = true;
                animator.SetTrigger(DeathParam);
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

        public void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}
