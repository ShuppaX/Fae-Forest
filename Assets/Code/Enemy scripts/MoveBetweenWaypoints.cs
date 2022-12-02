using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RemixGame
{
    public class MoveBetweenWaypoints : MonoBehaviour
    {
        [SerializeField] private float[] movementSpeeds = { 5f, 6f, 7f };
        [SerializeField] private float offsetToDestination = 0.0001f;
        [SerializeField] private Transform[] waypoints;

        private Transform destination;
        private Vector2 currentPosition;
        private Vector2 destinationPos;
        private Rigidbody2D rb;

        private PlayerHealthSystem playerHealthSystem;
        private int playersCurrentHealth;
        private int difficultyIndex;
        private float currentMovementSpeed;

        private bool deathSequence = false;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerHealthSystem = FindObjectOfType<PlayerHealthSystem>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            destination = waypoints[1];
            destinationPos = destination.position;
            transform.position = waypoints[0].position;

            if (playerHealthSystem == null)
            {
                Debug.LogError("The " + gameObject.name + " couldn't find a PlayerHealthSystem!");
            }

            // Check the players current health and adjust difficulty index according to it.
            CheckPlayersHealth();
            currentMovementSpeed = movementSpeeds[difficultyIndex];
        }

        private void Update()
        {
            UpdateSprite();

            // Check the players current health and adjust difficulty index according to it.
            CheckPlayersHealth();
            currentMovementSpeed = movementSpeeds[difficultyIndex];

            deathSequence = GetComponent<PlayerProjectileActions>().DeathSequence;
        }

        private void FixedUpdate()
        {
            if (!deathSequence)
            {
                Move(destinationPos, Time.fixedDeltaTime);
            }
        }

        // Method to move the enemy to the destination
        private void Move(Vector2 destination, float deltaTime)
        {
            rb.MovePosition(Vector2.MoveTowards(transform.position, destination, currentMovementSpeed * deltaTime));
            currentPosition = transform.position;

            if ((currentPosition - destinationPos).magnitude < offsetToDestination)
            {
                NextWaypoint();
            }
        }

        // Method to get the next waypoint (two waypoint systems only)
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

        // Method to update the sprites way of facing
        private void UpdateSprite()
        {
            if (transform.localPosition.x > destinationPos.x)
            {
                spriteRenderer.flipX = false;
            }
            else if (transform.localPosition.x < destinationPos.x)
            {
                spriteRenderer.flipX = true;
            }
        }

        // Method to check the players health and change the difficulty index according to it.
        private void CheckPlayersHealth()
        {
            playersCurrentHealth = playerHealthSystem.PlayerCurrentHealth;

            if (playersCurrentHealth != 0)
            {
                difficultyIndex = playersCurrentHealth - 1;
            }
        }
    }
}
