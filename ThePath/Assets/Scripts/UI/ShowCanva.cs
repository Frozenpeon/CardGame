using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class ShowCanva : MonoBehaviour
    {
        void Start()
        {
            AugmentHandler.AugmentHide += Hide;
            AugmentHandler.AugmentShow += Show;
        }

        public void Hide()
        {
            if (GetComponent<Canvas>()) GetComponent<Canvas>().enabled = false;
        }

        public void Show()
        {
            if (GetComponent<Canvas>()) GetComponent<Canvas>().enabled = true;
        }

        private void OnDestroy()
        {
            AugmentHandler.AugmentHide -= Hide;
            AugmentHandler.AugmentShow -= Show;
        }
    }
}