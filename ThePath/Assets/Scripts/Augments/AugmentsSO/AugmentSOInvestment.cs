using Com.IsartDigital.F2P.SO.CardSO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    // Author : Thomas Verdier

    /// <summary>
    ///  <see cref="AugmentSO"/> that keeps track of certain card that the player holds in hand. If he holds them 
    ///  long enough the card will receive a buff.
    ///  </summary>

    [CreateAssetMenu(menuName = "Augments/Investment")]
    public class AugmentSOInvestment : AugmentSO
    {
        [SerializeField] private CardType _CardType1;
        [SerializeField] private CardType _CardType2;
        [SerializeField] private int _InvestmentTime;
        [SerializeField] private int _ImprovementValue;
        protected Dictionary<PlayableCard, int> dayCounts = new Dictionary<PlayableCard, int>();

        public override string GetDescription()
        {
            return $"Improve your {_CardType1} or {_CardType2}  if you hold them in hand for : {_InvestmentTime - level} days. They will give you +{1 + _ImprovementValue} {GetIconForCardType(_CardType1)} or {GetIconForCardType(_CardType2)}";
        }

        public override void OnSelect()
        {
            base.OnSelect();

        }

        public override object Clone()
        {
            AugmentSOInvestment res = CreateInstance<AugmentSOInvestment>();
            res.name = name;
            res.augmentName = augmentName;
            res.augmentDescription = augmentDescription;
            res._CardType1 = _CardType1;
            res._CardType2 = _CardType2;
            res._InvestmentTime = _InvestmentTime - level;
            res._ImprovementValue = _ImprovementValue;
            res.dayCounts = new Dictionary<PlayableCard, int>();
            res.SubcribToEvents();
            res.level = level;
            foreach (PlayableCard lCard in Deck.instance.actualHand)
                res.AddCardTodictionnary(lCard);         

            return res;    
        }
        /// <summary>
        /// Method to add a <see cref="PlayableCard"/> to the dictionnary, it will only work if the <see cref="CardType"/> of the card is the same as
        /// the <paramref name="_CardType"/> of the augment.
        /// </summary>
        /// <param name="pCard"></param>
        public void AddCardTodictionnary(PlayableCard pCard)
        {

            if ((pCard.cardType == _CardType1 || pCard.cardType == _CardType2) && !dayCounts.ContainsKey(pCard) && !pCard.hasBeenInvestmentedBuffed)
            {
                dayCounts.Add(pCard, 0);
            }
        }

        /// <summary>
        /// Method to remove a <see cref="PlayableCard"/> from the dictionnary.
        /// </summary>
        /// <param name="pCard"></param>
        public void RemoveCardFromDictionnary(PlayableCard pCard)
        {
            
            if (dayCounts.ContainsKey(pCard))
            {
                dayCounts.Remove(pCard);              
            }
        }

        /// <summary>
        /// When the day end, it will increments the count of every cards in the dictionnary. Apply the needed buff and make sure 
        /// the card can't get another investment buff
        /// </summary>
        public override void OnDayEnd()
        {
            List<PlayableCard> cardToRemove = new List<PlayableCard>();

            for (int i = 0; i < dayCounts.Keys.Count; i++)
            {
                PlayableCard card = dayCounts.Keys.ToList()[i];

                dayCounts[card] += 1;
                if (dayCounts[card] >= _InvestmentTime)
                {
                    cardToRemove.Add(card);
                    switch (card.cardType)
                    {
                        case CardType.CropField:
                            card.wheatBonus += _ImprovementValue;
                            break;

                        case CardType.Sharpening:
                            card.attackBonus += _ImprovementValue;
                            break;
                            
                        case CardType.Life:
                            card.healthBonus += _ImprovementValue;
                            break;

                        default:
                            break;

                    }
                    card.hasBeenInvestmentedBuffed = true;
                    card.UpdateInfos();
                    foreach (PlayableCard lcard in cardToRemove)
                        dayCounts.Remove(lcard);
                }

            }
        }



        public override void UnSubcribToEvents()
        {
            GameStateChanges.addToHand -= AddCardTodictionnary;
            GameStateChanges.removedFromHand -= RemoveCardFromDictionnary;
        }

        public override void SubcribToEvents()
        {
            GameStateChanges.addToHand += AddCardTodictionnary;
            GameStateChanges.removedFromHand += RemoveCardFromDictionnary;
        }
    }
}
