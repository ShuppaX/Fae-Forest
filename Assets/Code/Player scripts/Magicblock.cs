using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class Magicblock : MonoBehaviour
    {
        [SerializeField] private float despawnTimer = 2.5f;

        // Start the coroutine for despawning in awake
        private void Awake()
        {
            StartCoroutine(DespawnTimer());
        }

        // Simple IEnumerator to despawn the object after a set amount of time
        IEnumerator DespawnTimer()
        {
            yield return new WaitForSeconds(despawnTimer);
            Destroy(gameObject);
        }
    }
}
