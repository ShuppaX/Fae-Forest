using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class TempEnemyDestroy : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.tag.Equals("Projectile"))
            {
                Destroy(col.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
