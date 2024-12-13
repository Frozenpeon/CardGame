using Com.IsartDigital.F2P.UI.Overlay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P
{
    [Serializable]
    public class OnVolumeChangedEvent : UnityEvent<float, VolumeType> { }

    public class VolumeSliderSetUp : MonoBehaviour
    {
        private Slider _Slider;

        public VolumeType volumeType;

        public OnVolumeChangedEvent VolumeChangedEvent;

        private void Start()
        {
            _Slider = GetComponent<Slider>();
            if (volumeType == VolumeType.General)
                _Slider.value = SettingsData.ActualSettingsData.actualVolumeMusic * 100;
            else
                _Slider.value = SettingsData.ActualSettingsData.actualVolumeSFX * 100;
        }

        public void SetUpVolume(float pVolume)
        {
            VolumeChangedEvent?.Invoke(pVolume, volumeType);
        }
        
    }
}
