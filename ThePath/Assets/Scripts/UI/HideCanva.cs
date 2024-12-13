using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class HideCanva : MonoBehaviour
    {
        void Start()
        {
            AugmentHandler.AugmentHide += Show;
            AugmentHandler.AugmentShow += Hide;
        }

        public void Hide()
        {
           GetComponent<Canvas>().enabled = false;
        }

        public void Show()
        {
            GetComponent<Canvas>().enabled = true;
        }

        private void OnDestroy()
        {
            AugmentHandler.AugmentHide -= Show;
            AugmentHandler.AugmentShow -= Hide;
        }

    }
}
