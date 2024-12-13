using Com.IsartDigital.F2P.UI.HUD.Augment;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class LockStatus : MonoBehaviour
    {
        [SerializeField] private bool _IsUnlocked = false;

        private void Start()
        {
            if (TryGetComponent<CardDisplay>(out var cardDisplay))
            {
                int lIndex = cardDisplay.displayID;
                IsUnlocked = CardDB.cardDatabaseList[lIndex].cardSO.isUnlocked;
            }
            else if (TryGetComponent<AugmentDisplay>(out var augmentDisplay))
            {
                var augmentSO = augmentDisplay.GetAugmentSO();
                IsUnlocked = AugmentsDB.augmentsDatabaseList.Find(augment => augment == augmentSO)?.isUnlocked ?? false;
            }
        }

        public bool IsUnlocked
        { 
            get { return _IsUnlocked; }
            set { _IsUnlocked = value; }
        }
    }
}
