using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RemixGame
{

    public class Character : MonoBehaviour
    {
        [Header("Ground checks")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer, magicblockLayer;

        [Header("Projectile offset and projectiles")]
        [SerializeField] private Transform projectileLaunchOffset;
        [SerializeField] private GameObject projectile;

        [Header("Player characters")]
        [SerializeField] private GameObject otherCharacter;

        [Header("Player variables")]
        [SerializeField] private float movementSpeed = 8f;
        [SerializeField] private float jumpingPower = 10f;
        [SerializeField] double jumpCd = 0.6;

        [Header("Tags")]
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private string enemyProjectileTag = "EnemyProjectile";
        [SerializeField] private string healthManagerTag = "HealthManager";

        [Header("Animator parameters")]
        public const string AttackParam = "Attack";
        public const string AnimStateParam = "AnimState";
        public const string JumpParam = "Jump";
        public const string GroundCheckParam = "GroundCheck";
        public const string MagicBlockCheckParam = "MagicBlockCheck";

        //Objects
        private Rigidbody2D rb;

        private GameObject healthManager;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        //Variables
        private Vector2 groundbox = new(0.6f, 0.1f);
        private float horizontal;
        private bool facingRight = true;
        private float sinceJump = 0f;
        private bool isJumping = false;
        private bool isAttacking = false;

        public bool FacingRight
        {
            get { return facingRight; }
        }

        public Transform getPosition { get; }
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            healthManager = GameObject.FindWithTag(healthManagerTag);

            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            UpdateAnimator();
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            rb.velocity = new Vector2(horizontal * movementSpeed, rb.velocity.y);
            
            sinceJump += Time.deltaTime;
        }

        private void UpdateAnimator()
        {
            if (rb.velocity.x < -Mathf.Epsilon)
            {
                spriteRenderer.flipX = true;
                CheckWayOfFacing();
                if (projectileLaunchOffset.transform.localPosition.x > 0)
                {
                    FlipProjectileOffset();
                }
            }
            else if (rb.velocity.x > Mathf.Epsilon)
            {
                spriteRenderer.flipX = false;
                CheckWayOfFacing();
                if (projectileLaunchOffset.transform.localPosition.x < 0)
                {
                    FlipProjectileOffset();
                }
            }

            // Is the character on ground or a magicblock?
            animator.SetBool(GroundCheckParam, IsGrounded());
            animator.SetBool(MagicBlockCheckParam, IsOnMagicblock());

            // Jump
            if (isJumping)
            {
                animator.SetTrigger(JumpParam);
                isJumping = false;
            }

            // Attack
            if (isAttacking)
            {
                animator.SetTrigger(AttackParam);
                isAttacking = false;
            }

            // Run
            if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon)
            {
                animator.SetInteger(AnimStateParam, 1);
            }
            else // Idle
            {
                animator.SetInteger(AnimStateParam, 0);
            }
        }
        
        // Groundcheck using empty object under characters feet. Checks overlaps within a box
        private bool IsGrounded()
        {
            return Physics2D.OverlapBox(groundCheck.position, groundbox, 0f, groundLayer);
        }

        // Groundcheck to magicblocks, works the same as above
        private bool IsOnMagicblock()
        {
            return Physics2D.OverlapBox(groundCheck.position, groundbox, 0f, magicblockLayer);
        }

        // jump action with rigidbody velocity applying force directly
        public void Jump(InputAction.CallbackContext context)
        {
            if (context.performed && sinceJump > jumpCd && IsGrounded())
            {
                sinceJump = 0;
                rb.AddForce(new Vector2(rb.velocity.x, jumpingPower), ForceMode2D.Impulse);
            }
            else if (context.performed && sinceJump > jumpCd && IsOnMagicblock())
            {
                sinceJump = 0;
                rb.AddForce(new Vector2(rb.velocity.x, jumpingPower), ForceMode2D.Force);
            }

            if (context.canceled && rb.velocity.y > 0f)
            {
                var velocity = rb.velocity;
                velocity = new Vector2(velocity.x, velocity.y * 0.66f);
                rb.velocity = velocity;
            }
        }

        // Method to move the character
        public void Move(InputAction.CallbackContext context)
        {
            horizontal = context.ReadValue<Vector2>().x;
        }

        // Method to fire a projectile when the set button is pressed.
        public void Fire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isAttacking = true;
            }
        }

        // Method to swap between characters
        public void Swap(InputAction.CallbackContext context)
        {
            otherCharacter.transform.position = transform.position;
            otherCharacter.GetComponent<Character>().facingRight = facingRight;
            //otherCharacter.GetComponent<Character>().FlipOnSwap();

            otherCharacter.GetComponent<Rigidbody2D>().velocity = rb.velocity;

            gameObject.SetActive(false);
            otherCharacter.SetActive(true);
        }

        // Method to check which way the character is already facing
        private void CheckWayOfFacing()
        {
            if (!spriteRenderer.flipX)
            {
                facingRight = true;
            }
            else
            {
                facingRight = false;
            }
        }

        // Methos used to flip the character face the same way than the other character was facing
        private void FlipOnSwap()
        {
            if (facingRight && !spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
            }
            else if (!facingRight && spriteRenderer.flipX)
            {
                spriteRenderer.flipX = false;
            }
        }

        private void FlipProjectileOffset()
        {
            Vector3 position = projectileLaunchOffset.transform.localPosition;
            position.x *= -1;
            projectileLaunchOffset.transform.localPosition = position;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(enemyTag) || collision.gameObject.CompareTag(enemyProjectileTag))
            {
                //TODO: Trigger possible invincibility?

                healthManager.GetComponent<PlayerHealthSystem>().ReduceHealth();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(groundCheck.position,groundbox);
        }

        public void InstantiateProjectile()
        {
            Instantiate(projectile, projectileLaunchOffset.position, projectileLaunchOffset.transform.rotation);
        }
    }
}