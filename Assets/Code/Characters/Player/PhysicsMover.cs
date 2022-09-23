using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
	public class PhysicsMover : MonoBehaviour, IMove
	{
		// The new keyword hides the inherited name rigidbody and tells the compiler
		// to use this variable instead.
		private new Rigidbody2D rigidbody;

		private Vector2 movement;

		public float Speed
		{
			get;
			// Private modifier means that we can set the value of Speed from this
			// object and nowhere else. It is private for the object created from
			// this class.
			private set;
		}

		private void Awake()
		{
			rigidbody = GetComponent<Rigidbody2D>();
			if (rigidbody == null)
			{
				Debug.LogError("The PhysicsMover is missing a reference to the Rigidbody2D " +
					"component!");
			}
		}

		public void Setup(float speed)
		{
			Speed = speed;
		}

		public void Jump(float height)
		{
			// F = m * a || F = Force, m = mass, a = acceleration
			float m = rigidbody.mass;

			// Solve the initial acceleration
			// a = v/t || v = speed, t = time

			// Solve the innitial speed (v) and the time (t) to reach the highest point of the jump
			// P = 1/2 * a * t^2 + v * t + P0 || P0 = initial position, P = jump height = h
			// a = g || g = gravity which is the only force which affects the jump after it has started
			// h = 1/2 * g * t^2 || We can calculate time from this
			// t = Sqrt(h / 0.5g)

			// Now we know how long it takes to reach the max. height.
			// Next we must solve the initial velocity. Since the graph of a jump is symmetrical,
			// the initial velocity equals the final velocity.
			// vInitial = vFinal

			// a = v/t || *t => v = a * t => v = gt

			// Now we can solve the acceleration
			// a = v / t

			// And force
			// F = m * a

			float g = Mathf.Abs(Physics2D.gravity.y);
			float t = Mathf.Sqrt(height / (0.5f * g));

			float F = m * (g * t);

			rigidbody.AddForce(new Vector2(0, F), ForceMode2D.Impulse);
		}

		private void FixedUpdate()
		{
			rigidbody.velocity = new Vector2(movement.x * Speed, rigidbody.velocity.y);
		}

		public void Move(Vector2 direction)
		{
			movement = direction;
		}
	}
}
