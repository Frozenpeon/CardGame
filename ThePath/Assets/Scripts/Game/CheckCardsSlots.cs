using Com.IsartDigital.F2P.Cards;
using Com.IsartDigital.F2P.Manager;
using Com.IsartDigital.F2P.SO.CardSO;
using Com.IsartDigital.F2P.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P.Game.Slot
{

    [Serializable] public class CardSaveInSlot
    {
        public int cardID;
        public int slotIndex;
        public Vector3 position;
    }

    // By Matteo Renaudin

    public class CheckCardsSlots : MonoBehaviour, ISavedGameElement
    {
        public List<CardSO> cardsInSlot = new List<CardSO>();
        public List<CardSaveInSlot> cardsSaved = new List<CardSaveInSlot>();
        private Path _Path => Path.instance;
        private Deck _Deck => Deck.instance;
        private StatsManager _StatsManager => StatsManager.instance;
        #region SINGLETON

        private static CheckCardsSlots Instance;
        public static CheckCardsSlots instance
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
            PathEventManager.saveData += SaveData;
            PathEventManager.loadData += LoadData;
        }
        public List<PlayableCard> GetAffectedCards(PlayableCard pCard, CardAffected pCardsAffected)
        {
            List<PlayableCard> lList = new List<PlayableCard>();
            int lCardIndex = _Path.slots.IndexOf(pCard.transform.parent.gameObject);

            switch (pCardsAffected)
            {
                case CardAffected.Left:
                    lList.Add(GetNextCard(lCardIndex, true));
                    break;

                case CardAffected.Right:
                    lList.Add(GetNextCard(lCardIndex, false));
                    break;

                case CardAffected.Adjacent:
                    lList.Add(GetNextCard(lCardIndex, true));
                    lList.Add(GetNextCard(lCardIndex, false));
                    break;
                case CardAffected.Path:

                    PlayableCard lCard = null;
                    foreach (GameObject lSlot in _Path.slots)
                    {
                        lCard = lSlot?.GetComponent<Cards.Slot>()?.card?.GetComponent<PlayableCard>();
                        if (lCard != null) lList.Add(lCard);
                    }
                    break;
                case CardAffected.Self:
                    lList.Add(pCard);
                    break;
                default:
                    break;
            }
            return lList;
        }
        public PlayableCard GetNextCard(int pCardIndex, bool pIsLeft)
        {
            int lNextIndex = pIsLeft ? pCardIndex - 1 : pCardIndex + 1;
            bool lCondition = pIsLeft ? pCardIndex > 0 : pCardIndex < _Path.slots.Count - 1;

            GameObject lCard = lCondition ? _Path.slots[lNextIndex]?.GetComponent<Cards.Slot>()?.card : null;

            if (lCondition && lCard != null)
                return lCard.GetComponent<PlayableCard>();
            else 
                return null;
        }

        public void UpdateCards()
        {
            PlayableCard lCard = null;
            _StatsManager.ResetHUD();

            // Reset Card's values in path
            GoThroughSlotList(lCard, true);

            // Reset Card's values in deck
            foreach (GameObject lCardDeck in _Deck.transform.GetChildren())
            {
                lCard = lCardDeck.GetComponent<PlayableCard>();

                if (lCard == null) continue;
                lCard.boostList.Clear();
                lCard.GetValues();
            }
            // Update Cards

            GoThroughSlotList(lCard, false, true); // Activate Boostcard only to setup all cards with boost
            GoThroughSlotList(lCard, false, false); // Activate Other cards
            PathEventManager.InvokeOnCardUpdated();
            EventManager.CardPlayed(new CardEffect(0, 0, _StatsManager.startAttackValue, _StatsManager.startWheatValue));
        }

        private void GoThroughSlotList(PlayableCard pCard, bool pIsReset, bool pIsOnlyBoost = true)
        {
            Cards.Slot lSlotScript = null;
            foreach (GameObject lSlot in _Path.slots)
            {
                lSlotScript = lSlot?.GetComponent<Cards.Slot>();
                pCard = lSlotScript?.card?.GetComponent<PlayableCard>();

                if (pCard == null) continue;
                //pCard.BoostValueDependOfTime(lSlotScript.dayPart);
                if (pIsReset) ResetCardInSlot(pCard);
                else UpdateCard(pCard, pIsOnlyBoost);

                if (pCard.ContainBlessingInBoost())
                {
                    pCard.cardVFX.blessingParticle.gameObject.SetActive(true);
                    pCard.cardVFX.blessingEffect.SetActive(true);
                }
                else
                {
                    pCard.cardVFX.blessingEffect.SetActive(false);
                    pCard.cardVFX.blessingParticle.gameObject.SetActive(false);
                }
                }
        }

        private void ResetCardInSlot(PlayableCard pCard)
        {
            pCard.boostList.Clear();
            if (pCard.startCardDisplay.cardSO.cardType == CardType.Boost)
                pCard.cardDisplay = pCard.startCardDisplay;
        }
        private void UpdateCard(PlayableCard pCard, bool pOnlyBoost)
        {
            if (pCard.startCardDisplay.cardSO.cardType == CardType.Boost && pOnlyBoost) pCard.PlayEffect();
            if (!pOnlyBoost) pCard.PlayCard();
        }

        public bool CanBeatMonster(MonsterSO pCurrentMonster, int pMonsterIndex) => _StatsManager.AttackValue >= Mathf.Clamp(pCurrentMonster.attackRemoved - AugmentHandler.Instance.GetDamageReduction() - _Path.RemovedPowerOnSlotIfDecoy(pMonsterIndex), 0, pCurrentMonster.attackRemoved) && CanBeatMonsterWheat(pCurrentMonster, pMonsterIndex);
        public bool CanBeatMonsterWheat(MonsterSO pCurrentMonster, int pMonsterIndex) => _StatsManager.WheatValue >= Mathf.Clamp(pCurrentMonster.wheatRemoved - AugmentHandler.Instance.GetDamageReduction() - _Path.RemovedPowerOnSlotIfDecoy(pMonsterIndex), 0, pCurrentMonster.wheatRemoved);
        public void SortCardsOnSlots()
        {
            List<CardSO> lSortedCards = new List<CardSO>();
            CardDisplay lCardDisplay;

            foreach (GameObject lSlot in _Path.slots)
            {
                lCardDisplay = lSlot.GetComponentInChildren<CardDisplay>();

                if (lCardDisplay != null)
                    lSortedCards.Add(lSlot.GetComponentInChildren<MovableCard>()._CardSO);
            }
            cardsInSlot = lSortedCards;
        }

        public string GetCardsNameInSlot()
        {
            string lString = "";

            foreach (CardSO lCards in cardsInSlot)
            {
                lString += lCards.name + " | ";
            }
            return lString;
        }

        public void UpdateCardsSaved()
        {
            cardsSaved.Clear();
            Cards.Slot lSlot;
            CardSaveInSlot lCard;
            for (int i = 0; i < _Path.slots.Count; i++)
            {
                lSlot = _Path.slots[i].GetComponent<Cards.Slot>();
                if (lSlot.isUsed)
                {
                    lCard = new CardSaveInSlot();
                    lCard.slotIndex = i;
                    lCard.position = lSlot.GetComponentInChildren<PlayableCard>().transform.localPosition;
                    lCard.cardID = lSlot.GetComponentInChildren<PlayableCard>().startCardDisplay.cardSO.ID;
                    if (!lSlot.GetComponentInChildren<PlayableCard>().GetComponentInChildren<MovableCard>().canMove)
                    {
                        cardsSaved.Add(lCard);
                    }
                }
            }
        }

        public void SaveData()
        {
            UpdateCardsSaved();
            GameStateData.ActualGameStateData.cardSaveInSlots = new List<CardSaveInSlot>(cardsSaved);
        }        
        public void LoadData()
        {
            // TODO
            foreach (CardSaveInSlot lSavedCard in GameStateData.ActualGameStateData.cardSaveInSlots)
            {
                if (lSavedCard.slotIndex < _Path.slots.Count)
                {
                    GameObject lCard = Instantiate(_Deck.cardPrefab, _Path.slots[lSavedCard.slotIndex].transform);
                    lCard.transform.localPosition = lSavedCard.position;
                    CardDisplay lCardDisplay = lCard.GetComponent<CardDisplay>();
                    lCard.GetComponent<CardDisplay>().GetComponentInChildren<MovableCard>().canMove = false;

                    lCardDisplay.LoadInfos(lSavedCard.cardID);
                    cardsInSlot.Add(CardDB.cardDatabaseList.Find(x => x.cardSO.ID == lSavedCard.cardID).cardSO);
                }
            }
        }
        private void OnDestroy()
        {
            PathEventManager.saveData -= SaveData;
            PathEventManager.loadData -= LoadData;
            if (instance != null) instance = null;
        }
    }
}