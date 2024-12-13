using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.IsartDigital.F2P.SO
{
    [CreateAssetMenu]
    public class BoosterSO : ScriptableObject
    {
        public List<AugmentSO> cards;

        public bool isInitialized = false;

        private int _TotalDropChance = 0;

        public int cardDropAmount;

        public List<int> dropRates = new List<int>();

        public enum RarityValue
        {
            Common = 0,
            Rare = 1
        }

        private List<RarityValue> _RarityValues = new List<RarityValue>();

        private void Initialize()
        {
            if (!isInitialized)
            {
                _TotalDropChance = 0;

                _RarityValues = System.Enum.GetValues(typeof(RarityValue)).Cast<RarityValue>().ToList();

                for (int i = 0; i < dropRates.Count; i++)
                {
                    _TotalDropChance += dropRates[i];
                }

                isInitialized = true;
            }
        }

        private RarityValue GetRandomRarity()
        {
            int pTotalDropChance = Random.Range(0, _TotalDropChance);

            foreach (RarityValue rarity in _RarityValues)
            {
                if (dropRates[(int)rarity] >= pTotalDropChance)
                {
                    return rarity;
                }

                pTotalDropChance -= dropRates[(int)rarity];
            }
            throw new System.IndexOutOfRangeException();
        }

        public List<AugmentSO> GetRandomCards()
        {
            Initialize();

            List<AugmentSO> lCard = new List<AugmentSO>();
            List<AugmentSO> lCardsDrop = new List<AugmentSO>();

            for (int i = 0; i < cardDropAmount; i++)
            {
                RarityValue lRarity = GetRandomRarity();

                lCard.Clear();

                foreach (AugmentSO card in cards)
                {
                    if ((int)card.rarity == (int)lRarity) lCard.Add(card);
                }

                int lRandomAugment = Random.Range(0, lCard.Count);

                lCardsDrop.Add(lCard[lRandomAugment]);
            }

            return (lCardsDrop);
        }
    }
}