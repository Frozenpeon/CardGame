using Com.IsartDigital.F2P.SO.CardSO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P.Cards
{
    //By Matteo Renaudin
    [Serializable] public class Boost
    {
        public string boostname = "";
        public int value = 0;
        protected int _TempBoostValue;
        protected CardEffectType _EffectType = CardEffectType.None;
        public List<PlayableCard> beforeAffectedCards = new List<PlayableCard>();
        public Boost(CardEffectType lEffectType, int lValue = 0)
        {
            value = lValue;
            _EffectType = lEffectType;
        }
        public Boost(CardEffectType lEffectType)
        {
            _EffectType = lEffectType;
        }

        public void ActivateBoost(List<PlayableCard> pAffectedCards, SpecialBoost pSpecialBoost = SpecialBoost.None, PlayableCard pCurrentCard = null)
        {
            boostname = pCurrentCard.cardDisplay.cardSO.cardName;
            ActiveBoost(pAffectedCards, pCurrentCard);
        }
        protected Slot GetParentSlot(GameObject pGameObject)
        {
            return pGameObject.gameObject.transform.parent.GetComponent<Slot>();
        }

        public void ReturnToDeck()
        {
            foreach (PlayableCard lCard in beforeAffectedCards)
            {
                if (lCard == null) continue;
                lCard.boostList.Remove(this);
            }
        }
        protected virtual void UpdateCards(List<PlayableCard> pCards, bool pIsBefore, PlayableCard pCurrentCard = null) { }

        protected virtual void ActiveBoost(List<PlayableCard> pAffectedCards, PlayableCard pCurrentCard = null) { }

        public static T CreateBoost<T>(CardEffectType pEffectType, int pValue) where T : Boost
        {
            object[] lParamArray = new object[] { pEffectType, pValue };
            return (T)Activator.CreateInstance(typeof(T), args: lParamArray);
        }
    }
}