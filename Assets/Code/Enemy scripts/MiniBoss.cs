using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RemixGame
{
    public class MiniBoss : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float minDistance;
        [SerializeField] Transform target;
        [SerializeField] float aggrorange;
        [SerializeField] private Transform Castpoint;
        

        public GameObject projectile;
        public float timeBetweenShots;
        private float nextShotTime;
        private bool isAgro;
        private bool isSearching;
        private Rigidbody2D rb2d;

        private bool isFacingLeft;

        // Start is called before the first frame update
        void Start()
        {
            rb2d = GetComponent<Rigidbody2D>();
            
        }

        // Update is called once per frame
        void Update()
        {

            if (LineOfSight(aggrorange))
            {
                ChasePlayer();
                ShootPlayer();
                isAgro = true;
                //TODO: Shoot
            }
            else
            {
                if (isAgro)
                {
                    if (!isSearching)
                    {
                        isSearching = true;
                        Invoke("StopChasingPlayer",5);
                    }
                }
            }

            if (isAgro)
            {
                ShootPlayer();
                ChasePlayer();
            }
            
            
            
            
           
        
        }

        void ShootPlayer()
        {
            if(Time.time > nextShotTime)
            {
                Instantiate(projectile, transform.position, Quaternion.identity);
                nextShotTime = Time.time + timeBetweenShots;
            }


            if(Vector2.Distance(transform.position, target.position) < minDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);
            }
            
        }

        void ChasePlayer()
        {
            if (transform.position.x < target.position.x)
            {
                //player on right side
                rb2d.velocity = new Vector2(speed, 0);
                transform.localScale = new Vector2(1,1);
                isFacingLeft = false;
            }
            else
            {
                //Player on left side
                rb2d.velocity = new Vector2(speed, 0);
                transform.localScale = new Vector2(-1,1);
                isFacingLeft = true;
            }
        }

        void StopChasingPlayer()
        {
            isSearching = false;
            isAgro = false;
            rb2d.velocity = new Vector2(0, 0);
            
        }

        bool LineOfSight(float distance)
        {
            bool val = false;
            float castDist = -distance;
            Vector2 endPos = Castpoint.position + Vector3.right * castDist ;

            if (isFacingLeft)
            {
                castDist = -distance;
            }

            RaycastHit2D hit = Physics2D.Linecast(Castpoint.position, endPos, 1 << LayerMask.NameToLayer("Ground"));

            if (hit.collider !=null)
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    val = true;
                }
                else 
                {
                    val = false;
                }
                Debug.DrawLine(Castpoint.position, hit.point, Color.yellow);


            }
            else
            {
                Debug.DrawLine(Castpoint.position, endPos, Color.blue);

            }

            return val;
        }
    }
}
