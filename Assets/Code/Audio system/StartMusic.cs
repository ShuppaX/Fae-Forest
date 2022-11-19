using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemixGame
{
    public class StartMusic : MonoBehaviour
    {
        [Header("Level song name")]
        [SerializeField] private string songName;

        private AudioManager audioManager;

        private void Awake()
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (audioManager == null)
            {
                Debug.LogError("An audio manager was not found!");
            }

            audioManager.PlaySong(songName);
        }
    }
}
