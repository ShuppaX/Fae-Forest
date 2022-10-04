using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RemixGame.Code
{

    public class Character : MonoBehaviour
    {
        [SerializeField] private float speed = 8f;

        [SerializeField] private float jumpingPower = 8f;

        [SerializeField] private Transform groundCheck;

        [SerializeField] private LayerMask groundlayer;

        [SerializeField] private Transform projectileLaunchOffset;

        [SerializeField] private Projectile projectilePrefab;

        [SerializeField] private float fireRate = 0.5f;

        private Rigidbody2D rb;

        private float horizontal;

        private bool allowFiring = true;

        private bool buttonPressed = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        }

        // Groundcheck using empty object under characters feet. Checks overlaps within a circle
        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundlayer);
        }

        // jump action with rigidbody velocity applying force directly
        public void Jump(InputAction.CallbackContext context)
        {
            if (context.performed && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }

            // reduced jump height with just a tap of the button
            if (context.canceled && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }

        public void Move(InputAction.CallbackContext context)
        {
            horizontal = context.ReadValue<Vector2>().x;
        }

        // Method to fire projectiles when the set button is pressed.
        public void Fire(InputAction.CallbackContext context)
        {
            if (!allowFiring && !buttonPressed)
            {
                buttonPressed = true;
                StartCoroutine(Firerate());
            }

            if (allowFiring)
            {
                allowFiring = false;
                Instantiate(projectilePrefab, projectileLaunchOffset.position, projectileLaunchOffset.transform.rotation);
                buttonPressed = false;
            }
        }

        // IEnumerator used to have a firerate in seconds between every shot.
        IEnumerator Firerate()
        {
            if (!allowFiring)
            {
                yield return new WaitForSeconds(fireRate);
                allowFiring = true;
            }
        }
    }
}