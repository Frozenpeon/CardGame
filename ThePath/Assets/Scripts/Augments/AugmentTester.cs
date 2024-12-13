using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class AugmentTester : MonoBehaviour
    {
        public AugmentListSO testList;
        void Start()
        {
           
             foreach (AugmentSO t in testList.AugmentList) 
            {
                t.Presentation();
            }
        }
    }
}
