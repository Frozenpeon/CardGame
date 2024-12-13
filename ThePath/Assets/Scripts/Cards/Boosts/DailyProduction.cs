using Com.IsartDigital.F2P.SO.CardSO;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Com.IsartDigital.F2P.Cards.Boosts
{
    //Author : Julian Martin
    public class DailyProduction : Boost
    {
        private bool _IsSub = false;
        public Slot slot;
        public Slot beforeSlot;
        public DailyProduction(CardEffectType lEffectType, int lValue) : base(lEffectType, lValue)
        {
            value = lValue;
            _EffectType = lEffectType;
        }
        public DailyProduction(CardEffectType lEffectType) : base(lEffectType)
        {
            _EffectType = lEffectType;
        }
        protected override void ActiveBoost(List<PlayableCard> pAffectedCards, PlayableCard pCurrentCard = null)
        {
            base.ActiveBoost(pAffectedCards);
            _IsSub = false;
            Slot lSlot = GetParentSlot(pCurrentCard.gameObject);
            CardDisplay lCard = pCurrentCard.GetComponent<CardDisplay>();
            if (lSlot == null) return;
            beforeSlot = slot;
            slot = lSlot;

            switch (lSlot.dayPart)
            {
                case DayPart.Morning:
                    pCurrentCard.WheatValue = 1 + pCurrentCard.GetBoostValue();
                    pCurrentCard.AttackValue = 0;

                    if (lSlot != beforeSlot )
                    {
                        _IsSub = true;
                        pCurrentCard.cardVFX.playableCard = pCurrentCard;
                        pCurrentCard.cardVFX.cardDisplay = lCard;
                        pCurrentCard.cardVFX.sprite = pCurrentCard._CropFieldImage;
                        pCurrentCard.cardVFX.dailyProdEffectAction += DailyProdEffect;
                        pCurrentCard.cardVFX.StartEffectCoroutine(pCurrentCard.cardVFX.dailyProdEffect, 1, 0, "_Progress", 0.5f);
                    }

                    
                    
                    break;
                case DayPart.Night:
                    pCurrentCard.WheatValue = 0;
                    pCurrentCard.AttackValue = 1 + pCurrentCard.GetBoostValue();

                    if (lSlot != beforeSlot)
                    {
                        _IsSub = true;
                        pCurrentCard.cardVFX.playableCard = pCurrentCard;
                        pCurrentCard.cardVFX.cardDisplay = lCard;
                        pCurrentCard.cardVFX.sprite = pCurrentCard._SharpeningImage;
                        pCurrentCard.cardVFX.dailyProdEffectAction += DailyProdEffect;
                        pCurrentCard.cardVFX.StartEffectCoroutine(pCurrentCard.cardVFX.dailyProdEffect, 1, 0, "_Progress", 0.5f);
                    }
                   
                    break;
                default:
                    pCurrentCard.WheatValue = 0;
                    pCurrentCard.AttackValue = 0;
                    lCard.SetupImage(pCurrentCard.startImage);
                    break;
            }
            pCurrentCard.RecoverCard();
            pCurrentCard.GetValues();

        }

        private void DailyProdEffect(PlayableCard pPCard, CardDisplay pDCard, Sprite pSprite)
        {
            pDCard.SetupImage(pSprite);
            if (_IsSub) pPCard.cardVFX.dailyProdEffectAction -= DailyProdEffect;
        }

        public static T CreateBoost<T>(CardEffectType pEffectType, int pValue, Sprite pSharpeningImage, Sprite pCropFieldImage) where T : Boost
        {
            object[] lParamArray = new object[] { pEffectType, pValue, pSharpeningImage, pCropFieldImage };
            return (T)Activator.CreateInstance(typeof(T), args: lParamArray);
        }
    }
}