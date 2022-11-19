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
        [Range(0f, 100f)] public float SFXVolume = 100f;
        [Range(0f, 100f)] public float MusicVolume = 100f;

        [Header("Slider game objects")]
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider musicSlider;

        [Header("Music and SFX texts")]
        [SerializeField] private string musicTextFirstPart = "MUSIC: ";
        [SerializeField] private string sfxTextFirstPart = "SFX: ";
        [SerializeField] private TextMeshProUGUI musicText;
        [SerializeField] private TextMeshProUGUI sfxText;

        private void Awake()
        {
            SFXVolume = PlayerPrefs.GetFloat("SFXVol", 100);
            sfxSlider.value = SFXVolume;
            sfxText.text = sfxTextFirstPart + SFXVolume;

            MusicVolume = PlayerPrefs.GetFloat("MusicVol", 100);
            musicSlider.value = MusicVolume;
            musicText.text = musicTextFirstPart + MusicVolume;

            foreach (Sound s in sfx)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = SFXVolume;
                s.source.loop = s.loop;
            }

            foreach (Sound m in songs)
            {
                m.source = gameObject.AddComponent<AudioSource>();
                m.source.clip = m.clip;
                m.source.volume = MusicVolume;
                m.source.loop = m.loop;
            }
        }

        private void Update()
        {
            UpdateSFXVolume();
            UpdateMusicVolume();
        }
        
        // Function to update SFX volume
        private void UpdateSFXVolume()
        {
            SFXVolume = sfxSlider.value;
            sfxText.text = sfxTextFirstPart + SFXVolume;

            foreach (Sound s in sfx)
            {
                s.source.volume = SFXVolume;
            }
        }

        // Function to update music volume
        private void UpdateMusicVolume()
        {
            MusicVolume = musicSlider.value;
            musicText.text = musicTextFirstPart + MusicVolume;

            foreach (Sound s in songs)
            {
                s.source.volume = MusicVolume;
            }
        }

        // Function to play a sound with a set name
        public void PlaySfx(string name)
        {
            Sound s = Array.Find(sfx, sound => sound.name == name);
            if (s == null) return;

            s.source.Play();
        }

        // Function to play a song with a set name
        public void PlaySong(string name)
        {
            Sound m = Array.Find(songs, sound => sound.name == name);
            if (m == null) return;
            
            m.source.Play();
        }

        // Function to stop playing music
        public void StopPlayingSong(string name)
        {
            Sound m = Array.Find(songs, sound => sound.name == name);
            if (m == null) return;
            
            m.source.Stop();
        }

        // Function to save and change the SFX volume
        public void ChangeSFXOptions()
        {
            PlayerPrefs.SetFloat("SFXVol", SFXVolume);
            SFXVolume = sfxSlider.value;
        }

        // Function to save and change the music volume
        public void ChangeMusicOptions()
        {
            PlayerPrefs.SetFloat("MusicVol", MusicVolume);
            MusicVolume = musicSlider.value;
        }
    }
}
