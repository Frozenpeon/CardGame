using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{    
    // Author : Thomas Verdier
    public enum DayPart
    {
        Morning,
        Day,
        Night,
        /// <summary>
        /// This is in case the ressources comes from another way than the classic Path, for example an augment.
        /// </summary>
        Default
    }
    /// <summary>
    ///  <see cref="AugmentSO"/> that upgrade certain part of the day, with the wanter power upgrade and specified type of card boosted.
    /// </summary>
    [CreateAssetMenu(menuName = "Augments/AugmentSOUpPartDay")]
    public class AugmentSOUpPartDay : AugmentSO
    {
        public int powerUpGrade;
        public DayPart dayPart;
        public CardType cardTypeBoosted;
        public bool allTypeUpgraded;
        public override void OnSelect()
        {
            if (AugmentHandler.Instance != null)
                AugmentHandler.Instance.AddActiveAugment(this);
        }
        public override object Clone()
        {
            AugmentSOUpPartDay res = CreateInstance<AugmentSOUpPartDay>();
            res.augmentName = augmentName;
            res.name = name;
            res.augmentDescription = augmentDescription;
            res.powerUpGrade = powerUpGrade + level;
            res.dayPart = dayPart;
            res.cardTypeBoosted = cardTypeBoosted;
            res.allTypeUpgraded = allTypeUpgraded;
            return res;
        }
        public override string GetDescription()
        {
            return $"Your  {cardTypeBoosted} gives an additionnal + {powerUpGrade + level} {GetIconForCardType(cardTypeBoosted)} if they are on {dayPart}";
        }

    }
}
