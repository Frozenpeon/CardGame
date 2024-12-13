using Com.IsartDigital.F2P.Cards;
using Com.IsartDigital.F2P.Cards.Boosts;
using Com.IsartDigital.F2P.Game.Slot;
using Com.IsartDigital.F2P.SO.CardSO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author : Julian Martin
namespace Com.IsartDigital.F2P
{
    [Serializable] public class PlayableCard : MonoBehaviour
    {
        public Sprite _CropFieldImage;
        public Sprite _SharpeningImage;
        [HideInInspector] public Sprite startImage;

        [SerializeField] private int _MaxHealthValue;
        [SerializeField] private int _HealthValue;
        public int attackValue;
        public int wheatValue;
        public Card cardDisplay = null;
        public Card startCardDisplay = null;
        public Boost boost = null;

        public CardVFX cardVFX = null;
        public CardType cardType;

        public int attackBonus = 0;
        public int wheatBonus = 0;
        public int healthBonus = 0;

        public int upPartDayAttackBonus => BoostValueDependOfTime(transform.parent.GetComponent<Slot>() ? transform.parent.GetComponent<Slot>().dayPart : DayPart.Default);
        public int upPartDayWheatBonus => BoostValueDependOfTime(transform.parent.GetComponent<Slot>() ? transform.parent.GetComponent<Slot>().dayPart : DayPart.Default);

        public bool hasBeenInvestmentedBuffed = false;

        private int _BoostValue => GetBoostValue();
        private int _SlotBoostValue => transform.parent.GetComponent<Slot>() ? transform.parent.GetComponent<Slot>().boostValue : 0;


        public List<Boost> boostList = new List<Boost>();

        private CheckCardsSlots _CheckCardsSlots => CheckCardsSlots.instance;
        private AugmentHandler _AugmentHandler => AugmentHandler.Instance;

        public bool isInPath = false;

        private void Start()
        {
            UpdateInfos();
            cardType = cardDisplay.cardSO.cardType;
        }

        public void UpdateInfos()
        {
            boost = null;
            cardDisplay = GetComponent<CardDisplay>().cardDisplay;
            ResetValue();
            GetValues();
            startCardDisplay = cardDisplay;
            if (cardDisplay != null && cardDisplay.cardSO != null && cardDisplay.cardSO.cardType == CardType.Boost)
            {
                BoostSO lBoost = cardDisplay.cardSO as BoostSO;
                switch (lBoost.specialBoost)
                {
                    case SpecialBoost.Buff:
                        boost = Boost.CreateBoost<Buff>(lBoost.effectType, lBoost.effectValue);
                        break;
                    case SpecialBoost.DailyProduction:
                        boost = DailyProduction.CreateBoost<DailyProduction>(lBoost.effectType, lBoost.effectValue);
                        break;
                    case SpecialBoost.Copy:
                        boost = Boost.CreateBoost<Reproduce>(lBoost.effectType, lBoost.effectValue);
                        break;
                    case SpecialBoost.DailyPrayer:
                        boost = Boost.CreateBoost<DailyPrayer>(lBoost.effectType, lBoost.effectValue);
                        break;
                    default:
                        break;
                }
            }
        }


        public bool ContainBlessingInBoost() => boostList.Find(x => x is Buff) != null;

        public void ResetValue()
        {
            boostList = new List<Boost>();
            attackValue = 0;
            _HealthValue = 0;
            wheatValue = 0;
        }

        public void GetValues()
        {
            switch (cardDisplay.cardSO.cardType)
            {
                case CardType.Sharpening:
                    attackValue = cardDisplay.cardSO.effectValue + _BoostValue + attackBonus + _SlotBoostValue + upPartDayAttackBonus;
                    break;
                case CardType.Life:
                    _HealthValue = cardDisplay.cardSO.effectValue + _BoostValue + healthBonus + _SlotBoostValue;
                    break;
                case CardType.CropField:
                    wheatValue = cardDisplay.cardSO.effectValue + _BoostValue + wheatBonus + _SlotBoostValue + upPartDayWheatBonus;
                    break;
                default:
                    break;
            }
        }

        public int GetBoostValue()
        {
            int lValue = 0;
            if (boostList.Count > 0)
            {
                foreach (Boost lBoost in boostList)
                {
                    lValue += lBoost.value;
                }
            }
            return lValue;
        }

        public int BoostValueDependOfTime(DayPart pDayPart)
        {
            switch (pDayPart)
            {
                case DayPart.Morning:
                    if (cardDisplay.cardSO.cardType == CardType.Sharpening)
                    {
                        return AddBonusValueWithAugment<AugmentSOUpPartDay>(cardDisplay.cardSO.cardType);
                    }
                    break;

                case DayPart.Day:                    
                    break;

                case DayPart.Night:
                    if (cardDisplay.cardSO.cardType == CardType.CropField)
                    {
                        return AddBonusValueWithAugment<AugmentSOUpPartDay>(cardDisplay.cardSO.cardType);
                    }
                    break;

                default:
                    break;
            }
            return 0;
        }

        private int AddBonusValueWithAugment<T>(CardType pType) where T : AugmentSO
        {
            T lAugment = _AugmentHandler.activAugments.Find(x => x is T) as T;

            if (lAugment != null)
            {
                if (lAugment is AugmentSOUpPartDay && (lAugment as AugmentSOUpPartDay).cardTypeBoosted == pType)
                {
                    if (pType == CardType.Sharpening) return (lAugment as AugmentSOUpPartDay).powerUpGrade;
                    else if (pType == CardType.CropField) return (lAugment as AugmentSOUpPartDay).powerUpGrade;                    
                }                
            }
            return 0;
        }

        public void PlayEffect()
        {
            //By Matteo Renaudin
            if (boost != null && cardDisplay.cardSO is BoostSO && isInPath)
            {
                List<PlayableCard> lCards = new List<PlayableCard>(_CheckCardsSlots.GetAffectedCards(this, ((BoostSO)cardDisplay.cardSO).cardAffected));
                boost.ActivateBoost(lCards, ((BoostSO)cardDisplay.cardSO).specialBoost, this);
            }
            GetValues();
        }

        public void PlayCard()
        {
            CardEffect effect = new CardEffect(
                maxHealth: _MaxHealthValue, 
                health: _HealthValue, 
                damage: attackValue, 
                wheat: wheatValue);
            EventManager.CardPlayed(effect);
        }

        public void RecoverCard()
        {
            CardEffect effect = new CardEffect(
                maxHealth: _MaxHealthValue * -1, 
                health: _HealthValue * -1, 
                damage: attackValue * -1, 
                wheat: wheatValue * -1);
            EventManager.CardPlayed(effect);
        }

        public CardType GetCardType()
        {
            return cardDisplay.cardSO.cardType;
        }

        public void RemoveFromHand()
        {
            GameStateChanges.InvokeCardRemovedFromHand(this);
        }
        


        private void OnDestroy()
        {
            RemoveFromHand();
        }

        public int AttackValue
        {
            get { return attackValue; }
            set { attackValue = value; }
        }

        public int WheatValue
        {
            get { return wheatValue; }
            set { wheatValue = value; }
        }

        public int HealthValue
        {
            get { return _HealthValue; }
            set { _HealthValue = value; }
        }
    }
}
