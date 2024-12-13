using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P
{
    public static class AdsEventManager
    {
        public static event Action<Button> resetBtn;
        public static void InvokeResetBtn(Button pBtn) => resetBtn.Invoke(pBtn);
    }
}
