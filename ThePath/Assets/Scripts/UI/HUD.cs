using Com.IsartDigital.F2P.Analytics;
using Com.IsartDigital.F2P.Game.Slot;
using Com.IsartDigital.F2P.Manager;
using Com.IsartDigital.F2P.UI.HUD.Augment;
using Com.IsartDigital.F2P.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.UI.HUD
{
    [Serializable] public class OverlayInfo
    {
        public Canvas canvas;
        public InfoType infoType;
    }
    public enum InfoType
    {
        GameProgress, Augment, Pause
    }
    public class HUD : Menu.Menu
    {
        [SerializeField] private Image _ProgressBarFill = default;
        [SerializeField] private Transform _HeartContainer = default;

        [SerializeField] private GameObject _AugmentDisplayPrefab = default;
        [SerializeField] private GameObject _AugmentContainer = default;

        [SerializeField] private List<OverlayInfo> _OverlayInfos = default;

        private bool _OpenInfo = false;

        private List<GameObject> _Hearts => _HeartContainer.GetChildren() as List<GameObject>;
        private StatsManager _StatsManager => StatsManager.instance;
        private AugmentHandler _AugmentHandler => AugmentHandler.Instance;

        [SerializeField] private GetInfo _StartGame;
        [SerializeField] private GetInfo _Mulligan;

        #region Singleton
        private static HUD Instance;
        public static HUD instance
        {
            get => Instance;
            private set => Instance = value;
        }

        private void Awake()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;
        }
        #endregion
        void Start()
        {
            if (!PlayerData.ActualPlayerData.isFTUE) PathEventManager.monsterKilledRatio += UpdateAugmentProgressBar;
            PathEventManager.onLifeLost += UpdateLife;
            PathEventManager.updateMulligan += SendMulliganInfo;

            List<object> lList = new List<object>()
            {
                PlayerData.ActualPlayerData.PlayerDataID, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
            };

            foreach (ParametersData lParameter in _StartGame.eventData.parameters)
            {
                _StartGame.GetValue(lParameter.paramName, lList[_StartGame.eventData.parameters.IndexOf(lParameter)]);
            }
            _StartGame.SendEvent();
            PathEventManager.isFirstTimeLaunch = GameStateData.ActualGameStateData.isFirstTimeLaunch;

            if (!PathEventManager.isFirstTimeLaunch)
            {
                PathEventManager.InvokeLoadData();
                UpdateLife();
            }
            PathEventManager.isFirstTimeLaunch = true;
        }

        public void QuitToMainMenu()
        {
            if (PlayerData.ActualPlayerData.isFTUE)
            {
                PlayerData.ActualPlayerData.isFTUE = false;
                return;
            }
            PathEventManager.isFirstTimeLaunch = false;
            GameStateData.ActualGameStateData.isFirstTimeLaunch = PathEventManager.isFirstTimeLaunch;
            PathEventManager.InvokeSaveData();
            SaveSystem.SaveActualDatas(); 
        }

        private void UpdateLife() 
        {
            foreach (GameObject lHeart in _Hearts)
            {
                lHeart.GetComponentInChildren<Image>().color = Color.black;
            }
            int lCount = _StatsManager.HealthValue;
            for (int i = 0; i < lCount; i++)
            {
                _Hearts[i].GetComponentInChildren<Image>().color = Color.white;
            }            
        }

        private void UpdateAugmentProgressBar(float pMonsterKilledRatio)
        {
            _ProgressBarFill.fillAmount = pMonsterKilledRatio;
            if (_ProgressBarFill.fillAmount >= 1)
            {
                Path.instance.charette.gameObject.SetActive(false);
                Path.instance.nMonsterKilledTemp = 0;
                AugmentHandler.Instance.Show();
            }
        }

        public void CloseInfo()
        {
            CloseAllInfos();
            _OpenInfo = !_OpenInfo;
        }

        public void OnInfoBtn(int pInfoTypeValue)
        {
            _OpenInfo = !_OpenInfo;
            if (!_OpenInfo) return;

            OverlayInfo lOverlay = _OverlayInfos.Find(x => x.infoType == (InfoType)pInfoTypeValue);

            if (lOverlay != null && lOverlay.canvas)
            {
                lOverlay.canvas.enabled = true;
                lOverlay.canvas.gameObject.SetActive(true);
            }
            else throw new Exception("No QuestOverlay has this Info or this QuestOverlay has a null canvas");
        }

        
        private void CloseAllInfos()
        {
            foreach (OverlayInfo lInfo in _OverlayInfos)
            {
                if (!lInfo.canvas) continue;
                lInfo.canvas.enabled = false;
                lInfo.canvas.gameObject.SetActive(false);
            }
        }

        private void SendMulliganInfo(int pInt)
        {
            List<object> lList = new List<object>()
            {
                PlayerData.ActualPlayerData.PlayerDataID, CheckCardsSlots.instance.GetCardsNameInSlot()
            };

            foreach (ParametersData lParameter in _Mulligan.eventData.parameters)
            {
                _Mulligan.GetValue(lParameter.paramName, lList[_Mulligan.eventData.parameters.IndexOf(lParameter)]);
            }
            _Mulligan.SendEvent();
        }

        public void PlayBtnSound(bool pIsEndTurnBtn)
        {
            if (pIsEndTurnBtn) PathEventManager.InvokeOnEndOfTurnBtn();
            else PathEventManager.InvokeOnHUDBtn();
        }

        public void OnBtnAugmentView()
        {
            List<AugmentSO> lList = new List<AugmentSO>(_AugmentHandler.activAugments);
            AugmentDisplay lAugment;
            AugmentLoader lAugmentLoader;
            GameObject go;
            foreach (AugmentSO lAugmentSO in lList)
            {
                Debug.Log("THIS IS THE AUGMENT : " + lAugmentSO);
                go = Instantiate(_AugmentDisplayPrefab, _AugmentContainer.transform);
                lAugmentLoader = go.GetComponent<AugmentLoader>();
                lAugmentLoader.LoadAnAugment(lAugmentSO);
            }
        }

        public void OnQuitAugmentView()
        {
            for (int i = 0; i < _AugmentContainer.transform.childCount; i++)
            {
                Destroy(_AugmentContainer.transform.GetChild(i).gameObject);
            }
        }

        public void ResetGame() => GameStateData.ActualGameStateData.isFirstTimeLaunch = true;

        private void OnDestroy()
        {
            PathEventManager.monsterKilledRatio -= UpdateAugmentProgressBar;
            PathEventManager.onLifeLost -= UpdateLife;
            PathEventManager.updateMulligan -= SendMulliganInfo;
            if (instance != null) instance = null;
        }

        private void OnApplicationQuit()
        {
            QuitToMainMenu();
        }
    }
}