using Com.IsartDigital.F2P.UI.Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class MusicManager : MonoBehaviour
    {
       
        private AudioSource musicSource;

        public AudioClip menuMusic;
        public AudioClip gameLoopMusic;

        private void Awake()
        {
            MusicSwitch.SwitchTogame += SetUpGame;
            MusicSwitch.SwitchToMenu += SetUpMenu;

        }

        void Start()
        {
            musicSource = GetComponent<AudioSource>();
            SettingsOverlay.OnMusicVolumeChanged += SetUpVolume;
            if (SettingsData.ActualSettingsData != null)  musicSource.volume = SettingsData.ActualSettingsData.actualVolumeMusic; 

        }

        private void OnDestroy()
        {
            SettingsOverlay.OnMusicVolumeChanged -= SetUpVolume;
        }

        public void SetUpVolume(float pVolume, VolumeType pType)
        {
            if (pType == VolumeType.General)
                musicSource.volume = pVolume;
        }

        public void SetUpMenu()
        {
            if (musicSource.clip == menuMusic)
                return;
            musicSource.clip = menuMusic;
            musicSource.Play();
        }

        public void SetUpGame()
        {
            if (musicSource.clip == gameLoopMusic)
                return;
            musicSource.clip = gameLoopMusic;
            musicSource.Play();
        }

    }
}
