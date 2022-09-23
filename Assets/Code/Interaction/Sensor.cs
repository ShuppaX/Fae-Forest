using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
	public class Sensor : MonoBehaviour
	{
		private int collisionCount = 0;

		public bool IsActive
		{
			get { return collisionCount > 0; }
		}

		// Protected visibility modifier means that inherited types can access
		// this method (private would be unaccessible).
		// Virtual keyword means that the inherited type can override this
		// method's implementation.
		protected virtual void OnTriggerEnter2D(Collider2D collision)
		{
			collisionCount++;
		}

		protected virtual void OnTriggerExit2D(Collider2D collision)
		{
			collisionCount--;
		}
	}
}
