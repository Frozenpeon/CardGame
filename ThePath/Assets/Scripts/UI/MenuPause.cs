using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Com.IsartDigital.F2P.Utils;
using Com.IsartDigital.F2P.Manager;
using TMPro;

namespace Com.IsartDigital.F2P.UI.Menu
{
    public class MenuPause : Menu
    {
        [SerializeField] TextMeshProUGUI _LevelText = default;
        [SerializeField] private Transform _HeartContainer = default;
        private List<GameObject> _Hearts => _HeartContainer.GetChildren() as List<GameObject>;
        private StatsManager _StatsManager => StatsManager.instance;
        private GameManager _GameManager => GameManager.instance;

        private UIManager _UIManager => UIManager.GetInstance;

        #region Singleton
        private static MenuPause Instance;
        public static MenuPause instance
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

        private void Start()
        {
            if (_UIManager) OnBtnSettingsClick += _UIManager.SwitchSettingsOverlay;
            PathEventManager.onLifeLost += UpdateLife;
            PathEventManager.stepChanged += UpdateLevel;
        }

        private void UpdateLevel(int pLevel) => _LevelText.text = _GameManager.hud.stepCounter.text;
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

        private void OnDestroy()
        {
            if (_UIManager) OnBtnSettingsClick -= _UIManager.SwitchSettingsOverlay;
            PathEventManager.onLifeLost -= UpdateLife;
            PathEventManager.stepChanged -= UpdateLevel;
            if (instance != null) instance = null;
        }
    }
}
