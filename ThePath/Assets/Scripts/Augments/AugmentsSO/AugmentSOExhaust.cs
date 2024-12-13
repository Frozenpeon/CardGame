using Com.IsartDigital.F2P.Cards;
using Com.IsartDigital.F2P.Manager;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Com.IsartDigital.F2P
{
    [CreateAssetMenu(menuName = "Augments/Exhaust")]
    public class AugmentSOExhaust : AugmentSO
    {
        public int DailyUpKeppIncrease;
        public int MonsterPowerDowngrade;

        public override string GetDescription()
        {
            return $"Increase the “Daily Upkeep” by {DailyUpKeppIncrease + level} but decrease monsters strenght by {MonsterPowerDowngrade + (2 * level)}";
        }
        public override object Clone()
        {
            AugmentSOExhaust res = CreateInstance<AugmentSOExhaust>();
            res.name = name;
            res.augmentName = augmentName;
            res.augmentDescription = augmentDescription;
            res.DailyUpKeppIncrease = DailyUpKeppIncrease + level;
            res.MonsterPowerDowngrade = MonsterPowerDowngrade + (2 * level);
            res.level = level;
            return res;
        }

        public override void OnSelect()
        {
            base.OnSelect();
            foreach (GameObject slot in Path.instance.slots)
            {
                slot.GetComponent<Slot>().UpdateDailyUpKeep(1);
            }
        }

        public override void OnDayEnd()
        {
            GameStateChanges.InvokeRessourceChange(Ressources.wheat, -DailyUpKeppIncrease);
        }
    }
}
