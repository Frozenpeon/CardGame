using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Rendering;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class AugmentDisplayDatabase : MonoBehaviour
    {
        public static AugmentDisplayDatabase instance;
        private void Awake()
        {
            if(instance!=null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        [Serializable]
        public class AugmentAndDisplay
        {
            public AugmentSO augmentSO;
            public Sprite sprite;
        }

        public List<AugmentAndDisplay> augmentAndDisplayList = new();

        public Sprite GetSprite(AugmentSO pAugment)
        {
            foreach (var item in augmentAndDisplayList)
            {
                if (item.augmentSO.name == pAugment.name)
                {
                    return item.sprite;
                }
            }
            Debug.LogWarning($"Sprite for augment {pAugment.augmentName} not found!");
            return null;
        }
    }
}
