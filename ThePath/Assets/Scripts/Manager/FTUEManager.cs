using Com.IsartDigital.F2P.SO.CardSO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Com.IsartDigital.F2P.Game.FTUE
{
    public enum PopUpEnum
    {
        Start, Monster, GiveToMonster, Resources, EndTurn, 
        LoseLife, TotalLife, CardEffect, CardMove, EndOfDay, 
        Augment
    }
    [Serializable] public class PopUpEvent
    {
        public GameObject popUpObject;
        public GameObject highlight;
        public PopUpEnum popUpEnum;
    }

    [Serializable] public class StepEvent
    {
        public uint nMonsterKilledCondition = 0;
        public uint nSlotToSpawn = 0;
        public uint nCardInHand = 0;
        public MonsterSO _MonsterToSpawn;
        public List<Deck.CardConfig> cardInHand = new List<Deck.CardConfig>();
        public List<AugmentConfig> augmentToSpawn = new List<AugmentConfig>();

        public List<AugmentConfig> augmentRandom = new List<AugmentConfig>();
        public List<MulliganConfig> mulliganConfig = new List<MulliganConfig>();
    }

    [Serializable] public class AugmentConfig
    {
        public AugmentSO augmentSO;
        public MonsterSO monsterToSpawn;
        public List<Deck.CardConfig> cardInHand = new List<Deck.CardConfig>();
    }
    [Serializable]
    public class MulliganConfig
    {
        public List<Deck.CardConfig> cardDraw= new List<Deck.CardConfig>();
    }

    public enum FTUEState
    {
        Start, Damage, DayCost, FirstAugment, AugmentHand, RandomAugment, Reward, Menu,
        DeckMenu, GoToMainMenu, ShopMenu, Booster, End
    }

    public class FTUEManager : MonoBehaviour
    {
        [SerializeField] private List<StepEvent> _StepEvents = new List<StepEvent>();
        public StepEvent currentStep = null;
        public FTUEState state = FTUEState.Start;
        public int augmentChooseIndex = 0;
        [SerializeField] private GameObject _EndScreen;
        [SerializeField] private GameObject _Life;
        [SerializeField] private GameObject _Mulligan;
        [SerializeField] private GameObject _Hand;
        [SerializeField] private Transform _NextHandPose;

        [SerializeField] private List<PopUpEvent> _PopUpList = new List<PopUpEvent>();
        public PopUpEnum popEnum = PopUpEnum.Start;
        public PopUpEvent currentPopUp = null;

        private AugmentHandler _AugmentHandler => AugmentHandler.Instance;
        #region SINGLETON

        private static FTUEManager Instance;
        public static FTUEManager instance
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
            _Life.SetActive(false);
            _Mulligan.SetActive(false);
            PathEventManager.onMonsterKilled += UpdateFTUE;
            PathEventManager.desactivateHand += DesactivateHand;
            PathEventManager.moveHand += MoveHand;
            LockAllCardsSO();
        }
        public void ActivateHand() => _Hand.SetActive(true); 
        public void DesactivateHand() => _Hand.SetActive(false); 
        public void MoveHand() => _Hand.transform.position = _NextHandPose.position;
        public void StartFTUE() => PathEventManager.InvokeOnMonsterKilled(0);
        
        public void ChangePopUp()
        {
            PopUpEvent l = _PopUpList.Find(x => x.popUpEnum == popEnum);
            if (l != null && l.highlight != null) l?.highlight?.SetActive(false);
            popEnum++;

            PopUpEvent lPopUpEvent = _PopUpList.Find(x => x.popUpEnum == popEnum);
            lPopUpEvent?.popUpObject.SetActive(true);
            if(lPopUpEvent != null && lPopUpEvent.highlight != null)lPopUpEvent?.highlight?.SetActive(true);
        }

        public void LockAllCardsSO()
        {
            UnityEngine.Object[] assets = Resources.LoadAll("ScriptableObject/Card/Cards/", typeof(CardSO));

            CardSO[] cardSOs = assets.OfType<CardSO>().ToArray();

            foreach (var cardSO in cardSOs)
            {
                cardSO.isUnlocked = false;
            }
        }

        public void UpdateCurrentStep(int pNMonsterKilled) 
        {
            currentStep = _StepEvents.Find(x => x.nMonsterKilledCondition == pNMonsterKilled);

            if (currentStep == null) return;

            switch (currentStep.nMonsterKilledCondition)
            {
                case 0:
                    state = FTUEState.Start;
                    break;
                case 1:
                    _Life.SetActive(true);
                    state = FTUEState.Damage;
                    break;
                case 2:
                    state = FTUEState.DayCost;
                    break;
                case 3:
                    state = FTUEState.FirstAugment;
                    _AugmentHandler.FTUEAugments = CreateListFromAugmentConfig();
                    break;
                case 4:
                    _AugmentHandler.augmentAlreadySpawn.Clear();
                    state = FTUEState.AugmentHand;
                    _AugmentHandler.FTUEAugments = CreateListFromAugmentConfig();
                    break;
                case 5:
                    _Mulligan.SetActive(true);
                    state = FTUEState.RandomAugment;
                    if (_AugmentHandler.augmentAlreadySpawn.Contains(currentStep.augmentRandom[0].augmentSO))
                    {
                        currentStep.augmentToSpawn.Add(currentStep.augmentRandom[1]);
                    }
                    else if (_AugmentHandler.augmentAlreadySpawn.Contains(currentStep.augmentRandom[1].augmentSO))
                    {
                        currentStep.augmentToSpawn.Add(currentStep.augmentRandom[0]);
                    }
                    else
                    {
                        currentStep.augmentToSpawn.Add(currentStep.augmentRandom[UnityEngine.Random.Range(0, currentStep.augmentRandom.Count)]);
                    }
                    _AugmentHandler.FTUEAugments = CreateListFromAugmentConfig();
                    break;
                default:
                    break;
            }

        }
        private void UpdateFTUE(int pNMonsterKilled)
        {
            currentStep = _StepEvents.Find(x => x.nMonsterKilledCondition == pNMonsterKilled);
            if (pNMonsterKilled == _StepEvents.Count)
            {
                state = FTUEState.Reward;
                PlayerData.ActualPlayerData.ftueState = state;
                _EndScreen.SetActive(true);
                return;
            }
            if (currentStep == null) return;

            switch (currentStep.nMonsterKilledCondition)
            {
                case 0:
                    state = FTUEState.Start;
                    break;
                case 1:
                    _Life.SetActive(true);
                    state = FTUEState.Damage;
                    break;
                case 2:
                    state = FTUEState.DayCost;
                    break;
                case 3:
                    state = FTUEState.FirstAugment;
                    _AugmentHandler.FTUEAugments = CreateListFromAugmentConfig();
                    break;
                case 4:
                    _AugmentHandler.augmentAlreadySpawn.Clear();
                    state = FTUEState.AugmentHand;
                    _AugmentHandler.FTUEAugments = CreateListFromAugmentConfig();
                    break;
                case 5:
                    _Mulligan.SetActive(true);
                    state = FTUEState.RandomAugment;
                    if (_AugmentHandler.augmentAlreadySpawn.Contains(currentStep.augmentRandom[0].augmentSO))
                    {
                        currentStep.augmentToSpawn.Add(currentStep.augmentRandom[1]);
                    }
                    else if (_AugmentHandler.augmentAlreadySpawn.Contains(currentStep.augmentRandom[1].augmentSO))
                    {
                        currentStep.augmentToSpawn.Add(currentStep.augmentRandom[0]);
                    }
                    else
                    {
                        currentStep.augmentToSpawn.Add(currentStep.augmentRandom[UnityEngine.Random.Range(0, currentStep.augmentRandom.Count)]);
                    }
                    _AugmentHandler.FTUEAugments = CreateListFromAugmentConfig();
                    break;
                default:
                    break;
            }

            PathEventManager.InvokeOnFTUEStepChanged(currentStep, state);
        }

        private List<AugmentSO> CreateListFromAugmentConfig()
        {
            List<AugmentSO> lList = new List<AugmentSO>(); 
            foreach (AugmentConfig lAugment in currentStep.augmentToSpawn)
            {
                lList.Add(lAugment.augmentSO);
            }
            return lList;
        }
        private void OnDestroy()
        {
            PathEventManager.onMonsterKilled -= UpdateFTUE;
            PathEventManager.desactivateHand -= DesactivateHand;
            PathEventManager.moveHand -= MoveHand;
            if (instance != null) instance = null;
        }
    }
}