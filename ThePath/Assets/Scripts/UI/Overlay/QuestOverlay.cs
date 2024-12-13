using Com.IsartDigital.F2P.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Com.IsartDigital.F2P
{
    public class QuestOverlay : MonoBehaviour
    {
        private static Action OnBtnBackClick;
        private void OnEnable()
        {
            OnBtnBackClick += UIManager.GetInstance.SwitchQuestOverlay;
        }

        private void OnDisable()
        {
            OnBtnBackClick -= UIManager.GetInstance.SwitchQuestOverlay;
            OnBtnBackClick = null;
        }
        public void OnBackBtn()
        {
            OnBtnBackClick.Invoke();
        }
    }
}
