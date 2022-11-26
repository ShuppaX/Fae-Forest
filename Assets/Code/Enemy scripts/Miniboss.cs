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

        [SerializeField] private string healthManagerTag = "HealthManager";
        
        public float timeBetweenShots;
        private float nextShotTime;
        private bool isFacingLeft;
        private bool ActionsStopped;
        private float SocialDistancing;
        
        public bool MinibossAggro; // tells enemyAI when to do take over
        private Transform target;
        private Rigidbody2D rb2d;

        private GameObject healthManager;
        private int playersCurrentHealth;
        private int difficultyIndex;
        private float currentMovementSpeed;

        private void Awake()
        {
            target = FindObjectOfType<Character>().gameObject.transform;
            rb2d = GetComponent<Rigidbody2D>();
            isFacingLeft = true;
            healthManager = GameObject.FindWithTag(healthManagerTag);
        }

        private void Start()
        {
            if (healthManager == null)
            {
                Debug.LogError("The " + gameObject.name + " couldn't find an object with the tag " + healthManagerTag + "!");
            }

            playersCurrentHealth = healthManager.GetComponent<PlayerHealthSystem>().PlayerCurrentHealth;

            if (playersCurrentHealth != 0)
            {
                difficultyIndex = playersCurrentHealth - 1;
            }

            currentMovementSpeed = speeds[difficultyIndex];
        }

        private void Update()
        {
            ActionsStopped = GetComponent<PlayerProjectileActions>().StopActions;
            SocialDistancing = Mathf.Abs(Vector2.Distance(transform.position, target.position));
        }

        private void FixedUpdate()
        {
            if (LineOfSight(aggroRange))
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
                MinibossAggro = false;
                //Let enemy AI script handle
            }
        }
    
        private void ShootPlayer()
        {
            //shot timer + spawning
            if (Time.time > nextShotTime)
            {
                //Create projectile
                Instantiate(projectile, projectileSpawnOffset.position, Quaternion.identity);
                nextShotTime = Time.time + timeBetweenShots;
            }

            rb2d.velocity *= 0.5f;
            //keeping the player at set distance
            transform.position = Vector2.MoveTowards(transform.position, target.position, -currentMovementSpeed * Time.deltaTime);
        }

        private void ChasePlayer()
        {
            Vector3 unitScale = transform.localScale;
            
            if (transform.position.x < target.position.x && isFacingLeft)
            {
                //player on right side
                isFacingLeft = false;
                //GetComponent<SpriteRenderer>().flipX = true;
                
                
                Debug.Log("Miniboss is facing right");
            }
            else if (!isFacingLeft)
            {
                //Player on left side
                isFacingLeft = true;
                rb2d.velocity = new Vector2(currentMovementSpeed, 0);
                //GetComponent<SpriteRenderer>().flipX = false;
                Debug.Log("Miniboss is facing left");
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
    }
}



