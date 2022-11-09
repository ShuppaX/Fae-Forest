using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace RemixGame
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] songs;
        public Sound[] sfx;

        //public static AudioManager instance;

        [Range(0f, 1f)] public float SFXVolume = 1f;
        [Range(0f, 1f)] public float MusicVolume = 1f;

        [SerializeField] private Slider sfxSlider, musicSlider;
        
        private void Awake()
        {
            SFXVolume = PlayerPrefs.GetFloat("SFXVol", 1);
            sfxSlider.value = SFXVolume;
            MusicVolume = PlayerPrefs.GetFloat("MusicVol", 1);
            musicSlider.value = MusicVolume;
            
            
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
            
            foreach (Sound s in sfx)
            {
                s.source.volume = SFXVolume;
            }
        }

        // Function to update music volume
        private void UpdateMusicVolume()
        {
            MusicVolume = musicSlider.value;
            
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
