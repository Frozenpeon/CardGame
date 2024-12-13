using Com.IsartDigital.F2P.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    // Author : Thomas Verdier

    /// <summary>
    ///  <see cref="AugmentSO"/> that create a DragonEgg for the player, with the specified caracteristics.
    /// </summary>
    [CreateAssetMenu(menuName = "Augments/AugmentSOEgg")]
    public class AugmentSOEgg : AugmentSO
    {
        public Ressources ressourcesEated;
        public int amountEatedBeforeEvolving;      
        public DayPart partOfDayEating;
        public Ressources ressourcesGiven;
        public int amountRessourceGiven;

        public override void OnSelect()
        {
            base.OnSelect();
            SetUpNewEgg();
        }

        public override string GetDescription()
        {
            return $"Each time you would gain {GetIconForRessources(ressourcesEated)} on the {partOfDayEating}, it transfers to the egg.";
        }

        private void SetUpNewEgg()
        {
            EggManager.InvokeOnNewEgg(ressourcesEated == Ressources.wheat ? EggType.Dragon : EggType.Mole);

            EggManager.instance.eggs.Add(new Egg(ressourcesEated, amountEatedBeforeEvolving, 
                                                 ressourcesGiven, amountRessourceGiven, partOfDayEating));
        }
        public override object Clone()
        {
            AugmentSOEgg res = CreateInstance<AugmentSOEgg>();
            res.augmentName = augmentName;
            res.augmentDescription = augmentDescription;
            res.amountEatedBeforeEvolving = amountEatedBeforeEvolving;
            res.ressourcesEated = ressourcesEated;
            res.partOfDayEating = partOfDayEating;
            res.ressourcesGiven = ressourcesGiven;
            res.amountRessourceGiven = amountRessourceGiven;

            return res;
        }
    }
}
