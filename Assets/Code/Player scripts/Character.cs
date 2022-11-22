using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

namespace RemixGame
{

    public class Character : MonoBehaviour
    {
        [Header("Ground checks")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer, magicblockLayer;

        [Header("Projectile offset and projectiles")]
        [SerializeField] private Transform projectileLaunchOffset;
        [SerializeField] private CompositeProjectile compositeProjectile;
        [SerializeField] private MagicProjectile magicProjectile;

        [Header("Player characters")]
        [SerializeField] private GameObject characterOne;
        [SerializeField] private GameObject characterTwo;

        [Header("Player variables")]
        [SerializeField] private float movementSpeed = 8f;
        [SerializeField] private float jumpingPower = 10f;
        [SerializeField] double jumpCd = 0.6;

        [Header("Tags")]
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private string enemyProjectileTag = "EnemyProjectile";
        [SerializeField] private string healthManagerTag = "HealthManager";

        [Header("Animator")]
        private Animator chara1Animation; 
        //private Animator chara2Animation;
        
        private static readonly int Speed = Animator.StringToHash("speed");
        private static readonly int IsJumping = Animator.StringToHash("isJumping");
        private static readonly int GroundCheck = Animator.StringToHash("GroundCheck");
        private static readonly int MagicBlockCheck = Animator.StringToHash("MagicBlockCheck");
        private static readonly int JumpSpeed = Animator.StringToHash("jumpSpeed");



        //Objects
        private Rigidbody2D rbOne;
        private Rigidbody2D rbTwo;
        private GameObject healthManager;
        
        //Variables
        private Vector2 currentScale;
        private Vector2 groundbox = new(0.6f, 0.1f);
        private float horizontal;
        private bool goingRight;
        private bool facingRight = true;
        private float sinceJump = 0f;


        public bool FacingRight
        {
            get { return facingRight; }
        }

        public Transform getPosition { get; }
        
        
        private void Awake()
        {
            rbOne = characterOne.GetComponent<Rigidbody2D>();
            rbTwo = characterTwo.GetComponent<Rigidbody2D>();
            currentScale = transform.localScale;
            healthManager = GameObject.FindWithTag(healthManagerTag);
            
            chara1Animation = GetComponent<Animator>();

            characterOne.SetActive(true);
            characterTwo.SetActive(false);
        }

        private void Update()
        {
            if (characterOne.activeSelf)
            {
                //Animations no idea if update or fixed better seems to have no difference
                //TODO shooting animation
                chara1Animation.SetFloat(Speed, Mathf.Abs(rbOne.velocity.x));
                chara1Animation.SetFloat(JumpSpeed, Mathf.Abs(rbOne.velocity.y));
                chara1Animation.SetBool(GroundCheck, IsGrounded());
                chara1Animation.SetBool(MagicBlockCheck, IsOnMagicblock());
            }
            else if (characterTwo.activeSelf)
            {
                //TODO art pls
                
            }
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (characterOne.activeSelf)
            {
                rbOne.velocity = new Vector2(horizontal * movementSpeed, rbOne.velocity.y);
                
            }
            else if (characterTwo.activeSelf)
            {
                rbTwo.velocity = new Vector2(horizontal * movementSpeed, rbTwo.velocity.y);
            }
            
            sinceJump += Time.deltaTime;
            
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
            if (characterOne.activeSelf)
            {
                if (context.performed && sinceJump > jumpCd && IsGrounded())
                {
                    sinceJump = 0;
                    rbOne.AddForce(new Vector2(rbOne.velocity.x, jumpingPower), ForceMode2D.Impulse);
                    chara1Animation.SetTrigger(IsJumping);
                }
                else if (context.performed && sinceJump > jumpCd && IsOnMagicblock())
                {
                    sinceJump = 0;
                    rbOne.AddForce(new Vector2(rbOne.velocity.x, jumpingPower), ForceMode2D.Impulse);
                    chara1Animation.SetTrigger(IsJumping);

                }
                
                //Lower jump from releasing the key mid flight
                if (context.canceled && rbOne.velocity.y > 0f)
                {
                    var velocity = rbOne.velocity;
                    velocity = new Vector2(velocity.x, velocity.y * 0.66f);
                    rbOne.velocity = velocity;
                }

               
            } 
            else if (characterTwo.activeSelf)
            {
                if (context.performed && sinceJump > jumpCd && IsGrounded())
                {
                    sinceJump = 0;
                    rbTwo.AddForce(new Vector2(rbTwo.velocity.x, jumpingPower), ForceMode2D.Impulse);
                }
                else if (context.performed && sinceJump > jumpCd && IsOnMagicblock())
                {
                    sinceJump = 0;
                    rbTwo.AddForce(new Vector2(rbTwo.velocity.x, jumpingPower), ForceMode2D.Impulse);
                }
                
                //Lower jump from releasing the key mid flight
                if (context.canceled && rbTwo.velocity.y > 0f)
                {
                    var velocity = rbTwo.velocity;
                    velocity = new Vector2(velocity.x, velocity.y * 0.66f);
                    rbTwo.velocity = velocity;
                }
            }
            
        }

        // Method to move the character
        public void Move(InputAction.CallbackContext context)
        {
            horizontal = context.ReadValue<Vector2>().x;
            CheckDirectionOfMovement();
            CheckWayOfFacing();
            Flip();
        }

        
        //NOTE TO SELF THIS IS THE BASIC PROJECTILE
        // Method to fire a projectile when the set button is pressed.
        public void FireComposite(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (characterOne.activeSelf)
                {
                    Instantiate(compositeProjectile, projectileLaunchOffset.position, projectileLaunchOffset.transform.rotation);
                } else if (characterTwo.activeSelf)
                {
                    Instantiate(magicProjectile, projectileLaunchOffset.position, projectileLaunchOffset.transform.rotation);
                }
            }
        }

        // Method to swap between characters
        public void Swap(InputAction.CallbackContext context)
        {
            if (characterOne.activeSelf)
            {
                characterTwo.transform.position = characterOne.transform.position;

                characterTwo.GetComponent<Character>().facingRight = characterOne.GetComponent<Character>().facingRight;
                characterTwo.GetComponent<Character>().FlipOnSwap();

                rbTwo.velocity = rbOne.velocity;

                characterOne.SetActive(false);
                characterTwo.SetActive(true);
            }
            else if (characterTwo.activeSelf)
            {
                characterOne.transform.position = characterTwo.transform.position;

                characterOne.GetComponent<Character>().facingRight = characterTwo.GetComponent<Character>().facingRight;
                characterOne.GetComponent<Character>().FlipOnSwap();

                rbOne.velocity = rbTwo.velocity;

                characterTwo.SetActive(false);
                characterOne.SetActive(true);
            }
        }

        // Method to check which way the character is moving
        private void CheckDirectionOfMovement()
        {
            if (horizontal > 0)
            {
                goingRight = true;
            }
            else if (horizontal < 0)
            {
                goingRight = false;
            }
        }

        // Method to check which way the character is already facing
        private void CheckWayOfFacing()
        {
            if (currentScale.x > 0)
            {
                facingRight = true;
            }
            else
            {
                facingRight = false;
            }
        }

        // Method to make the character face the other way
        private void Flip()
        {
            if (!goingRight && facingRight)
            {
                currentScale.x *= -1;
                transform.localScale = currentScale;
            }
            else if (goingRight && !facingRight)
            {
                currentScale.x *= -1;
                transform.localScale = currentScale;
            }
        }

        // Methos used to flip the character face the same way than the other character was facing
        private void FlipOnSwap()
        {
            if (facingRight && currentScale.x < 0)
            {
                currentScale.x *= -1;
                transform.localScale = currentScale;
            }
            else if (!facingRight && currentScale.x > 0)
            {
                currentScale.x *= -1;
                transform.localScale = currentScale;
            }
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
    }
}