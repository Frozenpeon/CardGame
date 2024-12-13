using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    // Author : Thomas Verdier

    public enum TradedItem
    {
        card,
        ressources
    }

    /// <summary>
    /// Class use for trading ressources or cards for another card or ressources, it gives you the choice to give either card or ressources and get ressources or card 
    /// in return.
    /// </summary>
    public class AugmentSOTrade : AugmentSO
    {
        // Offered part
        public TradedItem offeredItem;
        public int amountOffered;
        [HideInInspector] public CardType cardTypeOffered = 0;
        [HideInInspector] public Ressources ressourceTypeOffered = 0;

        // Received part
        public TradedItem receivedItem;
        public int amountReceived;
        [HideInInspector] public CardType cardTypeReceived = 0;
        [HideInInspector] public Ressources ressourceTypeReceived = 0;

        public override string GetDescription()
        {
            string result = $"J'offre {amountOffered}";
            switch (offeredItem)
            {
                case TradedItem.card:
                    result += $" {cardTypeOffered}";
                    break;
                case TradedItem.ressources:
                    result += $" {ressourceTypeOffered}";
                    break;
                default:
                    break;
            }
            result += $" contre {amountReceived}";

            switch (receivedItem)
            {
                case TradedItem.card:
                    result += $" {cardTypeReceived}";
                    break;
                case TradedItem.ressources:
                    result += $" {ressourceTypeReceived}";
                    break;
                default:
                    break;
            }
            
            return result;
        }

        public override void OnSelect()
        {
            base.OnSelect();
            switch (offeredItem)
            {
                case TradedItem.card:
                    GameStateChanges.InvokeCardChange(cardTypeOffered,amountOffered);
                    break;
                case TradedItem.ressources:
                    GameStateChanges.InvokeRessourceChange(ressourceTypeOffered, amountOffered);
                    break;
                default:
                    break;
            }


            switch (receivedItem)
            {
                case TradedItem.card:
                    GameStateChanges.InvokeCardChange(cardTypeReceived, amountReceived);
                    break;
                case TradedItem.ressources:
                    GameStateChanges.InvokeRessourceChange(ressourceTypeReceived, amountReceived);
                    break;
                default:
                    break;
            }

        }
    }
}
