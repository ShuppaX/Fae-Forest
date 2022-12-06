using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace RemixGame
{
    public class EnemyAiMiniboss : MonoBehaviour
    {

        //headers to improve editor readability
        [Header("Pathfinding")]
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

        [Header("Animator parameters")]
        public const string JumpParam = "Jump";
        public const string AirSpeedParam = "AirSpeedY";
        public const string ChasingParam = "Chasing";
        public const string GroundCheckParam = "GroundCheck";
        public const string GettingAwayParam = "GettingAway";

        //Other variables
        private Transform target;
        private Miniboss _miniboss;
        private Path path;
        private int currentWaypoint = 0;
        private bool ActionsStopped;
        private bool deathSequence;
        private bool isJumping;
        private bool isGettingAway;

        private Seeker seeker;
        private Rigidbody2D rb;
        private RaycastHit2D isGrounded;

        private Character playerCharacter;

        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerCharacter = FindObjectOfType<Character>();

            target = playerCharacter.transform;
        }

        public void Start()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            _miniboss = GetComponent<Miniboss>();

            InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
        }

        private void Update()
        {
            playerCharacter = FindObjectOfType<Character>();
            target = playerCharacter.transform;

            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            ActionsStopped = GetComponent<PlayerProjectileActions>().StopActions;
            deathSequence = GetComponent<PlayerProjectileActions>().DeathSequence;

            
            if (!ActionsStopped && !_miniboss.MinibossAggro && !deathSequence)
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

            if (deathSequence)
            {
                rb.velocity = Vector3.zero;
            }
        }

        private void UpdateAnimator()
        {
            isGettingAway = GetComponent<Miniboss>().IsGettingAway;

            // Change the characters spriterenderer flip to match the direction of movement
            // and if the character is getting away or not
            if (isGettingAway)
            {
                if (rb.velocity.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
                else if (rb.velocity.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
            }
            else if (!isGettingAway)
            {
                if (rb.velocity.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
                else if (rb.velocity.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
            }

            // Is the character grounded?
            animator.SetBool(GroundCheckParam, isGrounded);

            // Jump
            if (isJumping)
            {
                animator.SetTrigger(JumpParam);
                isJumping = false;
            }

            // Falling
            animator.SetFloat(AirSpeedParam, rb.velocity.y);

            // Chasing a target
            animator.SetBool(ChasingParam, TargetInDistance());

            // Getting away from target
            animator.SetBool(GettingAwayParam, isGettingAway);
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
                    isJumping = true;
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
        }

        private bool TargetInDistance()
        {
            if (deathSequence)
            {
                return false;
            }

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
    

