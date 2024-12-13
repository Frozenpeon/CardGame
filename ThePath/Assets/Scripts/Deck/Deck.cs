using System;
using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.F2P.Game;
using Com.IsartDigital.F2P.Game.FTUE;
using Com.IsartDigital.F2P.SO.CardSO;
using Com.IsartDigital.F2P.Utils;
using UnityEngine;

//Author : Julian Martin
namespace Com.IsartDigital.F2P
{
    public class Deck : MonoBehaviour, ISavedGameElement
    {
        static public Deck instance;

        //Only visible if isFTUE is true
        public bool isFTUE;
        public List<CardConfig> cardConfigList = new();

        /*[HideInInspector]*/ public List<CardSO> pool = new List<CardSO>();
        /*[HideInInspector]*/ public List<CardSO> deck = new List<CardSO>();
        /*[HideInInspector]*/ public List<CardSO> cemetary = new List<CardSO>();
        /*[HideInInspector]*/ public List<PlayableCard> actualHand = new List<PlayableCard>();
        public GameObject cardPrefab;
        public int poolSize;
        public int handSize;
        public float cardSpacing = 30f;
        private Vector2 _CardSize;
        public int cardsAmount => CardsAmount();
        public int cardsPlayed => CardsPlayed();

        [Serializable] public class CardConfig
        {
            public CardSO CardSO;
            public int Amount;
        }
        public uint nMulligan = 5;

        [SerializeField] private GameObject _ChangeHand, _WatchForMulligan;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            _CardSize = cardPrefab.GetComponent<RectTransform>().sizeDelta;
        }

        private void Start()
        {
            if (!PlayerData.ActualPlayerData.isFTUE)
            {
                InitializePool();
                InitializeHand();
                ArrangeCards();
            }
            
            PathEventManager.saveData += SaveData;
            PathEventManager.loadData += LoadData;
            GameStateChanges.removedFromHand += RemoveCardFromHandList;
            PathEventManager.updateMulligan += UpdateMulligan;
            PathEventManager.onFTUEStepChanged += UpdateHandFTUE;

            _WatchForMulligan.SetActive(false);
            _ChangeHand.SetActive(true);
        }

        private void OnDestroy()
        {
            PathEventManager.saveData -= SaveData;
            PathEventManager.loadData -= LoadData;
            PathEventManager.updateMulligan -= UpdateMulligan;
            PathEventManager.onFTUEStepChanged -= UpdateHandFTUE;
            Destroy(gameObject);
        }


        private void InitializePool()
        {
            if (!isFTUE)
            {
                int lIndex;
                int lCardDBCount = CardDB.cardDatabaseList.Count;

                for (int i = 0; i < poolSize; i++) //generates the pool based on the database
                {
                    lIndex = UnityEngine.Random.Range(0, lCardDBCount);
                    pool.Add(CardDB.cardDatabaseList[lIndex].cardSO);
                }
            }
            else
            {
                if (cardConfigList.Count > 0)
                {
                    for (int i = 0; i < cardConfigList.Count; i++)
                    {
                        for (int j = 0; j < cardConfigList[i].Amount; j++)
                        {
                            pool.Add(cardConfigList[i].CardSO);
                        }
                    }
                }
            }
        }

        private void InitializeHand()
        {
            for (int i = 0; i < handSize; i++) //take card out of the deck and put it in the hand
            {
                CreateCard();
            }
            UpdateHand(handSize);
            if (!GameStateData.ActualGameStateData.isFirstTimeLaunch)
            {
                StartCoroutine(DelayedUpdateCardsUnlockedText());
            }



            //if (GameStateData.ActualGameStateData.isFristTimeLaunch)
            //{
            //    UpdateHand(handSize);
            //}
            //else
            //{
            //    UpdateLoadedCards();
            //}
        }

        /// <summary>
        /// Arrange in a line all cards that are children of the deck 
        /// </summary>
        public void ArrangeCards()
        {
            print("AAAAAAAAAAAAAAAAAAAAA");
            float cardsNumber = transform.childCount;
            float cardsTotalSize = cardsNumber * (_CardSize.x + cardSpacing) - cardSpacing;
            float startX = (GetComponent<RectTransform>().rect.width - cardsTotalSize) / 2 + _CardSize.x/2; //Added CardSize / 2 to recenter the offset
            for (int i = 0; i < cardsNumber; i++)
            {
                Transform lCard = transform.GetChild(i);
                lCard.GetChild(0).GetComponent<MovableCard>().canMove = true;
                float lNewX = startX + i * (_CardSize.x + cardSpacing);
                lCard.GetComponent<RectTransform>().anchoredPosition = new(lNewX, GetComponent<RectTransform>().rect.height/2);
            }
        }

        private void CreateCard()
        {
            GameObject lCard = Instantiate(cardPrefab, transform);
            lCard.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Change cards to the next one in the pool
        /// </summary>
        /// <param name="pCardsToUpdate">Amount of cards to be replaced starting from the end of the deck</param>
        public void UpdateHand(int pCardsToUpdate)
        {
            CheckHand();
            ShufflePool();
            MoveCardsFromListToList(pool, deck, pCardsToUpdate);
            GameObject gO;
            PlayableCard playCard;
            for (int i = handSize - pCardsToUpdate; i < handSize; i++)
            {
                gO = transform.GetChild(i).gameObject;
                playCard = gO.GetComponent<PlayableCard>();
                CardDisplay lCard = transform.GetChild(i).gameObject.GetComponent<CardDisplay>();
                actualHand.Add(playCard);
                GameStateChanges.InvokeAddToHand(playCard);
                ChangeCard(lCard, i); 
                PathEventManager.InvokeOnCardDraw();
            }
        }

        /// <summary>
        /// Make sure the deck's size is equal to the number of cards currently in the hand
        /// </summary>
        public void CheckHand()
        {
            print("AAA");
            if (cardsAmount < handSize)
            {
                int lCardNumberDiff = handSize - cardsAmount;
                for (int i = 0; i < lCardNumberDiff; i++)
                {
                    CreateCard();
                }
            }
        }

        public void MoveCardsFromListToList(List<CardSO> pFromList, List<CardSO> pToList, int pCardsAmount)
        {
            for (int i = 0; i < pCardsAmount; i++)
            {
                pToList.Add(pFromList[0]);
                pFromList.RemoveAt(0);
            }
        }

        public void ShufflePool()
        {
            System.Random lRNG = new System.Random(); // Instantiate a random number generator
            int lPoolCardsAmount = pool.Count;
            int lNextCard;
            while (lPoolCardsAmount > 1)
            {
                lPoolCardsAmount--;
                lNextCard = lRNG.Next(lPoolCardsAmount + 1);
                (pool[lPoolCardsAmount], pool[lNextCard]) = (pool[lNextCard], pool[lPoolCardsAmount]);
            }
        }

        public void ChangeEntireHand()
        {
            --nMulligan;

            ReturnAllCardInDeck();

            if (FTUEManager.instance && FTUEManager.instance.state == FTUEState.RandomAugment)
            {
                deck.Clear();
                List<CardSO> lTempPool = new List<CardSO>();
                foreach (CardConfig lCard in FTUEManager.instance.currentStep.mulliganConfig[FTUEManager.instance.augmentChooseIndex].cardDraw)
                {
                    for (int i = 0; i < lCard.Amount; i++)
                    {
                        lTempPool.Add(lCard.CardSO);
                    }
                }
                pool = new List<CardSO>(lTempPool);
            }
            else
            {
                MoveCardsFromListToList(cemetary, pool, cemetary.Count);
                MoveCardsFromListToList(deck, pool, handSize);
                ShufflePool();
            }

           
            MoveCardsFromListToList(pool, deck, handSize);
            for (int i = 0; i < handSize; i++)
            {
                CardDisplay lCard = transform.GetChild(i).gameObject.GetComponent<CardDisplay>();
                ChangeCard(lCard, i);
            }

            if (nMulligan <= 0)
            {
                _WatchForMulligan.SetActive(true);
                _ChangeHand.SetActive(false);
            }
            else
            {
                _WatchForMulligan.SetActive(false);
                _ChangeHand.SetActive(true);
            }
        }

       
        public void RemoveCardFromHandList(PlayableCard pCard)
        {
            actualHand.Remove(pCard);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                UpdateLoadedCards();
            }
        }

        private IEnumerator DelayedUpdateCardsUnlockedText()
        {
            yield return null; // Wait for the end of the frame to ensure all scripts are loaded
            UpdateLoadedCards();
        }

        public void UpdateLoadedCards()
        {
            //Load pool
            //load deck
            int lCount = deck.Count; // If the player had x card in his hand
            for (int i = 0; i < lCount; i++)
            {
                if (i < transform.childCount) transform.GetChild(i).GetComponent<CardDisplay>().LoadInfos(deck[i].ID);
            }
        }

        private void UpdateHandFTUE(StepEvent pStepEvent, FTUEState pFTUEState)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            handSize = (int)pStepEvent.nCardInHand;
            deck.Clear();
            pool.Clear();
            actualHand.Clear();

            if (pFTUEState >= FTUEState.FirstAugment && pFTUEState < FTUEState.Menu)
            {
                FTUEGenerateHand(pStepEvent.augmentToSpawn[FTUEManager.instance.augmentChooseIndex].cardInHand);
            }
            else if (pFTUEState < FTUEState.FirstAugment)
            {
                FTUEGenerateHand(pStepEvent.cardInHand);
            }
            InitializeHand();
            ArrangeCards();
            StartCoroutine(DelayedUpdateCardsUnlockedText());
        }

        private void FTUEGenerateHand(List<CardConfig> pList)
        {
            foreach (CardConfig lCard in pList)
            {
                for (int i = 0; i < lCard.Amount; i++)
                {
                    pool.Add(lCard.CardSO);
                }
            }
        }

        /// <summary>
        /// Change a single card to the next one in the pool
        /// </summary>
        /// <param name="pCardDisplay"></param>
        public void ChangeCard(CardDisplay pCardDisplay, int pIndex)
        {
            pCardDisplay.displayID = deck[pIndex].ID;
            pCardDisplay.LoadInfos(pCardDisplay.displayID);
            GameStateChanges.InvokeAddToHand(pCardDisplay.GetComponent<PlayableCard>());
        }

        public Vector2 GetCardPosition(int pCardIndex)
        {
            float cardsNumber = transform.childCount;
            float cardsTotalSize = cardsNumber * (_CardSize.x + cardSpacing) - cardSpacing;
            float startX = (GetComponent<RectTransform>().rect.width - cardsTotalSize) / 2 + _CardSize.x / 2; //Added CardSize / 2 to recenter the offset
            float lNewX = startX + pCardIndex * (_CardSize.x + cardSpacing);
            return new Vector2(lNewX, GetComponent<RectTransform>().rect.height / 2);
        }

        public void SortDeckList()
        {
            List<CardSO> sortedCards = new List<CardSO>();

            foreach (Transform card in transform)
            {
                CardDisplay cardDisplay = card.GetComponentInChildren<CardDisplay>();
                if (cardDisplay != null)
                {
                    sortedCards.Add(card.GetComponentInChildren<MovableCard>()._CardSO);
                }
            }

            deck = sortedCards;
        }

        private void ReturnAllCardInDeck()
        {
            if (cardsAmount != handSize)
            {
                foreach (Transform card in Manager.Path.instance.transform)
                {
                    MovableCard lMovableCard = card.GetComponentInChildren<MovableCard>();
                    if (lMovableCard != null && lMovableCard.canMove)
                    {
                        lMovableCard.ResetSlot();
                        lMovableCard.ReturnToDeck();
                        GameStateChanges.InvokeAddToHand(card.GetComponent<PlayableCard>());
                    }
                }
                SortDeckList();
            }
        }

        private int CardsPlayed()
        {
            return handSize - cardsAmount;
        }

        private int CardsAmount()
        {
            return transform.childCount;
        }


        public void RemoveACardFromPool(CardType pCardType)
        {
            CardSO lCardToRemove = pool.Find(x => x.cardType == pCardType);
            pool.Remove(lCardToRemove);
        }

        private void UpdateMulligan(int pValueToAdd)
        {
            PathEventManager.InvokeOnBtnMulligan();
            nMulligan += (uint)pValueToAdd;
            ChangeEntireHand();
        }

        public string GetCardsNameInDeck()
        {
            string lString = "";

            foreach (PlayableCard lCards in actualHand)
            {
                lString += lCards.startCardDisplay.cardSO.name + " | ";
            }
            return lString;
        }
        public void SaveData()
        {
            ReturnAllCardInDeck();
            GameStateData.ActualGameStateData.pool = ListAddons.GetIDList(pool);
            GameStateData.ActualGameStateData.deck = ListAddons.GetIDList(deck);
            GameStateData.ActualGameStateData.cemetary = ListAddons.GetIDList(cemetary);
        }
        public void LoadData()
        {
            pool = ListAddons.GetCardSOListFromDB<CardSO>(GameStateData.ActualGameStateData.pool);
            deck = ListAddons.GetCardSOListFromDB<CardSO>(GameStateData.ActualGameStateData.deck);
            cemetary = ListAddons.GetCardSOListFromDB<CardSO>(GameStateData.ActualGameStateData.cemetary);
        }        
    }
}