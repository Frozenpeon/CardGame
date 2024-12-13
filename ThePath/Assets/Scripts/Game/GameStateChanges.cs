using Com.IsartDigital.F2P.SO.CardSO;
using System;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.F2P
{    
    // Author : Thomas Verdier

    /// <summary>
    /// Static class for all the event we wanna call when the game change it's state.
    /// </summary>
    public static class GameStateChanges 
    {
        /// <summary> 
        /// Will notice a ressource change, if the value sent is negativ, you have to reduce the corresponding value. 
        /// </summary>
        public static event Action<Ressources, int, DayPart> ressourceChange;

        /// <summary> 
        /// Will notice a card change, if the value sent is negativ, you have to reduce the corresponding value. 
        /// </summary>
        public static event Action<CardType, int, DayPart> cardChange;

        /// <summary>
        /// Event invoked if a day is passed.
        /// </summary>
        public static event Action dayPassed;

        /// <summary>
        /// Event invoked if an enemy is defeated. 
        /// </summary>
        public static event Action enemyDefeated;

        /// <summary>
        /// Event invoked if a <see cref="PlayableCard"/> is added to the player hand.
        /// </summary>
        public static event Action<PlayableCard> removedFromHand;

        /// <summary>
        /// Event invoked if a <see cref="PlayableCard"/> is removed from the player hand.
        /// </summary>
        public static event Action<PlayableCard> addToHand;

        /// <summary> 
        /// Event invoked if a card with a specified  <see cref="CardType"/> is played.
        /// </summary>
        public static event Action<PlayableCard> cardPlayed;

        public static event Action GameEnded;
        public static event Action GameRestarted;

        /// <summary>
        /// Method to make the event usable for inheritent classes able to invoke the event. If the <paramref name="pAmount"/> is negativ, it means that 
        /// you have to reduce the correct amount.
        /// </summary>
        /// <param name="pRessource"></param>
        /// <param name="pAmount"></param>
        public static void InvokeRessourceChange(Ressources pRessource, int pAmount, DayPart pDayPart = DayPart.Default)
        {
            ressourceChange?.Invoke(pRessource, pAmount, pDayPart);
        }

        /// <summary>
        /// Method to make the event usable for inheritent classes able to invoke the event.If the <paramref name="pAmount"/> is negativ, it means that 
        /// you have to reduce the correct amount.
        /// </summary>
        /// <param name="pRessource"></param>
        /// <param name="pAmount"></param>
        public static void InvokeCardChange(CardType pCard, int pAmount, DayPart pDayPart = DayPart.Default)
        {
            cardChange?.Invoke(pCard, pAmount, pDayPart);
        }

        /// <summary>
        /// Method to invoke an event if a day is passed in the game.
        /// </summary>
        public static void InvokeDayPassed()
        {
            dayPassed?.Invoke();
        }

        /// <summary>
        /// Method to invoke an event if an enemy is defeated;
        /// </summary>
        public static void InvokeEnemyDefeated()
        {
           enemyDefeated?.Invoke();
        }


        /// <summary>
        /// Method to invoke an event if a card is removed from the hand.
        /// </summary>
        /// <param name="pCard"></param>
        public static void InvokeCardRemovedFromHand(PlayableCard pCard)
        {
            removedFromHand?.Invoke(pCard);
        }

        /// <summary>
        /// Method to invoke an event if a card is added to the hand.
        /// </summary>
        /// <param name="pCard"></param>
        public static void InvokeAddToHand(PlayableCard pCard)
        {
            addToHand?.Invoke(pCard);
        }

        /// <summary>
        /// Method to invoke an event if a card with a specific card type is played.
        /// </summary>
        /// <param name="pCard"></param>
        public static void InvokeAddToPath(PlayableCard pCard)
        {
            cardPlayed?.Invoke(pCard);
        }


        public static void InvokeGameEnded()
        {
            GameEnded?.Invoke();
        }

        public static void InvokeGameRestarted()
        {
            GameRestarted?.Invoke();
        }
    }
}
