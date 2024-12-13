using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class SaveInit : MonoBehaviour
    {
        public AugmentListSO baseAugmentList;

        private void Start()
        {
            SaveSystem.Init(baseAugmentList);        
        }
    }
}
