using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    // Author : Thomas Verdier
    /// <summary>
    ///  <see cref="AugmentSO"/> that is a basic addition classe, this is going to add ressources with a specified amount.
    /// </summary>
    [CreateAssetMenu(menuName = "Augments/Add")]
    public class AugmentSOAdd : AugmentSO
    {
        public int Value;
        public Ressources Ressouces;
        public bool isPermanent;



        public override string GetDescription()
        {
            return $"Gives  +{Value}{GetIconForRessources(Ressouces)}";
        }

        public override void OnSelect()
        {
            base.OnSelect();
            GameStateChanges.InvokeRessourceChange(Ressouces, Value);
        }
    }
}
