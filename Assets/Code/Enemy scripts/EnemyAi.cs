using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UIElements;

namespace RemixGame
{
    public class EnemyAi : MonoBehaviour
    {
        //headers to improve editor readability
        [Header("Pathfinding")] 
        public GameObject chara1;
        public GameObject chara2;
        public float activateDistance = 50f;
        public float pathUpdateSeconds = 0.5f;

        [Header("Patrol")] 
        public float patrolAcceleration = 2f;
        public float patrolmaxSpeed = 5f;
        private Vector2 patrolmovement;
        private float patroltimeLeft;
        
        [Header("Physics")] 
        public float[] movementSpeeds = { 150f, 175f, 200f };
        public float nextWaypointDistance = 3f;
        public float jumpNodeHeightRequirement = 0.8f;
        public float jumpModifier = 0.3f;
        public float jumpCheckOffset = 0.1f;

        [Header("Custom Behavior")] 
        public bool followEnabled = true;
        public bool jumpEnabled = true;
        public bool directionLookEnabled = true;

        [Header("Health Manager tag")]
        [SerializeField] private string healthManagerTag = "HealthManager";

        [Header("Animator parameters")]
        public const string JumpParam = "Jump";
        public const string AirSpeedParam = "AirSpeedY";
        public const string ChaseParam = "Chasing";
        public const string GroundCheckParam = "GroundCheck";

        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;

        //Other variables
        private Transform target;
        private Path path;
        private int currentWaypoint = 0;
        private bool ActionsStopped;

        private GameObject healthManager;
        private int playersCurrentHealth;
        private int difficultyIndex;
        private float currentMovementSpeed;

        private bool isJumping = false;

        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private Seeker seeker;
        private Rigidbody2D rb;
        private RaycastHit2D isGrounded;

        private Vector2 groundbox = new(0.6f, 0.1f);

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            healthManager = GameObject.FindWithTag(healthManagerTag);
        }

        public void Start()
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            
            InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);

            if (healthManager == null)
            {
                Debug.LogError("The " + gameObject.name + " couldn't find an object with the tag " + healthManagerTag + "!");
            }

            playersCurrentHealth = healthManager.GetComponent<PlayerHealthSystem>().PlayerCurrentHealth;

            if (playersCurrentHealth != 0)
            {
                difficultyIndex = playersCurrentHealth - 1;
            }

            currentMovementSpeed = movementSpeeds[difficultyIndex];
        }

        private void Update()
        {
            UpdateAnimator();

            Debug.Log("Chasers rb velocity = " + rb.velocity);
        }

        private void FixedUpdate()
        {
            ActionsStopped = GetComponent<PlayerProjectileActions>().StopActions;

            if (chara1.activeSelf)
            {
                target = chara1.transform;
            }
            else
            {
                target = chara2.transform;
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

        private void UpdateAnimator()
        {
            if (rb.velocity.x > 0)
            {
                spriteRenderer.flipX = true;
                Debug.Log("Chasers spriterenderer flipped!");
            }
            else if (rb.velocity.x < 0)
            {
                spriteRenderer.flipX = false;
                Debug.Log("Chasers spriterenderer NOT flipped!");
            }

            // Is the character on the ground?
            animator.SetBool(GroundCheckParam, IsGrounded());

            // Jump
            if (isJumping)
            {
                animator.SetTrigger(JumpParam);
                isJumping = false;
            }

            // For jumping and falling
            animator.SetFloat(AirSpeedParam, rb.velocity.y);

            // Chase
            animator.SetBool(ChaseParam, TargetInDistance());
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
            Vector2 force = direction * (currentMovementSpeed * Time.deltaTime);

            // Jump
            if (jumpEnabled && isGrounded)
            {
                if (direction.y > jumpNodeHeightRequirement)
                {
                    rb.AddForce(Vector2.up * (currentMovementSpeed * jumpModifier));
                    isJumping = true;
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

        // Groundcheck using empty object under characters feet. Checks overlaps within a box
        private bool IsGrounded()
        {
            return Physics2D.OverlapBox(groundCheck.position, groundbox, 0f, groundLayer);
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
    

