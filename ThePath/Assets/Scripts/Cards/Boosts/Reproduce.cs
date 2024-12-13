using Com.IsartDigital.F2P.Game.Slot;
using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.Cards.Boosts
{    
    //By Matteo Renaudin
    public class Reproduce : Boost
    {
        CheckCardsSlots _CheckCardsSlots => CheckCardsSlots.instance;
        public Boost boostedCard = null;
        public PlayableCard affectedCard;
        public PlayableCard beforeCard;
        private bool _IsSub = false;

        public Reproduce(CardEffectType lEffectType, int lValue = 0) : base(lEffectType, lValue)
        {
            value = lValue;
            _EffectType = lEffectType;
        }
        public Reproduce(CardEffectType lEffectType) : base(lEffectType)
        {
            _EffectType = lEffectType;
        }
        protected override void ActiveBoost(List<PlayableCard> pAffectedCards, PlayableCard pCurrentCard = null)
        {
            base.ActiveBoost(pAffectedCards);

            _IsSub = false;
            if (pAffectedCards.Count < 0 || pAffectedCards == null || pAffectedCards[0] == null)
            {
                ResetReproduce(pCurrentCard);
                return;
            }
            beforeCard = affectedCard;
            if (pCurrentCard != null)
            {
                affectedCard = pAffectedCards[0];
                PlayableCard lAffectedCard = pAffectedCards[0];

                boostedCard = lAffectedCard.boost;

                pCurrentCard.cardDisplay = lAffectedCard.cardDisplay;
                pCurrentCard.boostList = new List<Boost>(lAffectedCard.boostList);
                pCurrentCard.GetValues();

                if (beforeCard != pAffectedCards[0])
                {
                    _IsSub = true;
                    pCurrentCard.cardVFX.card = pCurrentCard.cardDisplay;
                    pCurrentCard.cardVFX.playableCard = pCurrentCard;
                    pCurrentCard.cardVFX.reproduceEffectAction += ReproduceEffect;
                    pCurrentCard.cardVFX.StartEffectCoroutine(pCurrentCard.cardVFX.reproduceEffect, 4, -2, "_Movement", -1);
                }


                
                if (pCurrentCard.cardDisplay.cardSO.cardType == CardType.Boost)
                {
                    BoostSO lBoostSO = (BoostSO)lAffectedCard.cardDisplay.cardSO;
                    List<PlayableCard> lCards = new List<PlayableCard>(_CheckCardsSlots.GetAffectedCards(pCurrentCard, lBoostSO.cardAffected));

                    if (((BoostSO)lAffectedCard.startCardDisplay.cardSO).specialBoost == SpecialBoost.Copy && lCards.Contains(lAffectedCard))
                        lCards.Remove(lAffectedCard);


                    // Activate Boost
                    switch (lBoostSO.specialBoost)
                    {
                        case SpecialBoost.DailyPrayer:
                            ActivateBeforeBoost<DailyPrayer>(lBoostSO.effectType, lBoostSO.effectValue, lCards, lBoostSO.specialBoost, pCurrentCard);
                            break;

                        case SpecialBoost.Buff:
                            ActivateBeforeBoost<Buff>(lBoostSO.effectType, lBoostSO.effectValue, lCards, lBoostSO.specialBoost, pCurrentCard);
                            break;
                        case SpecialBoost.DailyProduction:
                            ActivateBeforeBoost<DailyProduction>(lBoostSO.effectType, lBoostSO.effectValue, lCards, lBoostSO.specialBoost, pCurrentCard);
                            break;

                        default:
                            if (((BoostSO)lAffectedCard.startCardDisplay.cardSO).specialBoost == SpecialBoost.Copy)
                                ((Reproduce)lAffectedCard.boost).boostedCard?.ActivateBoost(lCards, lBoostSO.specialBoost, pCurrentCard);
                            else
                                lAffectedCard.boost.ActivateBoost(lCards, lBoostSO.specialBoost, pCurrentCard);
                            break;
                    }

                }
            }
            
        }

        private void ReproduceEffect(PlayableCard pPlayableCard, Card pCard)
        {
            pPlayableCard.gameObject.GetComponent<CardDisplay>().SetupCard(pPlayableCard.cardDisplay);
            pPlayableCard.cardVFX.reproduceParticle.Play();
            if (_IsSub) pPlayableCard.cardVFX.reproduceEffectAction -= ReproduceEffect;
        }

        private void ActivateBeforeBoost<T>(CardEffectType pEffectType, int pValue, List<PlayableCard> pCards, SpecialBoost pSpecialBoost, PlayableCard pCurrentCard) where T : Boost
        {
            CreateBoost<T>(pEffectType, pValue).ActivateBoost(pCards, pSpecialBoost, pCurrentCard);
        }

        public void ResetReproduce(PlayableCard pCurrentCard)
        {
            if (pCurrentCard == null) return;
            affectedCard = null;
            pCurrentCard.cardDisplay = pCurrentCard.startCardDisplay;
            pCurrentCard.boostList.Clear();
            pCurrentCard.ResetValue();
            pCurrentCard.cardVFX.reproduceEffect.GetComponent<Image>().enabled = false;

            pCurrentCard.gameObject.GetComponent<CardDisplay>().SetupCard(pCurrentCard.cardDisplay);
        }       
    }
}