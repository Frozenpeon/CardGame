using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    // Author : Thomas Verdier
    /// <summary>
    /// This <see cref="AugmentSO"/> will destroy the next card of the specified <paramref name="cardTypeDestroyed"/> when the card
    /// is played and it will grant the player an <paramref name="amountRessourceGained"/> of <paramref name="ressourceGained"/>
    /// </summary>
    [CreateAssetMenu(menuName = "Augments/Pact")]
    public class AugmentSOPact : AugmentSO
    {
        public CardType cardTypeDestroyed;
        public int amountRessourceGained;
        public Ressources ressourceGained;

        public override string GetDescription()
        {
            return $"The next {cardTypeDestroyed} you play will be destroyed but you gain {amountRessourceGained + (2 *level)} {GetIconForRessources(ressourceGained)}";
        }

        public override object Clone()
        { 
            AugmentSOPact res = CreateInstance<AugmentSOPact>();
            res.name = name;
            res.augmentName = augmentName;
            res.augmentDescription = augmentDescription;
            res.cardTypeDestroyed = cardTypeDestroyed;
            res.ressourceGained = ressourceGained;
            res.amountRessourceGained = amountRessourceGained + (2 * level);
            res.level = level;  
            return res;
        }

        public override void OnCardPlayed(PlayableCard pCard)
        {
            if (pCard.startCardDisplay.cardSO.cardType == cardTypeDestroyed)
            {
                Deck.instance.RemoveACardFromPool(pCard.startCardDisplay.cardSO.cardType);
                Destroy(pCard.gameObject);
                GameStateChanges.InvokeRessourceChange(ressourceGained, amountRessourceGained);
                RemoveAugment();
                Deck.instance.CheckHand();
            }
        }

    }
}
