using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RemixGame.Code
{

    public class PlayerMovement : MonoBehaviour
    {
        public Rigidbody2D rb;

        public Transform groundCheck;

        public LayerMask groundlayer;

        private float horizontal;

        private float speed = 8f;

        private float jumpingPower = 8f;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        }

        //Groundcheck using empty object under characters feet. Checks overlaps within a circle
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
            //reduced jump height with just a tap of the button
            if (context.canceled && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }

        public void Move(InputAction.CallbackContext context)
        {
            horizontal = context.ReadValue<Vector2>().x;
        }

    }
}