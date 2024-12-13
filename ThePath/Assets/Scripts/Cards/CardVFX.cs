using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.Cards
{
    public class CardVFX : MonoBehaviour
    {
        public GameObject blessingEffect;
        public GameObject reproduceEffect;
        public GameObject dailyProdEffect;

        public ParticleSystem reproduceParticle;
        public ParticleSystem blessingParticle;

        public Coroutine coroutine;
        public float speed = 1.0f;

        [HideInInspector] public Slot slot;
        [HideInInspector] public PlayableCard playableCard;
        [HideInInspector] public CardDisplay cardDisplay;
        [HideInInspector] public List<PlayableCard> cards;
        [HideInInspector] public Sprite sprite;
        [HideInInspector] public Card card;


        public event Action<PlayableCard, CardDisplay, Sprite> dailyProdEffectAction;
        public event Action<PlayableCard, Card> reproduceEffectAction;


        public void DesactivateSFX()
        {
            blessingEffect.SetActive(false);
            reproduceEffect.SetActive(false);
            dailyProdEffect.SetActive(false);
        }
        public void InvokeDailyProdEffect(PlayableCard pPCard, CardDisplay pDCard, Sprite pSprite) => dailyProdEffectAction?.Invoke(pPCard, pDCard, pSprite);
        public void InvokeReproduceEffect(PlayableCard pPlayableCard, Card pCard) => reproduceEffectAction?.Invoke(pPlayableCard, pCard);
        public void StartEffectCoroutine(GameObject pObjEffect, int pMaxValue, int pMinValue, string pProperty, float pMidValue)
        {
            coroutine = StartCoroutine(EffectCoroutine(pObjEffect, pMaxValue, pMinValue, pProperty, pMidValue));
        }
        public IEnumerator EffectCoroutine(GameObject pObjEffect, int pMaxValue, int pMinValue, string pProperty, float pMidValue)
        {
            pObjEffect.GetComponent<Image>().enabled = true;
            pObjEffect.GetComponent<Image>().material.SetFloat(pProperty, pMinValue);
            float lValue = pObjEffect.GetComponent<Image>().material.GetFloat(pProperty);
            bool lInvoke = false;
            while (lValue <= pMaxValue)
            {
                if (lValue >= pMidValue && !lInvoke)
                {
                    lInvoke = true;
                    InvokeReproduceEffect(playableCard, card);
                    InvokeDailyProdEffect(playableCard, cardDisplay, sprite);
                }


                lValue += Time.deltaTime * speed;
                pObjEffect.GetComponent<Image>().material.SetFloat(pProperty, lValue);
                yield return new WaitForEndOfFrame();
            }
            pObjEffect.GetComponent<Image>().enabled = false;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
