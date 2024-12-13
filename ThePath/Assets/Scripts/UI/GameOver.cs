using Com.IsartDigital.F2P.Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Com.IsartDigital.F2P.Utils;
using TMPro;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.UI.HUD
{
    public class GameOver : Menu.Menu
    {
        GameManager _GameManager => GameManager.instance;
        private int _CurrentScene => SceneManager.GetActiveScene().buildIndex;
        public void ReloadScene() => SceneManager.LoadScene(_CurrentScene);

        [SerializeField] TextMeshProUGUI _PalierText = default;
        [SerializeField] TextMeshProUGUI _MonsterKilledText = default;
        [SerializeField] TextMeshProUGUI _DayPassedText = default;
        [SerializeField] TextMeshProUGUI _WheatText = default;
        [SerializeField] TextMeshProUGUI _AttackText = default;
        [SerializeField] private Transform _HeartContainer = default;
        private List<GameObject> _Hearts => _HeartContainer.GetChildren() as List<GameObject>;
        private StatsManager _StatsManager => StatsManager.instance;

        public void WatchPub()
        {
            _GameManager?.ReplayWithPub();
        }
        private void Start()
        {
            PathEventManager.onLifeLost += UpdateLife;
            PathEventManager.stepChanged += UpdateLevel;
            PathEventManager.dayPassed += UpdateDayPassed;
            PathEventManager.monsterKilled += UpdateMonsterKilled;
        }
        private void UpdateLevel(int pLevel) => _PalierText.text = "Palier : " + _GameManager.hud.stepCounter.text;
        private void UpdateMonsterKilled(int pMonsterKilled) => _MonsterKilledText.text = "Monstres tués : " + pMonsterKilled;
        private void UpdateDayPassed(int pDayPassed) => _DayPassedText.text = "Nombre de jour passés : " + pDayPassed;
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

        private void OnEnable()
        {
            LoadInfo();
        }

        private void OnDestroy()
        {
            PathEventManager.onLifeLost -= UpdateLife;
            PathEventManager.stepChanged -= UpdateLevel;
            PathEventManager.dayPassed -= UpdateDayPassed;
            PathEventManager.monsterKilled -= UpdateMonsterKilled;
        }
        public void LoadInfo()
        {
            _AttackText.text = _StatsManager.GetAttackValueTxt();
            _WheatText.text = _StatsManager.GetWheatValueTxt();
        }
    }
}