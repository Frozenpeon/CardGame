using Com.IsartDigital.F2P.Manager;
using Com.IsartDigital.F2P.SO.CardSO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Com.IsartDigital.F2P
{    
    // Author : Thomas Verdier

    public enum Ressources
    {
        life,
        gold, 
        wheat,
        attack,
        maxLife
    }

    public enum Rarity
    {
        Common = 0, 
        Rare = 1
    }

    /// <summary>
    /// This is the base class for every augment in the game, they all have basics informations such as,
    /// a <paramref name="Name"/>, a <paramref name="Description"/> and a specific method to apply on selection.
    ///  <para> These is also a virtual funtion OnSelect() that is going to be the effect of the augment</para>
    /// </summary>
    [CreateAssetMenu]
    public class AugmentSO : ScriptableObject , ICloneable
    {
        public bool isUnlocked;
        public int dropChance;
        public Rarity rarity;
        public string augmentName;
        public string augmentDescription;
        //public Sprite augmentIcon;

        public int level;        

        /// <summary>
        /// Just to have a quick debug view, writing what we want in the debug log;
        /// </summary>
        public virtual void Presentation()
        {
            Debug.Log(GetDescription());
        }

        /// <summary>
        /// Method to get the card description of the card. Molstly used to debug and write on the card.
        /// </summary>
        /// <returns></returns>
        public virtual string GetDescription()
        {
            return augmentDescription;
        }
        /// <summary>
        /// Method called when the augment is selected.
        /// </summary>
        public virtual void OnSelect()
        {
            if (AugmentHandler.Instance != null)
            {
                if (PlayerData.ActualPlayerData.isFTUE) AugmentHandler.Instance.augmentAlreadySpawn.Add(this);
                AugmentHandler.Instance.AddActiveAugment(this);
            }

            PathEventManager.InvokeOnMonsterKilled((int)Path.instance.nMonsterKilled);
        }

        private void CheckLock()
        {
            if (isUnlocked)
            {
                return;
            }
            else
            {
                isUnlocked = true;
            }
        }

        /// <summary>
        /// Method used to remove the augment from the activ augment list.
        /// </summary>
        protected void RemoveAugment()
        {
            if (AugmentHandler.Instance != null)
                AugmentHandler.Instance.RemoveActiveAugment(this);
        }

        /// <summary>
        /// Method that create a cloned instance of the <see cref="ScriptableObject"/> augment.
        /// Need to make sure to do it properly, otherwise the clone isn't properly set up and the effect might
        /// not work as expected.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            AugmentSO res = CreateInstance<AugmentSO>();
            res.name = name;
            res.augmentName = augmentName;
            res.augmentDescription = augmentDescription;
            Debug.Log("Maybe forgot to implement Clone(), in the new augment class ");
            return res;
        }

        /// <summary>
        /// Method called when the day end.
        /// </summary>
        public virtual void OnDayEnd()
        {

        }

        /// <summary>
        /// Mathod called when a card is played.
        /// </summary>
        public virtual void OnCardPlayed(PlayableCard pCard)
        {

        }

        /// <summary>
        /// Mathod called when an enemy is defeated.
        /// </summary>
        public virtual void OnenemyDefeated()
        {

        }

        public virtual void UnSubcribToEvents()
        {

        }
        public virtual void SubcribToEvents()
        {

        }

        protected string GetIconForRessources(Ressources pRessource)
        {
            switch (pRessource)
            {
                case Ressources.wheat:
                    return IconsManager.WHEAT;
                case Ressources.attack:
                    return IconsManager.ARROW;
                case Ressources.life:
                    return IconsManager.HEART;
                case Ressources.gold:
                    return IconsManager.GOLD;
            }
            return "UNKNOWN";
        }

        protected string GetIconForCardType(CardType pCard)
        {
            switch (pCard)
            {
                case CardType.CropField:
                    return IconsManager.WHEAT;
                case CardType.Sharpening:
                    return IconsManager.ARROW;
            }
            return "";
        }
        
    }
}
