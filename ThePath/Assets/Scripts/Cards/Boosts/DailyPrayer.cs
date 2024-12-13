using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections.Generic;
namespace Com.IsartDigital.F2P.Cards.Boosts
{
    //By Matteo Renaudin
    public class DailyPrayer : Boost
    {
        public DailyPrayer(CardEffectType lEffectType, int lValue = 0) : base(lEffectType, lValue)
        {
            value = lValue;
            _EffectType = lEffectType;
        }
        public DailyPrayer(CardEffectType lEffectType) : base(lEffectType) => _EffectType = lEffectType;

        protected override void ActiveBoost(List<PlayableCard> pAffectedCards, PlayableCard pCurrentCard = null)
        {
            base.ActiveBoost(pAffectedCards);

            UpdateCards(beforeAffectedCards, true); // To Clean
            beforeAffectedCards = new List<PlayableCard>();

            // Variables
            CardSO lCardSO = null;
            DayPart lDayPart = DayPart.Default;
            if (pCurrentCard != null) lDayPart = GetParentSlot(pCurrentCard.gameObject) ? GetParentSlot(pCurrentCard.gameObject).dayPart : DayPart.Default ;

            foreach (PlayableCard lCard in pAffectedCards)
            {
                lCardSO = lCard.cardDisplay.cardSO;
                if (lCard.boost == this || lCardSO == null) continue;

                switch (lDayPart)
                {
                    case DayPart.Morning:

                        if (lCardSO.cardType == CardType.CropField || (lCardSO.cardType == CardType.Boost && ((BoostSO)lCardSO).specialBoost == SpecialBoost.DailyProduction))
                            UpdateCards(pAffectedCards, false, lCard);
                        break;

                    case DayPart.Day:

                        if (lCardSO.cardType == CardType.Boost && ((BoostSO)lCardSO).specialBoost == SpecialBoost.Buff)
                            UpdateCards(pAffectedCards, false, lCard);
                        break;

                    case DayPart.Night:

                        if (lCardSO.cardType == CardType.Sharpening || (lCardSO.cardType == CardType.Boost && ((BoostSO)lCardSO).specialBoost == SpecialBoost.DailyProduction))
                            UpdateCards(pAffectedCards, false, lCard);
                        break;
                    default:
                        break;
                }
            }
        }
        protected override void UpdateCards(List<PlayableCard> pCards, bool pIsBefore, PlayableCard pCurrentCard = null)
        {
            base.UpdateCards(pCards, pIsBefore, pCurrentCard);

            if (pIsBefore)
            {
                if (beforeAffectedCards != null && beforeAffectedCards.Count > 0)
                {
                    foreach (PlayableCard lPlayableCard in beforeAffectedCards)
                    {
                        if (lPlayableCard != null && lPlayableCard.boostList.Contains(this))
                        {
                            lPlayableCard.boostList.Remove(this);
                            lPlayableCard.RecoverCard();
                            lPlayableCard.GetValues();
                        }
                    }
                }
            }
            else
            {
                if (!pCurrentCard.boostList.Contains(this))
                {
                    pCurrentCard.boostList.Add(this);
                    beforeAffectedCards.Add(pCurrentCard);
                    pCurrentCard.GetValues();
                    if (pCurrentCard.cardDisplay.cardSO is BoostSO) pCurrentCard.PlayEffect();
                }
            }
        }
    }
}