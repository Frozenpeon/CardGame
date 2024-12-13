using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class AugmentSaver
    {
        public static AugmentSaver augmentSaverInstance;

        public AugmentListSO AcutalList;

        public AugmentSaver(AugmentListSO pList)
        {
            AcutalList = pList;
            augmentSaverInstance = this;
        }

    }
}
