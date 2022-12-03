using UnityEngine;
using UnityEngine.Serialization;

namespace RemixGame
{
    public class Miniboss : MonoBehaviour
    {
        [SerializeField] float[] speeds = { 6f, 7f, 8f };
        [SerializeField] float minDistance;
         
        [FormerlySerializedAs("aggrorange")] [SerializeField] float aggroRange;
        [FormerlySerializedAs("castpoint")] [SerializeField] private Transform castPoint;
        [SerializeField] private Transform projectileSpawnOffset;
        [SerializeField] private MinibossProjectile projectile;
        
        public float timeBetweenShots;
        private float nextShotTime;
        private bool isFacingLeft;
        private bool ActionsStopped;
        private float SocialDistancing;
        
        public bool MinibossAggro; // tells enemyAI when to do take over
        private Transform target;
        private Rigidbody2D rb2d;

        private PlayerHealthSystem playerHealthSystem;
        private int playersCurrentHealth;
        private int difficultyIndex;
        private float currentMovementSpeed;

        private bool deathSequence;
        private bool isGettingAway;

        public bool IsGettingAway { get { return isGettingAway; } }

        private void Awake()
        {
            target = FindObjectOfType<Character>().gameObject.transform;
            rb2d = GetComponent<Rigidbody2D>();
            isFacingLeft = true;
            playerHealthSystem = FindObjectOfType<PlayerHealthSystem>();
        }

        private void Start()
        {
            if (playerHealthSystem == null)
            {
                Debug.LogError("The " + gameObject.name + " couldn't find a PlayerHealthSystem!");
            }

            // Check up on amount of players health, if players health is lower, the enemys movement
            // is slower.
            CheckPlayersHealth();
            currentMovementSpeed = speeds[difficultyIndex];
        }

        private void Update()
        {
            ActionsStopped = GetComponent<PlayerProjectileActions>().StopActions;
            SocialDistancing = Mathf.Abs(Vector2.Distance(transform.position, target.position));
            deathSequence = GetComponent<PlayerProjectileActions>().DeathSequence;

            // Same as above, but to force changes to the movementspeed if player loses health
            CheckPlayersHealth();
            currentMovementSpeed = speeds[difficultyIndex];
        }

        private void FixedUpdate()
        {
            if (LineOfSight(aggroRange) && !deathSequence)
            {
                MinibossAggro = true;

                if (!ActionsStopped)
                {
                    if (SocialDistancing < minDistance)
                    {
                        ShootPlayer();
                    }
                    else
                    {
                        ChasePlayer();
                    }
                }
            }
            else
            {
                isGettingAway = false;
                MinibossAggro = false;
                //Let enemy AI script handle
            }
        }
    
        private void ShootPlayer()
        {
            // shot timer + spawning
            if (Time.time > nextShotTime)
            {
                //Create projectile
                Instantiate(projectile, projectileSpawnOffset.position, Quaternion.identity);
                nextShotTime = Time.time + timeBetweenShots;
            }
            rb2d.velocity *= 0.5f;
            // keeping the player at set distance
            isGettingAway = true;
            transform.position = Vector2.MoveTowards(transform.position, target.position, -currentMovementSpeed * Time.deltaTime);
        }

        private void ChasePlayer()
        {
            isGettingAway = false;

            if (transform.position.x < target.position.x && isFacingLeft)
            {
                //player on right side
                isFacingLeft = false;
            }
            else if (!isFacingLeft)
            {
                //Player on left side
                isFacingLeft = true;
            }
        }

        // Raycast function to spot the player 
        private bool LineOfSight(float distance)
        {
            bool val = false;
            float castDist = distance;
            var position = castPoint.position;
            Vector2 endPos = position - Vector3.right * castDist ;
            Vector2 startPos = position + Vector3.right * castDist ;
            
            if (isFacingLeft)
            {
                castDist = -distance;
            }
           
            RaycastHit2D hit = Physics2D.Linecast(startPos, endPos, 1 << LayerMask.NameToLayer("Player"));

            if (hit.collider !=null)
            {
                if (hit.collider.gameObject.GetComponent<Character>())
                {
                    val = true;
                }
                else 
                {
                    val = false;
                }
                Debug.DrawLine(castPoint.position, hit.point, Color.yellow);
            }
            else
            {
                Debug.DrawLine(castPoint.position, endPos, Color.cyan);
            }
            return val;
        }

        // Method to change difficulty index (the index of which movement speed value to use)
        // according to the players health
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