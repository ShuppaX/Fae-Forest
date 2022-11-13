using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace RemixGame
{
    public class EnemyAi : MonoBehaviour
    {
        //headers to improve editor readability
        [Header("Pathfinding")] 
        public GameObject chara1;
        public Transform character1coord;
        public GameObject chara2;
        public Transform character2coord;
        public Transform target;
        public float activateDistance = 50f;
        public float pathUpdateSeconds = 0.5f;

        [Header("Patrol")] 
        public float patrolAcceleration = 2f;
        public float patrolmaxSpeed = 5f;
        private Vector2 patrolmovement;
        private float patroltimeLeft;
        
        [Header("Physics")] 
        public float speed = 200f;
        public float nextWaypointDistance = 3f;
        public float jumpNodeHeightRequirement = 0.8f;
        public float jumpModifier = 0.3f;
        public float jumpCheckOffset = 0.1f;

        [Header("Custom Behavior")] 
        public bool followEnabled = true;
        public bool jumpEnabled = true;
        public bool directionLookEnabled = true;
        
        //Other variables
        private Path path;
        private int currentWaypoint = 0;
        private bool ActionsStopped;

        Seeker seeker;
        Rigidbody2D rb;
        RaycastHit2D isGrounded;
        
        
        public void Start()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            
            InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
        }

        private void FixedUpdate()
        {
            ActionsStopped = GetComponent<PlayerProjectileActions>().StopActions;

            if (chara1.activeSelf)
            {
                target = character1coord;
            }
            else
            {
                target = character2coord;
            }

            
            if (!ActionsStopped)
            {
                if (TargetInDistance() && followEnabled)
                {
                    PathFollow();
                }
                else
                {
                    Patrolmove();
                    rb.AddForce(patrolmovement * patrolmaxSpeed);
                }  
            }
        }

        private void UpdatePath()
        {
            if (followEnabled && TargetInDistance() && seeker.IsDone())
            {
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
        }

        public void PathFollow()
        {
            if (path == null)
            {
                return;
            }

            // Reached end of path
            if (currentWaypoint >= path.vectorPath.Count)
            {
                return;
            }

            // See if colliding with anything
            Vector3 startOffset = transform.position -
                                  new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
            isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);

            // Direction Calculation
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * (speed * Time.deltaTime);

            // Jump
            if (jumpEnabled && isGrounded)
            {
                if (direction.y > jumpNodeHeightRequirement)
                {
                    rb.AddForce(Vector2.up * (speed * jumpModifier));
                }
            }

            // Movement
            rb.AddForce(force);

            // Next Waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            // Direction Graphics Handling prob very bad
            if (directionLookEnabled)
            {
                if (rb.velocity.x > 0.05f)
                {
                    transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y,
                        transform.localScale.z);
                }
                else if (rb.velocity.x < -0.05f)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y,
                        transform.localScale.z);
                }
            }
        }

        private bool TargetInDistance()
        {
            return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }

        /// <summary>
        /// Patrol methods
        /// </summary>
        private void Patrolmove()
        {
            patroltimeLeft -= Time.deltaTime;
            if (patroltimeLeft <= 0)
            {
                patrolmovement = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                patroltimeLeft += patrolAcceleration;
            }
        }
    }
}
    

