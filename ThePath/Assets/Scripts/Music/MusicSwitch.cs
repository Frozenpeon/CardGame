using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public enum MenuMusicSwitch
    {
        Menu, 
        GameLoop
    }

    public class MusicSwitch : MonoBehaviour
    {
        public MenuMusicSwitch switchTo;

        public static event Action SwitchToMenu;
        public static event Action SwitchTogame;

        void Start()
        {
            if (switchTo == MenuMusicSwitch.Menu) SwitchToMenu?.Invoke();
            else SwitchTogame?.Invoke();
        }

      
    }
}
