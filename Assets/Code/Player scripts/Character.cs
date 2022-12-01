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
        [SerializeField] double jumpCd = 0.3;

        [Header("Tags")]
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private string enemyProjectileTag = "EnemyProjectile";
        [SerializeField] private string healthManagerTag = "HealthManager";

        [Header("Animator parameters")]
        public const string ShootParam = "Shoot";
        public const string AnimStateParam = "AnimState";
        public const string JumpParam = "Jump";
        public const string GroundCheckParam = "GroundCheck";
        public const string MagicBlockCheckParam = "MagicBlockCheck";
        public const string AirSpeedParam = "AirSpeedY";

        //Objects
        private Rigidbody2D rb;

        private GameObject healthManager;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        //Variables
        private Vector2 groundbox = new(0.6f, 0.2f);
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

        private void Start()
        {
            CheckOtherCharactersWayOfFacing();

            if (!spriteRenderer.flipX)
            {
                if (projectileLaunchOffset.transform.localPosition.x < 0)
                {
                    FlipProjectileOffset();
                }
            }
            else if (spriteRenderer.flipX)
            {
                if (projectileLaunchOffset.transform.localPosition.x > 0)
                {
                    FlipProjectileOffset();
                }
            }
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
            //CheckWayOfFacing();

            if (rb.velocity.x < -3.5)
            {
                spriteRenderer.flipX = true;
                facingRight = false;
                if (projectileLaunchOffset.transform.localPosition.x > 0)
                {
                    FlipProjectileOffset();
                }
            }
            else if (rb.velocity.x > 3.5)
            {
                spriteRenderer.flipX = false;
                facingRight = true;
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

            // For jumping and falling
            animator.SetFloat(AirSpeedParam, rb.velocity.y);

            // Attack
            if (isAttacking)
            {
                animator.SetTrigger(ShootParam);
                isAttacking = false;
            }

            // Run
            if (Mathf.Abs(rb.velocity.x) > 0.01)
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
                isJumping = true;
            }
            
            if (context.performed && sinceJump > jumpCd && IsOnMagicblock())
            {
                sinceJump = 0;
                rb.AddForce(new Vector2(rb.velocity.x, jumpingPower), ForceMode2D.Impulse);
                isJumping = true;
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
                InstantiateProjectile();
            }
        }

        // Method to swap between characters
        public void Swap(InputAction.CallbackContext context)
        {
            otherCharacter.transform.position = transform.position;
            otherCharacter.GetComponent<Rigidbody2D>().velocity = rb.velocity;

            otherCharacter.GetComponent<Character>().CheckOtherCharactersWayOfFacing();
            otherCharacter.GetComponent<Character>().CheckProjecileOffsetPosition();

            gameObject.SetActive(false);
            otherCharacter.SetActive(true);
        }

        // Method used to face the current character the same way the previous character was facing
        public void CheckOtherCharactersWayOfFacing()
        {
            if (otherCharacter.GetComponent<Character>().FacingRight)
            {
                spriteRenderer.flipX = false;
                facingRight = true;
            }
            else if (!otherCharacter.GetComponent<Character>().FacingRight)
            {
                spriteRenderer.flipX = true;
                facingRight = false;
            }
        }

        public void CheckProjecileOffsetPosition()
        {
            if (!spriteRenderer.flipX)
            {
                if (projectileLaunchOffset.transform.localPosition.x < 0)
                {
                    FlipProjectileOffset();
                }
            }
            else if (spriteRenderer.flipX)
            {
                if (projectileLaunchOffset.transform.localPosition.x > 0)
                {
                    FlipProjectileOffset();
                }
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
                if (!collision.gameObject.GetComponent<PlayerProjectileActions>().DeathSequence)
                {
                    healthManager.GetComponent<PlayerHealthSystem>().ReduceHealth();
                }
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