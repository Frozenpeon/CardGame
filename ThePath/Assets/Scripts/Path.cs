using Com.IsartDigital.F2P.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.IsartDigital.F2P.Utils;
using Com.IsartDigital.F2P.SO.CardSO;
using Com.IsartDigital.F2P.Game.Slot;
using Com.IsartDigital.F2P.Game;
using Com.IsartDigital.F2P.SO.QuestSO;
using Com.IsartDigital.F2P.UI.Menu;
using Com.IsartDigital.F2P.Analytics;
using System;
using Com.IsartDigital.F2P.Game.FTUE;


//By Matteo Renaudin
namespace Com.IsartDigital.F2P.Manager
{
    public enum BattleState
    {
        None, Win, Lose
    }
    public enum ResourcesOnMonster
    {
        None, Wheat, Arrow, WheatArrow
    }

    [Serializable] public class MonsterSaveInSlot
    {
        public int monsterID;
        public int monsterLevel;
        public int slotIndex;
        public bool isBoss;
        public Vector3 position;
    }
    public class Path : MonoBehaviour, ISavedGameElement
    {
        private bool _PathIsActivated = false;
        private float _Ratio = 0f;
        private Coroutine _Coroutine = null;
        private int _CardsAttackValue = 0;
        private int _CardsWheatValue = 0;
        private int _CardsLifeRemoved = 0;
        private int _DayPassed = 0;
        private bool _LastChance = true;
        private BattleState _BattleState = BattleState.None;
        private ResourcesOnMonster _ResourcesOnMonster = ResourcesOnMonster.None;
        #region SLOTS
        [Header("SLOT'S VARIABLES")]
        [SerializeField] private GameObject _SlotPrefab = default;
        [SerializeField] private float _MoveSpeed = 1f;
        [Tooltip("Number of cards to spawn on start")][SerializeField] private uint _NSlotToSpawn = 5;
        [Tooltip("Step to spawn monster")][SerializeField] private uint _NSlotToSpawnMonster = 5;
        [SerializeField] private int[] _NMonsterToSpawnNewCardSlot = null;
        [SerializeField] private Transform _DeletedSlotDirection;
        public Transform charette;
        private Vector3 _CharetteDir;
        private Vector3 _CharetteStartPose;
        [SerializeField] private float _TimeSpendOnSlot = 1f;
        public List<GameObject> slots => transform.GetChildren() as List<GameObject>;
        
        private GameObject _Slot;
        private uint _NormalSlotAlreadySpawn = 0;
        private uint _IndNewCardSlot = 0;
        private uint _NSlotToDelete = 2;
        private uint _TotalSlotSpawn = 0;
        public uint nStep = 0;
        #endregion

        #region GET INSTANCE
        private CheckCardsSlots _CheckCardsSlots => CheckCardsSlots.instance;
        private StatsManager _StatsManager => StatsManager.instance;
        private Deck _Deck => Deck.instance;
        private AugmentHandler _AugmentHandler => AugmentHandler.Instance;
        private GameManager _GameManager => GameManager.instance;
        #endregion

        [Space(10)]

        #region MONSTER
        [Header("MONSTER'S VARIABLES")]
        [SerializeField] private int _MinMonsterLevel = 1;
        [SerializeField] private int _MaxMonsterLevel = 1;
        [SerializeField] private int _EachNMonsterSpawnBoss = 3;
        [SerializeField] private List<MonsterSO> _MonsterSOsInPath = new List<MonsterSO>();
        public List<MonsterSaveInSlot> monstersSaved = new List<MonsterSaveInSlot>();
        public List<List<Monster>> monsterDB = new List<List<Monster>>();
        public List<List<Monster>> bossDB = new List<List<Monster>>();
        private const string MONSTER_DIRECTORY_PATH = "ScriptableObject/Card/Monsters/";
        private const string BOSS_DIRECTORY_PATH = "ScriptableObject/Card/Boss/";
        private bool _MonsterDestroyed = false;
        public uint nMonsterKilled = 0;
        public uint nMonsterKilledTemp = 0;
        private uint _NMonsterAlreadySpawn = 0;
        private bool _SpawnMonster = true;
        [SerializeField] private int _NMonsterKilledToSpawnAugment = 3;

        private Monster _FTUEMonster = null;
        #endregion

        [Space(10)]        

        #region GOD MOD
        [Header("God Mod (for testing features)")]

        [SerializeField] private bool _StartWithGodMod = false;
        [SerializeField] private uint _SlotToSpawnWithGodMod = 2;
        #endregion

        #region SINGLETON

        private static Path Instance;
        public static Path instance
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

        [SerializeField] private GetInfo _Validate;
        [SerializeField] private GetInfo _MeetMonster;
        [SerializeField] private GetInfo _FinishLanding;
        [SerializeField] private GetInfo _LoseHP;
        void Start()
        {
            PathEventManager.saveData += SaveData;
            PathEventManager.onFTUEStepChanged += GenerateFTUEPath;

            PathEventManager.onAugmentChoose += ActivateCharette;
            AddMonstersInDB();
            if (!GameStateData.ActualGameStateData.isFirstTimeLaunch && !PlayerData.ActualPlayerData.isFTUE)
            {
                _SpawnMonster = false;
                LoadData();
            }
            if (!PlayerData.ActualPlayerData.isFTUE)
            {
                for (int i = 0; i < _NSlotToSpawn; i++)
                {
                    SpawnSlot();
                }
            }
            _SpawnMonster = true;

            if (!GameStateData.ActualGameStateData.isFirstTimeLaunch)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].GetComponent<Slot>().dayPart = GameStateData.ActualGameStateData.dayPartInSlot[i];
                    slots[i].GetComponent<Slot>().SetDayPartImage();
                }
                _TotalSlotSpawn -= (uint)slots.Count;
            }

            foreach (MonsterSaveInSlot lMonster in monstersSaved)
            {
                if (lMonster.isBoss) slots[lMonster.slotIndex].GetComponent<Slot>().monster = LoadMonsters(bossDB[lMonster.monsterLevel - 1].Find(x => x.monsterSO.ID == lMonster.monsterID)?.monsterSO);
                else slots[lMonster.slotIndex].GetComponent<Slot>().monster = LoadMonsters(monsterDB[lMonster.monsterLevel - 1].Find(x => x.monsterSO.ID == lMonster.monsterID)?.monsterSO);

                slots[lMonster.slotIndex].GetComponent<Slot>().UpdateSlot(true, 1);
            }
        }

        public void OnBtnPlay()
        {
            if (_CheckCardsSlots.cardsInSlot.Count <= 0 || _PathIsActivated) return;

            _PathIsActivated = true;
            if (FTUEManager.instance && FTUEManager.instance.popEnum == PopUpEnum.EndTurn) FTUEManager.instance.ChangePopUp();

            PathEventManager.InvokeOnPathMoved();
            List<object> lList = new List<object>()
            {
                PlayerData.ActualPlayerData.PlayerDataID, _Deck.GetCardsNameInDeck(), _CheckCardsSlots.GetCardsNameInSlot()
            };
            SendEvent(_Validate, lList);

            if (_Deck.cemetary.Count!=0)
                _Deck.MoveCardsFromListToList(_Deck.cemetary, _Deck.pool, _Deck.cemetary.Count);

            EventManager.TriggerPlayOccured(PlayType.Turn);

            GameObject lCard = null;
            PlayableCard lPlayableCard = null;
            bool lFristCropFieldIsFind = false;
            foreach (GameObject lSlots in slots) 
            {
                lCard = lSlots.GetComponent<Slot>().card;
                if (lCard == null) continue;
                lCard.transform.GetChild(0).GetComponent<MovableCard>().canMove = false;
                lPlayableCard = lCard.GetComponent<PlayableCard>();

                if (lPlayableCard.cardDisplay.cardSO.cardType == CardType.CropField && !lFristCropFieldIsFind)
                {
                    GameStateChanges.InvokeCardRemovedFromHand(lPlayableCard);
                    lFristCropFieldIsFind = true;
                }

                EventManager.TriggerCardSOPlayed(lPlayableCard.cardDisplay.cardSO);
            }

            _Ratio = 0f;
            _NSlotToDelete = 0;

            _CardsWheatValue = 0;
            _CardsAttackValue = 0;
            _CardsLifeRemoved = 0;

            CheckHowManySlotToDelete();
            SetCardMovement();
        }
        private void CheckHowManySlotToDelete()
        {
            if (_StartWithGodMod)
            {
                _NSlotToDelete = _SlotToSpawnWithGodMod;
                return;
            }

            Slot lSlot = null;
            bool lStopCheck = false;
            int lSlotCount = slots.Count;
            MonsterSO lMonster = _MonsterSOsInPath[0];
            for (int i = 0; i < lSlotCount; i++)
            {
                // To check how many card we want to delete
                lSlot = slots[i].transform.GetComponent<Slot>();

                if (lSlot.isUsed) _NSlotToDelete += 1;
                else
                {
                    if (!lSlot.btn.interactable && !lStopCheck)
                    {
                        EventManager.TriggerEnemyKilled(lMonster.isBoss ? EnemiesType.Boss : EnemiesType.Monster);

                        
                        if (lMonster.isBoss)
                        {
                            ++nStep;
                            PathEventManager.InvokeStepChanged((int)nStep);
                            //_GameManager.hud.stepCounter.text = nStep.ToString();

                            TimeSpan lTime = DateTime.Now - PlayerData.ActualPlayerData.currentDate;
                            List<object> lList = new List<object>()
                            {
                                 PlayerData.ActualPlayerData.PlayerDataID, lTime.ToString()
                            };
                            GameStateChanges.InvokeEnemyDefeated();
                            SendEvent(_FinishLanding, lList);

                            _ResourcesOnMonster = ResourcesOnMonster.WheatArrow;
                        }
                        else
                        {
                            if (lMonster.attackRemoved > 0) _ResourcesOnMonster = ResourcesOnMonster.Arrow;
                            else if (lMonster.wheatRemoved > 0) _ResourcesOnMonster = ResourcesOnMonster.Wheat;
                        }

                        if (_CheckCardsSlots.CanBeatMonster(lMonster, i))
                        {

                            _CardsAttackValue -= Mathf.Clamp(lMonster.attackRemoved - AugmentHandler.Instance.GetDamageReduction() - RemovedPowerOnSlotIfDecoy(i), 0, lMonster.attackRemoved);
                            _CardsWheatValue -= Mathf.Clamp(lMonster.wheatRemoved - AugmentHandler.Instance.GetDamageReduction() - RemovedPowerOnSlotIfDecoy(i), 0, lMonster.wheatRemoved);
                            GameStateChanges.InvokeEnemyDefeated();
                            List<object> lList = new List<object>()
                            {
                                true, lMonster.name
                            };
                            SendEvent(_MeetMonster, lList);

                            _BattleState = BattleState.Win;
                        }
                        else
                        {
                            if ((FTUEManager.instance && FTUEManager.instance.state < FTUEState.Damage) ||
                                !FTUEManager.instance)
                            {
                                _CardsLifeRemoved -= lMonster.lifeRemoved;

                                List<object> lList = new List<object>()
                                {
                                    false, lMonster.name
                                };
                                SendEvent(_MeetMonster, lList);

                                lList = new List<object>()
                                {
                                     PlayerData.ActualPlayerData.PlayerDataID, _Deck.GetCardsNameInDeck(), lMonster.name, nMonsterKilled.ToString()
                                };
                                SendEvent(_LoseHP, lList);
                            }
                            _BattleState = BattleState.Lose;
                        }

                        if (_StatsManager.HealthValue > 0)
                        {
                            ++_NSlotToDelete;
                            _MonsterDestroyed = true;
                            nMonsterKilled += 1;
                            nMonsterKilledTemp += 1;
                           if (_GameManager.hud.monsterKilledCount) _GameManager.hud.monsterKilledCount.text = nMonsterKilled.ToString();
                            PathEventManager.InvokeMonsterKilled((int)nMonsterKilled);
                        }


                    }
                    else if (lSlot.btn.interactable) break;
                    lStopCheck = true;
                }
            }
        }

        public int RemovedPowerOnSlotIfDecoy(int pMonsterIndex) 
        {
            int lCount = 0;
            if (_AugmentHandler.ContainDecoy() && pMonsterIndex + 3 <= slots.Count)
            {
                int lSlotCount = slots.Count;
                for (int i = pMonsterIndex + 1; i < pMonsterIndex + 3; i++)
                {
                    if (slots[i].GetComponentInChildren<PlayableCard>()) lCount++;
                }
            }
            return lCount;
        }

        public float GetMonsterKilledToAugmentRatio() => nMonsterKilledTemp / (float)_NMonsterKilledToSpawnAugment;

        /// <summary>
        /// Set Movement and direction for each cards on play
        /// </summary>
        private void SetCardMovement()
        {
            Slot lSlot = null;
            GameObject lSlotObject = null;
            int lSlotCount = slots.Count;
            if (_NSlotToDelete >= 0)
            {
                lSlotCount = slots.Count;
                for (int i = 0; i < lSlotCount; i++)
                {
                    lSlotObject = slots[i];
                    lSlot = lSlotObject.GetComponent<Slot>();
                    lSlot.startPose = lSlot.transform.position;
                    lSlot.nextPose = i - _NSlotToDelete >= 0 ? slots[i - (int)_NSlotToDelete].transform.position : _DeletedSlotDirection.position;
                }
                _Coroutine = StartCoroutine(MovePath());
            }
        }
        
        /// <summary>
        /// Move Path on play
        /// </summary>
        /// <returns></returns>
        private IEnumerator MovePath()
        {
            _CharetteStartPose = charette.position;
            Charrette lCharrette = charette.GetComponent<Charrette>();
            Vector3 lTempPose = _CharetteStartPose;
            Slot lSlot = null;
            CardDisplay lCard = null;
            if (_NSlotToDelete > 0)
            {
                for (int i = 0; i < _NSlotToDelete; i++)
                {
                    _CharetteDir = slots[i].transform.position;
                    lSlot = slots[i].GetComponent<Slot>();

                    lCharrette.SetMoveTrigger();
                    while (_Ratio < 1)
                    {
                        _Ratio += Time.deltaTime * _MoveSpeed;
                        charette.position = Vector2.Lerp(lTempPose, new Vector3(_CharetteDir.x, charette.position.y), _Ratio);

                        yield return null;
                    }
                    lCharrette.StopMovement();
                    lTempPose = charette.position;
                    _Ratio = 0;


                    if (!lSlot.btn.interactable)
                    {
                        if (lSlot.monsterIsBoss) PathEventManager.InvokeOnBossKilled();
                        else PathEventManager.InvokePlayKilledMonster();

                        if (_BattleState == BattleState.Lose)
                        {
                            lCharrette.MeetMonster(false, (int)_ResourcesOnMonster - 1);
                        }
                        else if (_BattleState == BattleState.Win) lCharrette.MeetMonster(true, (int)_ResourcesOnMonster - 1);

                        if (_CardsLifeRemoved != 0 && _BattleState == BattleState.Lose && ((FTUEManager.instance && FTUEManager.instance.state >= FTUEState.Start) || !PlayerData.ActualPlayerData.isFTUE))
                        {
                            EventManager.CardPlayed(new CardEffect(0, _CardsLifeRemoved, 0, 0));
                            PathEventManager.InvokeOnLifeLost();
                        }
                    }
                    else
                    {
                        lCard = slots[i].GetComponentInChildren<CardDisplay>();
                        if (lSlot.dayPart == DayPart.Night)
                        {
                            PathEventManager.InvokeOnDailyUpKeep();
                            if ((FTUEManager.instance && FTUEManager.instance.state >= FTUEState.Start) || !PlayerData.ActualPlayerData.isFTUE)
                            {
                                GameStateChanges.InvokeDayPassed();
                                EventManager.TriggerPlayOccured(PlayType.Day);

                                _DayPassed++;
                                PathEventManager.InvokeDayPassed(_DayPassed);
                            }
                        }

                        if (lSlot.resGained == Ressources.attack)
                        {
                            PathEventManager.InvokeOnGainedArrow();
                            lCharrette._Cart_Script.GainArrow();
                        }
                        else if (lSlot.resGained == Ressources.wheat)
                        {
                            PathEventManager.InvokeOnGainedWheat();
                            lCharrette._Cart_Script.GainWheat();
                        }

                    }
                    yield return new WaitForSeconds(_TimeSpendOnSlot);
                }

            }
            while (_Ratio < 1)
            {
                _Ratio += Time.deltaTime * _MoveSpeed;

                foreach (GameObject lSlotObject in slots)
                {
                    lSlot = lSlotObject.GetComponent<Slot>();
                    lSlotObject.transform.position = Vector2.Lerp(lSlot.startPose, lSlot.nextPose, _Ratio);
                }                

                yield return null;
            } 
            _Ratio = 0;

            UpdateSlots();

            if (_NMonsterToSpawnNewCardSlot[_IndNewCardSlot] == nMonsterKilled)
            {
                _NSlotToSpawnMonster += 1;
                _IndNewCardSlot += 1;
                _NSlotToDelete += 1;
                _Deck.handSize += 1;
            }
            if (!PlayerData.ActualPlayerData.isFTUE)
            {
                GenerateSlots();
                _Deck.CheckHand();
                _Deck.ArrangeCards();
            }

            if (FTUEManager.instance && FTUEManager.instance.state < FTUEState.Reward)
            {
                PathEventManager.InvokeOnMonsterKilled((int)nMonsterKilled);
            }
            if (FTUEManager.instance && (FTUEManager.instance.state == FTUEState.FirstAugment || FTUEManager.instance.state == FTUEState.AugmentHand))
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (i < slots.Count) Destroy(slots[i]);
                }
                FTUEManager.instance.ChangePopUp();
                _AugmentHandler.Show();
            }
            /*else
            {
                PathEventManager.InvokeOnMonsterKilled((int)nMonsterKilled);
            }*/


            if (_MonsterDestroyed && nMonsterKilled == 1 && !PlayerData.ActualPlayerData.isFTUE)
            {
                nMonsterKilledTemp = 0;
                _AugmentHandler.Show();
                lCharrette.gameObject.SetActive(false);
            }
            PathEventManager.InvokeMonsterKilledRatio(GetMonsterKilledToAugmentRatio());

            if (_MonsterDestroyed)
            {
                _MonsterSOsInPath.RemoveAt(0);
                _MonsterDestroyed = false;
            }
            if (_NSlotToDelete > 0)
            {
                while (_Ratio < 1)
                {
                    _Ratio += Time.deltaTime * _MoveSpeed;
                    charette.position = Vector2.Lerp(new Vector3(_CharetteDir.x, charette.position.y), _CharetteStartPose, _Ratio);

                    yield return null;
                }
            }
            lCharrette.StopMovement();

            if (FTUEManager.instance && FTUEManager.instance.popEnum == PopUpEnum.EndTurn)
            {
                FTUEManager.instance.ChangePopUp();
            }
            _PathIsActivated = false;
            StopCoroutine(_Coroutine);
        }

        private void ActivateCharette() => charette.gameObject.SetActive(true);

        #region SLOT

        /// <summary>
        /// Generate next slots
        /// </summary>
        private void GenerateSlots()
        {
            for (int i = 0; i < _NSlotToDelete; i++)
            {
                SpawnSlot();
            }
        }
        /// <summary>
        /// Destroy Cards we need to delete
        /// </summary>
        private void UpdateSlots()
        {
            int lCardsToReturn = _Deck.cardsPlayed;
            List<CardSO> lCardToDelete = new List<CardSO>();
            int lBossSkipped = 0;

            for (int i = 0; i < _NSlotToDelete; i++)
            {
                if (slots[i].GetComponent<Slot>().isUsed) 
                    lCardToDelete.Add(_CheckCardsSlots.cardsInSlot[i - lBossSkipped]);
                else lBossSkipped++;
            }
            if (!PlayerData.ActualPlayerData.isFTUE) _Deck.MoveCardsFromListToList(lCardToDelete, _Deck.cemetary, lCardToDelete.Count);
            
            DeleteSlots();

            Slot lSlotScript = null;
            foreach (GameObject lSlot in slots)
            {
                lSlotScript = lSlot.GetComponent<Slot>();
                if (lSlotScript.isUsed)
                {
                    _CardsWheatValue += lSlotScript.card.GetComponent<PlayableCard>().wheatValue;
                    _CardsAttackValue += lSlotScript.card.GetComponent<PlayableCard>().attackValue;                
                }
            }

            _StatsManager.StoreTempValues();
            UpdateHUD();

            if (!PlayerData.ActualPlayerData.isFTUE) _Deck.UpdateHand(lCardsToReturn);
        }
        /// <summary>
        /// Detect and destroy slot to delete
        /// </summary>
        private void DeleteSlots()
        {
            int lAmountNightEggs = 0;
            int lAmountMorningEggs = 0;
            int lSlotChild = 0;
            Slot lSlotScript = null;
            PlayableCard lPlayableCard = null;
            GameObject lSlot = null;
            GameObject lCard = null;
            for (int i = 0; i < _NSlotToDelete; i++)
            {
                if (_CheckCardsSlots.cardsInSlot.Count > 0) _CheckCardsSlots.cardsInSlot.RemoveAt(0);
                lSlotChild = slots[i].transform.childCount;
                lSlot = slots[i];
                lSlotScript = lSlot.GetComponent<Slot>();

                for (int j = 0; j < lSlotChild; j++)
                {
                    lPlayableCard = lSlot?.transform?.GetChild(j)?.GetComponent<PlayableCard>();
                    lCard = lSlot.transform.GetChild(j).gameObject;

                    // Save removed card's value
                    if (lPlayableCard)
                    {
                        if (lSlotScript.dayPart == DayPart.Night && _AugmentHandler.ContainMagicMole(DayPart.Night))
                            lAmountNightEggs += 1;
                        else _CardsWheatValue += lPlayableCard.wheatValue;

                        if (lSlotScript.dayPart == DayPart.Morning && _AugmentHandler.ContainMagicMole(DayPart.Morning))
                            lAmountMorningEggs += 1;
                        else _CardsAttackValue += lPlayableCard.attackValue;

                        lPlayableCard.RemoveFromHand();
                    }

                    if (lPlayableCard && _Deck.cardsAmount < _Deck.handSize)
                    {
                        lCard.transform.SetParent(_Deck.transform);
                        break;
                    }
                    else Destroy(lCard.gameObject);
                }

                
                Destroy(lSlot);
            }
            PathEventManager.InvokeUpdateEgg(DayPart.Night, lAmountNightEggs);
            PathEventManager.InvokeUpdateEgg(DayPart.Morning, lAmountMorningEggs);
        }
        /// <summary>
        /// Save values of removed cards of path in HUD
        /// </summary>
        private void UpdateHUD()
        {
            _StatsManager.WheatValue = 0;
            _StatsManager.AttackValue = 0;
            _StatsManager.startWheatValue += _CardsWheatValue;
            _StatsManager.startAttackValue += _CardsAttackValue;

            _StatsManager.WheatValue += _StatsManager.startWheatValue;
            _StatsManager.AttackValue += _StatsManager.startAttackValue;

            CheckWheat();
            _StatsManager.UpdateHUD();

        }
        private void SpawnSlot(bool pIsNerf = false)
        {
            _Slot = Instantiate(_SlotPrefab, transform);
            _Slot.name = _TotalSlotSpawn.ToString();
            Slot lSlotScript = _Slot.transform.GetComponent<Slot>();

            if (pIsNerf) lSlotScript.boostValue = -1;
            ++_TotalSlotSpawn;

            if (_TotalSlotSpawn % _NSlotToSpawnMonster == 0 && _SpawnMonster)
            {
                lSlotScript.monster = PlayerData.ActualPlayerData.isFTUE ? _FTUEMonster : GenerateMonster();
                lSlotScript.UpdateSlot(true, 1);
            }
            else
            {
                lSlotScript.UpdateSlot(false, 1);
                lSlotScript.dayPart = (DayPart)(_NormalSlotAlreadySpawn % (Addons_Enum.GetEnumCount<DayPart>()-1));
                ++_NormalSlotAlreadySpawn;
                lSlotScript.SetDayPartImage();
            }
        }
        #endregion

        #region MONSTER
        public Monster GenerateMonster()
        {
            int lLevel = _NMonsterAlreadySpawn % _EachNMonsterSpawnBoss == 0 && _NMonsterAlreadySpawn > 1 ? Mathf.Clamp((int)nStep, 0, bossDB.Count) : Mathf.Clamp((int)nStep, 0, monsterDB.Count);
            //lLevel = 1; // Because Monster start at Level 1 and MonsterDB at index 0
            MonsterSO lMonster = _NMonsterAlreadySpawn % _EachNMonsterSpawnBoss == 0 && _NMonsterAlreadySpawn > 1 ? bossDB[lLevel][UnityEngine.Random.Range(0, bossDB[lLevel].Count)].monsterSO : monsterDB[lLevel][UnityEngine.Random.Range(0, monsterDB[lLevel].Count)].monsterSO;
            _MonsterSOsInPath.Add(lMonster);
            _NMonsterAlreadySpawn++;
            _TotalSlotSpawn = 0;
            return PlayerData.ActualPlayerData.isFTUE ? _FTUEMonster : new Monster(lMonster);
        }

        private void GenerateFTUEPath(StepEvent pStepEvent, FTUEState pFTUEState)
        {
            for (int i = 0; i < _NSlotToDelete; i++)
            {
                if (i < slots.Count) Destroy(slots[i]);
            }

            MonsterSO lMonster = pFTUEState >= FTUEState.FirstAugment && pFTUEState < FTUEState.Menu ? pStepEvent.augmentToSpawn[FTUEManager.instance.augmentChooseIndex].monsterToSpawn : pStepEvent._MonsterToSpawn;
            _FTUEMonster = new Monster(lMonster);
            _MonsterSOsInPath.Add(lMonster);

            _TotalSlotSpawn = 0;
            _NormalSlotAlreadySpawn = 0;
            _NSlotToSpawn = pStepEvent.nSlotToSpawn;
            _NSlotToSpawnMonster = pStepEvent.nSlotToSpawn;
            for (int i = 0; i < _NSlotToSpawn; i++)
            {
                SpawnSlot();
            }
        }

        private void AddMonstersInDB()
        {
            List<Monster> lMonsters;
            List<Monster> lMonstersTempDB = new List<Monster>();
            lMonstersTempDB.GetScriptableListFromFolder<Monster, MonsterSO>(MONSTER_DIRECTORY_PATH, true);
            int lMaxList = GetMaxMonsterLevel(lMonstersTempDB);

            for (int i = 1; i <= lMaxList; i++)
            {
                lMonsters = new List<Monster>(lMonstersTempDB.FindAll(lMonster => lMonster.monsterSO.level == i));
                monsterDB.Add(lMonsters);
            }

            // Boss
            lMonstersTempDB.GetScriptableListFromFolder<Monster, MonsterSO>(BOSS_DIRECTORY_PATH, true);
            lMaxList = GetMaxMonsterLevel(lMonstersTempDB);
            for (int i = 1; i <= lMaxList; i++)
            {
                lMonsters = new List<Monster>(lMonstersTempDB.FindAll(lMonster => lMonster.monsterSO.level == i));
                bossDB.Add(lMonsters);
            }
        }

        private int GetMaxMonsterLevel(List<Monster> pList)
        {
            int lMaxLevel = 1;
            foreach (Monster lMonster in pList)
            {
                if (lMonster.monsterSO.level > lMaxLevel) lMaxLevel = lMonster.monsterSO.level;
            }
            return lMaxLevel;
        }
        #endregion

        private void CheckWheat()
        {
            if (_StatsManager.WheatValue <= 0)
            {
                if (_LastChance) _LastChance = false;
                else _StatsManager.HealthValue -= 1;
            }
        }
        public void UpdateMonstersSaved()
        {
            monstersSaved.Clear();
            Slot lSlot;
            MonsterSaveInSlot lCard;
            int lCounter = 0; 
            for (int i = 0; i < slots.Count; i++)
            {
                lSlot = slots[i].GetComponent<Slot>();
                if (!lSlot.btn.interactable)
                {
                    lCard = new MonsterSaveInSlot();
                    lCard.slotIndex = i;
                    lCard.position = lSlot.GetComponentInChildren<MonsterCard>().transform.localPosition;
                    lCard.monsterID = _MonsterSOsInPath[lCounter].ID;
                    lCard.monsterLevel = _MonsterSOsInPath[lCounter].level;
                    lCard.isBoss = _MonsterSOsInPath[lCounter].isBoss;
                    monstersSaved.Add(lCard);
                    lCounter++;
                }
            }
        }

        private void SendEvent(GetInfo pEvent, List<object> pValues)
        {
            foreach (ParametersData lParameter in pEvent.eventData.parameters)
            {
                pEvent.GetValue(lParameter.paramName, pValues[pEvent.eventData.parameters.IndexOf(lParameter)]);
            }
            pEvent.SendEvent();
        }

        private Monster LoadMonsters(MonsterSO pMonsteSO) => new Monster(pMonsteSO);

        #region DATA
        public void SaveData()
        {
            GameStateData.ActualGameStateData.nSlotToSpawn = slots.Count;
            GameStateData.ActualGameStateData.nMonsterKilled = nMonsterKilled;
            GameStateData.ActualGameStateData.nMonsterKilledTemp = nMonsterKilledTemp;
            GameStateData.ActualGameStateData.nMonsterAlreadySpawn = _NMonsterAlreadySpawn;
            GameStateData.ActualGameStateData.nStep = nStep;
            GameStateData.ActualGameStateData.totalSlotSpawn = _TotalSlotSpawn;
            GameStateData.ActualGameStateData.indNewCardSlot = _IndNewCardSlot;
            GameStateData.ActualGameStateData.normalSlotAlreadySpawn = _NormalSlotAlreadySpawn;
            GameStateData.ActualGameStateData.dayPassed = _DayPassed;
            UpdateMonstersSaved();
            GameStateData.ActualGameStateData.monsterSaveInSlots = new List<MonsterSaveInSlot>(monstersSaved);

            PlayerData.ActualPlayerData.bestStep = nStep > PlayerData.ActualPlayerData.bestStep ? nStep : PlayerData.ActualPlayerData.bestStep;

            GameStateData.ActualGameStateData.dayPartInSlot.Clear();
            foreach (GameObject lSlot in slots)
            {
                GameStateData.ActualGameStateData.dayPartInSlot.Add(lSlot.GetComponent<Slot>().dayPart);
            }

        }
        public void LoadData()
        {
            _NSlotToSpawn = (uint)GameStateData.ActualGameStateData.nSlotToSpawn;
            nMonsterKilled = GameStateData.ActualGameStateData.nMonsterKilled;
            nMonsterKilledTemp = GameStateData.ActualGameStateData.nMonsterKilledTemp;
            _NMonsterAlreadySpawn = GameStateData.ActualGameStateData.nMonsterAlreadySpawn;

            _MonsterSOsInPath = new List<MonsterSO>(ListAddons.GetMonsterSOListFromDB(GameStateData.ActualGameStateData.monsterSaveInSlots));

            nStep = GameStateData.ActualGameStateData.nStep;
            _TotalSlotSpawn = GameStateData.ActualGameStateData.totalSlotSpawn;
            _IndNewCardSlot = GameStateData.ActualGameStateData.indNewCardSlot;
            _NormalSlotAlreadySpawn = GameStateData.ActualGameStateData.normalSlotAlreadySpawn;
            _DayPassed = GameStateData.ActualGameStateData.dayPassed;

            _GameManager.hud.monsterKilledCount.text = nMonsterKilled.ToString();
            if (_GameManager.hud.stepCounter && _GameManager.hud.stepCounter.text != null) _GameManager.hud.stepCounter.text = nStep.ToString();
            PathEventManager.InvokeMonsterKilledRatio(GetMonsterKilledToAugmentRatio());

            monstersSaved = new List<MonsterSaveInSlot>(GameStateData.ActualGameStateData.monsterSaveInSlots);

        }
        #endregion

        private void OnDestroy()
        {
            PathEventManager.saveData -= SaveData;
            PathEventManager.onFTUEStepChanged -= GenerateFTUEPath;
            PathEventManager.onAugmentChoose -= ActivateCharette;
            _Coroutine = null;
            if (instance != null) instance = null;
        }
    }
}