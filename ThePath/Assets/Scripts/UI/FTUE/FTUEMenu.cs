using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.Game.FTUE
{
    [Serializable] public class FTUE_UI_Event
    {
        public GameObject popUp;
        public List<Button> btnToActive;
        public List<Button> btnToDesactive;
        public FTUEState newState;
    }
    public class FTUEMenu : MonoBehaviour
    {
        [SerializeField] private List<FTUE_UI_Event> _UI_Events = new List<FTUE_UI_Event>();
        public FTUEState state = FTUEState.Menu;
        private PlayerData _PlayerData => PlayerData.ActualPlayerData;
        private FTUE_UI_Event _CurrentUIEvent;
        #region SINGLETON

        private static FTUEMenu Instance;
        public static FTUEMenu instance
        {
            get => Instance;
            private set => Instance = value;
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion 
        
        void Start()
        {
            if (!_PlayerData.isFTUE)
            {
                Destroy(gameObject);
                state = FTUEState.End;
            }
            else
            {
                state = FTUEState.DeckMenu;
                ChangeUIFTUE();
            }
        }

        public void ChangeUIFTUE()
        {
            ++state;
            if (state < FTUEState.End)
            {
                _CurrentUIEvent?.popUp?.SetActive(false);
                _CurrentUIEvent = _UI_Events.Find(x => x.newState == state);

                if (_CurrentUIEvent == null) return;
                _CurrentUIEvent?.popUp?.SetActive(true);
                foreach (Button btn in _CurrentUIEvent.btnToDesactive) btn.interactable = false;
                foreach (Button btn in _CurrentUIEvent.btnToActive) btn.interactable = true;
            }
            else
            {
                foreach (Button btn in _CurrentUIEvent.btnToDesactive) btn.interactable = true;
                foreach (Button btn in _CurrentUIEvent.btnToActive) btn.interactable = true;
                _PlayerData.isFTUE = false;
                Destroy(gameObject);
            }
        }
    }
}