using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

namespace RemixGame
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] songs;
        public Sound[] sfx;

        //public static AudioManager instance;

        [Header("Variables")]
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 1f;
        [SerializeField] private float volumeDisplayMultiplier = 100f;

        [Header("Slider game objects")]
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider musicSlider;

        [Header("Music and SFX texts")]
        [SerializeField] private string musicTextFirstPart = "MUSIC: ";
        [SerializeField] private string sfxTextFirstPart = "SFX: ";
        [SerializeField] private TextMeshProUGUI musicText;
        [SerializeField] private TextMeshProUGUI sfxText;

        private bool ignoreSliders = false;
        private bool ignoreTexts = false;

        private void Awake()
        {
            if (sfxSlider == null && musicSlider == null)
            {
                ignoreSliders = true;
            }

            if (musicText == null && sfxText == null)
            {
                ignoreTexts = true;
            }

            sfxVolume = PlayerPrefs.GetFloat("SFXVol", 100);
            musicVolume = PlayerPrefs.GetFloat("MusicVol", 100);

            foreach (Sound s in sfx)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = sfxVolume;
                s.source.loop = s.loop;
            }

            foreach (Sound m in songs)
            {
                m.source = gameObject.AddComponent<AudioSource>();
                m.source.clip = m.clip;
                m.source.volume = musicVolume;
                m.source.loop = m.loop;
            }
        }

        private void Start()
        {
            sfxSlider.SetValueWithoutNotify(sfxVolume);
            musicSlider.SetValueWithoutNotify(musicVolume);
        }

        private void Update()
        {
            if (!ignoreSliders && !ignoreTexts)
            {
                UpdateSFXVolume();
                UpdateMusicVolume();
            }
        }

        // Method to update SFX volume
        private void UpdateSFXVolume()
        {
            sfxVolume = sfxSlider.value;
            sfxText.text = sfxTextFirstPart + Mathf.Round(sfxVolume * volumeDisplayMultiplier);

            foreach (Sound s in sfx)
            {
                s.source.volume = sfxVolume;
            }
        }

        // Method to update music volume
        private void UpdateMusicVolume()
        {
            musicVolume = musicSlider.value;
            musicText.text = musicTextFirstPart + Mathf.Round(musicVolume * volumeDisplayMultiplier);

            foreach (Sound s in songs)
            {
                s.source.volume = musicVolume;
            }
        }

        // Method to play a sound with a set name
        public void PlaySfx(string name)
        {
            Sound s = Array.Find(sfx, sound => sound.name == name);
            if (s == null) return;

            s.source.Play();
        }

        // Method to stop playing a sound with a set name
        public void StopSfx(string name)
        {
            Sound s = Array.Find(sfx, sound => sound.name == name);
            if (s == null) return;

            s.source.Stop();
        }

        // Method to play a song with a set name
        public void PlaySong(string name)
        {
            Sound m = Array.Find(songs, sound => sound.name == name);
            if (m == null) return;
            
            m.source.Play();
        }

        // Method to stop playing music
        public void StopSong(string name)
        {
            Sound m = Array.Find(songs, sound => sound.name == name);
            if (m == null) return;
            
            m.source.Stop();
        }

        // Method to save and change the SFX volume
        public void ChangeSFXOptions()
        {
            sfxVolume = sfxSlider.value;
            PlayerPrefs.SetFloat("SFXVol", sfxVolume);
        }

        // Method to save and change the music volume
        public void ChangeMusicOptions()
        {
            musicVolume = musicSlider.value;
            PlayerPrefs.SetFloat("MusicVol", musicVolume);
        }

        // Method to update the sliders
        private void UpdateSliders()
        {
            if (!ignoreSliders)
            {
                sfxSlider.value = sfxVolume;
                musicSlider.value = musicVolume;
            }
        }

        // Method to update the texts that display the volume values
        private void UpdateTexts()
        {
            if (!ignoreTexts)
            {
                sfxText.text = sfxTextFirstPart + Mathf.Round(sfxVolume * volumeDisplayMultiplier);
                musicText.text = musicTextFirstPart + Mathf.Round(musicVolume * volumeDisplayMultiplier);
            }
        }
    }
}
