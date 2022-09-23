using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RemixGame
{
	public class Character: MonoBehaviour
	{
		[SerializeField] private float speed = 1;

		[SerializeField] private float jumpHeight = 1;

		[SerializeField] private Sensor groundSensor;

		[SerializeField] private float weightLimit = 1;

		// Member variable. It is owned by the object itself.
		// Can be used from any method in this class.
		// TODO: Decide wether to keep the input or not
		private Vector2 input;

		private IMove mover;

		private void Awake()
		{
			mover = GetComponent<IMove>();
			if (mover == null)
			{
				Debug.LogError("Can't find a component which implements the IMove interface!");
			}

			if (groundSensor == null)
			{
				Debug.LogError("Ground sensor is missing! Grounding checks won't work!");
			}
			

			// Player's inventory

		}

		// Start is called before the first frame update
		private void Start()
		{
			mover.Setup(speed);
		}

		public void Move(InputAction.CallbackContext context)
		{
			// Read the user input using InputSystem's callback context
			input = context.ReadValue<Vector2>();
			mover.Move(input);
		}

		public void Jump(InputAction.CallbackContext context)
		{
			// With and, both sides of the statement have to be true for the whole
			// statement to be true.
			if (groundSensor.IsActive && context.phase == InputActionPhase.Performed)
			{
				mover.Jump(jumpHeight);
			}
		}
	}
}
