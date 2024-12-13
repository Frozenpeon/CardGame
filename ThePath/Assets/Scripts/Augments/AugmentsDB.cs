using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections.Generic;
using UnityEngine;
using Com.IsartDigital.F2P.Utils;
using System.Linq;
using Unity.VisualScripting;

//Author : Julian Martin
namespace Com.IsartDigital.F2P
{
    public class AugmentsDB : MonoBehaviour
    {
        public static List<AugmentSO> augmentsDatabaseList = new();
        private const string AUGMENTS_DIRECTORY_PATH = "ScriptableObject/Augment/";

        private void Awake()
        {
            ClearList();
            GetCards();
            SortAugmentDatabase(); //Added because the getcards() method is asynchonous and doesn't get augments in the perfect order every time
        }

        private void ClearList() //Added because the list duplicates itself each time a new game is started
        {
            augmentsDatabaseList.Clear();
        }

        private void GetCards()
        {
            UnityEngine.Object[] assets = Resources.LoadAll(AUGMENTS_DIRECTORY_PATH, typeof(AugmentSO));

            AugmentSO[] augmentsSO = assets.OfType<AugmentSO>().ToArray();

            foreach (var augmentSO in augmentsSO)
            {
                augmentsDatabaseList.Add(augmentSO);
            }
        }

        private void SortAugmentDatabase()
        {
            augmentsDatabaseList = augmentsDatabaseList.OrderBy(augmentSO => augmentSO.augmentName).ToList();
        }
    }
}