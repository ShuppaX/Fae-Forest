using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
	public interface IMove
	{
		/// <summary>
		/// Speed property defines the speed of the mover
		/// </summary>
		float Speed { get; }

		/// <summary>
		/// Initialized the object created from the class which implements this
		/// interface.
		/// </summary>
		/// <param name="speed">The speed of the object.</param>
		void Setup(float speed);

		/// <summary>
		/// Moves any object the implementing class is attached to.
		/// </summary>
		/// <param name="direction">The direction of the movement.</param>
		void Move(Vector2 direction);

		/// <summary>
		/// The object should jump when this method is called.
		/// </summary>
		/// <param name="height">The max. height of the jump.</param>
		void Jump(float height);
	}
}
