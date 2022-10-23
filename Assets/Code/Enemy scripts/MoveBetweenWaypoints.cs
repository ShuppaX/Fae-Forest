using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RemixGame
{
    public class MoveBetweenWaypoints : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;

        [SerializeField] private float offsetToDestination = 0.0001f;

        [SerializeField] private Transform[] waypoints;

        private Transform destination;

        private Vector2 currentPosition;

        private Vector2 destinationPos;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            destination = waypoints[1];
            destinationPos = destination.position;
            transform.position = waypoints[0].position;
        }

        private void FixedUpdate()
        {
            Move(destinationPos, Time.fixedDeltaTime);
        }

        private void Move(Vector2 destination, float deltaTime)
        {
            rb.MovePosition(Vector2.MoveTowards(transform.position, destination, movementSpeed * deltaTime));
            currentPosition = transform.position;

            if ((currentPosition - destinationPos).magnitude < offsetToDestination)
            {
                NextWaypoint();
            }
        }

        private void NextWaypoint()
        {
            if (destination == null)
            {
                Debug.LogError("The enemy with the name " + gameObject.name + " doesn't have a destination waypoint!");
                return;
            }

            if (destination == waypoints[1])
            {
                destination = waypoints[0];
                destinationPos = destination.position;
                return;
            }

            if (destination == waypoints[0])
            {
                destination = waypoints[1];
                destinationPos = destination.position;
                return;
            }
        }
    }
}
