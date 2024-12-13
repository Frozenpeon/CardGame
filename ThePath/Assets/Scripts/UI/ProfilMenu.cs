using Com.IsartDigital.F2P.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

namespace Com.IsartDigital.F2P.UI.Overlay
{
    public class ProfilMenu : MonoBehaviour
    {
        public TMP_InputField TMP_InputField;

        private PlayerData _PlayerData => PlayerData.ActualPlayerData;

        private int _IndexCharette = 0;
        private int _IndexBG = 0;
        private static Action OnBtnBackClick;

        private void Start()
        {
            LoadData();
        }

        public void SetUpUsername()
        {
            PlayerData.ActualPlayerData.PlayerUsernName = TMP_InputField.text != "" ? TMP_InputField.text : "Name";
            gameObject.SetActive(false);
            SaveSystem.SaveActualDatas();
        }

        public void UpdateCharette(bool pIsLeft)
        {
            _IndexCharette += pIsLeft ? (_IndexCharette > 0 ? -1 : 0) : (_IndexCharette < _PlayerData.charetteSkinPossessed.Count ? 1 : 0);
        }

        public void UpdateBG(bool pIsLeft) => _IndexBG += pIsLeft ? (_IndexBG > 0 ? -1 : 0) : (_IndexBG < _PlayerData.bgSkinPossessed.Count ? 1 : 0);

        public void OnEquiped(bool pIsBG) 
        {
            if (pIsBG) _PlayerData.currentBGSkinEquiped = _PlayerData.bgSkinPossessed[_IndexBG];
            else _PlayerData.currentCharetteSkinEquiped = _PlayerData.charetteSkinPossessed[_IndexCharette];
        }

        public void OnReset()
        {
            /*PlayerData.ActualPlayerData = new();
            GameStateData.ActualGameStateData = new();
            ShopData.ActualShopData = new();
            SettingsData.ActualSettingsData = new();
            AugmentSaver.augmentSaverInstance = new AugmentSaver();*/
            SaveSystem.actualDatas = new SavedDatas(SaveSystem.SOList);
            SaveSystem.SaveActualDatas();
            Manager.UIManager.InvokeLoadData();
        }

        private void LoadData()
        {
            if (_PlayerData ==  null) return;

            if (_PlayerData.charetteSkinPossessed == null) 
            {
                _PlayerData.charetteSkinPossessed = new List<int>
                {
                    0
                };
            }
            if (_PlayerData.bgSkinPossessed == null)
            {
                _PlayerData.bgSkinPossessed = new List<int>
                {
                    0
                };
            }

            if (_PlayerData.charetteSkinPossessed.Contains(_PlayerData.currentCharetteSkinEquiped))
            {
                _IndexCharette = _PlayerData.charetteSkinPossessed.IndexOf(_PlayerData.currentCharetteSkinEquiped);
            }
            if (_PlayerData.bgSkinPossessed.Contains(_PlayerData.currentBGSkinEquiped))
            {
                _IndexBG = _PlayerData.bgSkinPossessed.IndexOf(_PlayerData.currentBGSkinEquiped);
            }

            TMP_InputField.text = PlayerData.ActualPlayerData.PlayerUsernName;
        }

        private void OnEnable()
        {
            OnBtnBackClick += UIManager.GetInstance.SwitchQuestOverlay;
        }

        private void OnDisable()
        {
            OnBtnBackClick -= UIManager.GetInstance.SwitchQuestOverlay;
            OnBtnBackClick = null;
        }
        public void OnBackBtn()
        {
            OnBtnBackClick?.Invoke();
        }
    }
}
