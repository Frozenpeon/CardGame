using Com.IsartDigital.F2P.Manager;
using Com.IsartDigital.F2P.UI.Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class TestforSD : MonoBehaviour
    {
        public AudioClip soundToPlay1;
        public AudioClip soundToPlay2;
        public AudioClip soundToPlay3;
        public AudioClip soundToPlay4;
        public AudioClip soundToPlay5;
        public AudioClip soundToPlay6;
        public AudioClip soundToPlay7;
        public AudioClip soundToPlay8;
        public AudioClip soundToPlay9;
        public AudioClip soundToPlay10;
        public AudioClip soundToPlay11;

        private AudioSource audioSource;

        void Start()
        {            
            audioSource = GetComponent<AudioSource>();
            PathEventManager.onLifeLost += LooseALife;
            PathEventManager.updateMulligan += Mulligan;
            PathEventManager.onCardDraw += Draw;
            PathEventManager.onCardPlayed += PlayCard;
            PathEventManager.onHUDBtn += HUDButton;
            PathEventManager.onEndOfTurnBtn += EOTButton;
            PathEventManager.onDailyUpKeep += DailyUpkeep;
            PathEventManager.onGainedArrow += GainArrow;
            PathEventManager.onGainedWheat += GainWheat;
            PathEventManager.onBossKilled += BossKilled;
            PathEventManager.playKilledMonster += MonsterKilled;
            SettingsOverlay.OnMusicVolumeChanged += SetUpVolume;
            if (SettingsData.ActualSettingsData != null)  audioSource.volume = SettingsData.ActualSettingsData.actualVolumeSFX; 

        }

        void LooseALife()
        {
            PlaySound(soundToPlay1, "LifeLost");
        }

        void Mulligan(int num)
        {
            PlaySound(soundToPlay2, "Mulligan");
        }

        void Draw()
        {
            PlaySound(soundToPlay3, "Draw");
        }

        void PlayCard()
        {
            PlaySound(soundToPlay4, "CardPlay");
        }

        void HUDButton()
        {
            PlaySound(soundToPlay5, "HUDBtn");
        }

        void EOTButton()
        {
            PlaySound(soundToPlay6, "EndOfTurn");
        }

        void DailyUpkeep()
        {
            PlaySound(soundToPlay7, "DailyUpkeep");
        }

        void GainArrow()
        {
            PlaySound(soundToPlay8, "GainedArrow");
        }

        void GainWheat()
        {
            PlaySound(soundToPlay9, "GainedWheat");
        }

        void BossKilled()
        {
            PlaySound(soundToPlay10, "BossKilled");
        }

        void MonsterKilled()
        {
            PlaySound(soundToPlay11, "MonsterKilled");
        }

        void PlaySound(AudioClip clip, string message)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
                //Debug.Log(message);
            }
        }

        private void OnDestroy()
        {
            PathEventManager.onLifeLost -= LooseALife;
            PathEventManager.updateMulligan -= Mulligan;
            PathEventManager.onCardDraw -= Draw;
            PathEventManager.onCardPlayed -= PlayCard;
            PathEventManager.onHUDBtn -= HUDButton;
            PathEventManager.onEndOfTurnBtn -= EOTButton;
            PathEventManager.onDailyUpKeep -= DailyUpkeep;
            PathEventManager.onGainedArrow -= GainArrow;
            PathEventManager.onGainedWheat -= GainWheat;
            PathEventManager.onBossKilled -= BossKilled;
            PathEventManager.playKilledMonster -= MonsterKilled;
            SettingsOverlay.OnMusicVolumeChanged -= SetUpVolume;
        }

        public void SetUpVolume(float pVolume, VolumeType pType)
        {
            if (pType == VolumeType.SFX)
                audioSource.volume = pVolume;
        }
    }

    

}