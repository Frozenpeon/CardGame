using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//Author : Julian Martin & Alexandre Le Bacquer
namespace Com.IsartDigital.F2P.Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] public Camera mainCamera;
        public Vector3 mousePos;


        [Space(10)] public TurnManager turnManager;
        [Space(10)] public HUD hud;


        [Serializable] public class TurnManager
        {
            internal const string TURN_TEXT = "Turn ";
            public TextMeshProUGUI turnText;
            public int turnCount;
        }

        [Serializable] public class HUD
        {
            public Button playTurnButton;
            public TextMeshProUGUI monsterKilledCount = default;
            public TextMeshProUGUI stepCounter = default;
        }

        [SerializeField] private int _StartHealth = 4;
        [SerializeField] private int _StartWheat = 0;
        [SerializeField] private int _StartAttack = 0;

        [SerializeField] private int _HealthOnRevive = 1;
        [SerializeField] private int _WheatOnRevive = 2;
        [SerializeField] private int _AttackOnRevive = 2;

        public uint startMulligans = 5;
        private StatsManager _StatsManager => StatsManager.instance;
        private Deck _Deck => Deck.instance;

        [SerializeField] private GameObject _GameOver = default;


        #region SINGLETON
        public static GameManager instance;

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
            StartTurn();
            _StatsManager.HealthValue = _StatsManager.MaxHealthValue = _StartHealth;
            _StatsManager.WheatValue = _StatsManager.startWheatValue = _StartWheat;
            _StatsManager.AttackValue = _StatsManager.startAttackValue = _StartAttack;

            _StatsManager.StoreTempValues();
            hud.playTurnButton.onClick.AddListener(StartTurn);

            _Deck.nMulligan = startMulligans;
            if (_GameOver) _GameOver.SetActive(false);
        }

        private void Update()
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
        }

        private void StartTurn()
        {
            turnManager.turnCount++;
            UpdateTurn(turnManager.turnCount);
        }

        public void OnTurnOver()
        {
            StartTurn();
            UpdateGameInfos();
        }

        public void UpdateGameInfos()
        {
            if (_StatsManager.WheatValue < 1)
            {
                hud.playTurnButton.interactable = false;
            }
            else hud.playTurnButton.interactable = true;
        }

        private void UpdateTurn(int turnCount)
        {
            if (turnManager.turnText) turnManager.turnText.text = TurnManager.TURN_TEXT + turnCount.ToString();
        }

        public void GameOver()
        {
            if (_GameOver) _GameOver.SetActive(true);
        }
        public void ReplayWithPub()
        {
            if (_GameOver) _GameOver.SetActive(false);
            _StatsManager.HealthValue = _HealthOnRevive;
            _StatsManager.WheatValue = _StatsManager.startWheatValue = _WheatOnRevive;
            _StatsManager.AttackValue = _StatsManager.startAttackValue = _AttackOnRevive;

            _StatsManager.StoreTempValues();
        }

        public List<RaycastResult> PointerTouched()
        {
            PointerEventData lPointer = new PointerEventData(EventSystem.current);
            lPointer.position = Input.mousePosition;

            List<RaycastResult> lRaycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(lPointer, lRaycastResults);

            return lRaycastResults;
        }

        private void OnDestroy()
        {
            hud.playTurnButton.onClick.RemoveListener(StartTurn);
        }
    }
}
