using Com.IsartDigital.F2P.Manager.Currency;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    // Author : Thomas Verdier
    /// <summary>
    /// This <see cref="AugmentSO"/> will grant an <paramref name="amountWin"/> of specified <paramref name="ressourceWin"/> when the player defeat a monster.
    /// </summary>
    [CreateAssetMenu(menuName = "Augments/BattleLoot")]
    public class AugmentSOBattleLoot : AugmentSO
    {
        [SerializeField] private Ressources ressourceWin;
        [SerializeField] private int amountWin;

        public override object Clone()
        {
            AugmentSOBattleLoot res = CreateInstance<AugmentSOBattleLoot>();
            res.name = name;
            res.augmentName = augmentName;
            res.augmentDescription = augmentDescription;
            res.amountWin = amountWin + (level * 5);
            res.ressourceWin = ressourceWin;
            res.level = level;
            return res;
        }

        public override string GetDescription()
        {
            return $"After defeating an enemy gives you  +{amountWin + (level * 5)}{GetIconForRessources(ressourceWin)}";
        }

        public override void OnenemyDefeated()
        {
            if (ressourceWin == Ressources.gold)
            {
                CurrencyManager.instance?.AddSoftCurrency(amountWin);
                CurrencyManager.InvokeUpdateAllText();
            }
            GameStateChanges.InvokeRessourceChange(ressourceWin, amountWin);
        }

    }
}
