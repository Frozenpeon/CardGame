using Com.IsartDigital.F2P.UI.Overlay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    /// <summary>
    /// the datas about the settings.
    /// </summary>
    public class SettingsData : SavingObject
    {
        public static SettingsData ActualSettingsData;

        public static event Action<float, VolumeType> VolumeLoaded;

        // TODO : Add the values we want to save, music/SFX value ? 
        public float actualVolumeMusic = 1;
        public float actualVolumeSFX = 1;


        public SettingsData()
        {
            AddListeners();
            ActualSettingsData = this;
        }

        private void AddListeners()
        {
            SettingsOverlay.OnMusicVolumeChanged += SaveVolume;
        }

        private void RemoveListeners()
        {
            SettingsOverlay.OnMusicVolumeChanged -= SaveVolume;
        }

        public void SaveVolume(float pVolume, VolumeType pType)
        {
            Debug.Log("Saving volume");

            if (pType == VolumeType.General)
                actualVolumeMusic = pVolume;
            else 
                actualVolumeSFX = pVolume;
            SaveSystem.SaveActualDatas();
        }

    }
}
