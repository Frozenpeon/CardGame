using Com.IsartDigital.F2P.Cards;
using Com.IsartDigital.F2P.Manager;
using Com.IsartDigital.F2P.SO.CardSO;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    [CreateAssetMenu(menuName = "Augments/AugmentGreat")]
    public class AugmentGreat : AugmentSO
    {
        public CardType cardType;
        public Ressources ressourceGained;
        public int amountRessourceGained;

        public override object Clone()
        {
            AugmentGreat res = CreateInstance<AugmentGreat>();
            res.name = name;
            res.augmentName = augmentName;
            res.augmentDescription = augmentDescription;
            res.cardType = cardType;
            res.ressourceGained = ressourceGained;
            res.amountRessourceGained = amountRessourceGained + level;
            res.level = level;
            return res;
        }

        public override string GetDescription()
        {
            return $"If you place 2 {cardType} cards next to each other,  you gain +{amountRessourceGained + level} {GetIconForRessources(ressourceGained)}";
        }

        public override void SubcribToEvents()
        {
            base.SubcribToEvents();
            PathEventManager.onPathMoved += CheckIfTwoCardWithSameTypeAreNext;
        }
        public override void UnSubcribToEvents()
        {
            base.UnSubcribToEvents();
            PathEventManager.onPathMoved -= CheckIfTwoCardWithSameTypeAreNext;
        }

        public override void OnSelect()
        {
            base.OnSelect();
            SubcribToEvents();
        }

        private void CheckIfTwoCardWithSameTypeAreNext()
        {
            int lNCardNext = 0;
            CardDisplay lCard = null;
            foreach (GameObject lSlot in Path.instance.slots)
            {
                if (lSlot.GetComponent<Slot>().isUsed)
                {
                    lCard = lSlot.GetComponentInChildren<CardDisplay>();

                    if (lCard && lCard.cardDisplay.cardSO.cardType == cardType)
                    {
                        lNCardNext++;
                        if (lNCardNext >= 2)
                        {
                            GameStateChanges.InvokeRessourceChange(ressourceGained, amountRessourceGained);
                            break;
                        }
                    }
                }
            }
        }
    }
    
}