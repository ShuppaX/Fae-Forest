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
        [SerializeField] Transform target;
        [SerializeField] float aggrorange;
        [SerializeField] private Transform Castpoint;
        [SerializeField] private MinibossProjectile projectile;
        public float timeBetweenShots;
        private float nextShotTime;

        private EnemyAi _enemyAi;

        private float aggroTimeCap = 10;
        private float sinceAggro;
        private Rigidbody2D rb2d;
        public bool flip;

        private bool isFacingLeft;

        public bool IsFacingLeft
        {
            get { return isFacingLeft; }
        }

        private void Awake()
        {
            _enemyAi = GetComponent<EnemyAi>();
            rb2d = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {

            if (LineOfSight(aggrorange))
            {
                sinceAggro = 0;
                if (!gameObject.GetComponent<PlayerProjectileActions>().StopActions)
                {
                    ChasePlayer();
                    ShootPlayer();
                }
            }
            else
            {
                _enemyAi.Patrolmove();
            }
        }

        private void FixedUpdate()
        {
            sinceAggro += Time.deltaTime;
        }

        private void ShootPlayer()
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

        private void ChasePlayer()
        {
            Vector3 unitScale = transform.localScale;
            
            if (transform.position.x < target.position.x)
            {
                //player on right side
                rb2d.velocity = new Vector2(speed, 0);
                unitScale.x = Mathf.Abs(unitScale.x) * -1 * (flip ? -1 : 1);
                isFacingLeft = false;
                Debug.Log("Miniboss is facing right");
            }
            else
            {
                //Player on left side
                rb2d.velocity = new Vector2(speed, 0);
                unitScale.x = Mathf.Abs(unitScale.x) * (flip ? -1 : 1);
                isFacingLeft = true;
                Debug.Log("Miniboss is facing left");
            }
        }

      
        // Raycast function to spot the player 
        private bool LineOfSight(float distance)
        {
            bool val = false;
            float castDist = -distance;
            Vector2 endPos = Castpoint.position + Vector3.right * castDist ;

            if (isFacingLeft)
            {
                castDist = distance;
            }

            RaycastHit2D hit = Physics2D.Linecast(Castpoint.position, endPos, 1 << LayerMask.NameToLayer("Player"));

            if (hit.collider !=null)
            {
                if (hit.collider.gameObject.GetComponent<Character>())
                {
                    Debug.Log("Hit "+ hit.collider.gameObject.name);
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



