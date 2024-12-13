using Com.IsartDigital.F2P.Game.Slot;
using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections.Generic;

namespace Com.IsartDigital.F2P.Cards.Boosts
{
    //By Matteo Renaudin
    public class Buff : Boost
    {
        private CheckCardsSlots _CheckCardsSlots => CheckCardsSlots.instance;
        private int _StartValue = 0;
        public Buff(CardEffectType lEffectType, int lValue = 0) : base(lEffectType, lValue)
        {
            value = lValue;
            _StartValue = lValue;
            _EffectType = lEffectType;
        }
        public Buff(CardEffectType lEffectType) : base(lEffectType)
        {
            _EffectType = lEffectType;
        }

        protected override void ActiveBoost(List<PlayableCard> pAffectedCards, PlayableCard pCurrentCard = null)
        {
            base.ActiveBoost(pAffectedCards);
            value = _StartValue + pCurrentCard.GetBoostValue();

            UpdateCards(beforeAffectedCards, true);
            UpdateCards(pAffectedCards, false, pCurrentCard);

            beforeAffectedCards = new List<PlayableCard>(pAffectedCards);
        }
        /// <summary>
        /// Update Cards according to the list
        /// </summary>
        /// <param name="pCards">Cards List</param>
        /// <param name="pIsBefore"></param>
        /// <param name="pCurrentCard"></param>
        protected override void UpdateCards(List<PlayableCard> pCards, bool pIsBefore, PlayableCard pCurrentCard = null)
        {
            base.UpdateCards(pCards, pIsBefore, pCurrentCard);

            CardSO lCardSO;
            foreach (PlayableCard lCard in pCards)
            {
                if (lCard == null) continue;

                if (pIsBefore)
                    lCard.boostList.Remove(this);
                else
                {
                    lCard.boostList.Add(this);
                    lCardSO = lCard.cardDisplay.cardSO;
                }
                lCard.RecoverCard();
                lCard.GetValues();

                if (lCard.boost != null && lCard.boost is DailyProduction)
                {
                    if (lCard.boost is DailyProduction)
                    {
                        ((DailyProduction)lCard.boost).ActivateBoost(_CheckCardsSlots.GetAffectedCards(lCard, ((BoostSO)lCard.cardDisplay.cardSO).cardAffected), ((BoostSO)lCard.cardDisplay.cardSO).specialBoost, lCard);
                    }
                    else if (lCard.boost is Reproduce)
                    {
                        ((Reproduce)lCard.boost).boostedCard.ActivateBoost(_CheckCardsSlots.GetAffectedCards(lCard, ((BoostSO)lCard.cardDisplay.cardSO).cardAffected), ((BoostSO)lCard.cardDisplay.cardSO).specialBoost, lCard);
                    }
                }
            }
        }
    }
}