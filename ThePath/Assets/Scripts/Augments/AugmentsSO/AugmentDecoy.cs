using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    [CreateAssetMenu(menuName = "Augments/AugmentDecoy")]
    public class AugmentDecoy : AugmentSO
    {
        public int nSlotAffected;

        public override object Clone()
        {
            AugmentDecoy res = CreateInstance<AugmentDecoy>();
            res.name = name;
            res.augmentName = augmentName;
            res.augmentDescription = augmentDescription;
            res.nSlotAffected = nSlotAffected;
            return res;
        }
    }
}