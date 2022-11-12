using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace RemixGame
{
    public class Miniboss : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float minDistance;
         
        [SerializeField] float aggrorange;
        [SerializeField] private Transform Castpoint;
        [SerializeField] private MinibossProjectile projectile;
        
        public float timeBetweenShots;
        private float nextShotTime;
        private bool isFacingLeft;
        private bool ActionsStopped;
        private float SocialDistancing;
        
        public bool MinibossAggro; // tells enemyAI when to do things
        private Transform target;
        private Rigidbody2D rb2d;

        

        private void Awake()
        {
            target = FindObjectOfType<Character>().gameObject.transform;
            rb2d = GetComponent<Rigidbody2D>();
            isFacingLeft = true;
        }

        private void Update()
        {
            
            ActionsStopped = GetComponent<PlayerProjectileActions>().StopActions;
            SocialDistancing = Mathf.Abs(Vector2.Distance(transform.position, target.position));
        }

        private void FixedUpdate()
        {
            if (LineOfSight(aggrorange))
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
                Instantiate(projectile, transform.position, Quaternion.identity);
                nextShotTime = Time.time + timeBetweenShots;
            }

            //keeping the player at set distance TODO: FIX
            transform.position = Vector2.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);

                
        }

            
        

        private void ChasePlayer()
        {
            Vector3 unitScale = transform.localScale;
            
            if (transform.position.x < target.position.x && isFacingLeft)
            {
                //player on right side
                isFacingLeft = false;
              //  GetComponent<SpriteRenderer>().flipX = true;
                
                
                Debug.Log("Miniboss is facing right");
            }
            else if (!isFacingLeft)
            {
                //Player on left side
                isFacingLeft = true;
                rb2d.velocity = new Vector2(speed, 0);
               // GetComponent<SpriteRenderer>().flipX = false;

                
                Debug.Log("Miniboss is facing left");
            }
        }

      
        // Raycast function to spot the player 
        private bool LineOfSight(float distance)
        {
            bool val = false;
            float castDist = distance;
            var position = Castpoint.position;
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
                    //Debug.Log("Hit "+ hit.collider.gameObject.name);
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
                Debug.DrawLine(Castpoint.position, endPos, Color.cyan);

            }

            return val;
        }

    }
    }



