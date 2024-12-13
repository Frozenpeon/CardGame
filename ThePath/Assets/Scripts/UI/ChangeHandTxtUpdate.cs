using Com.IsartDigital.F2P.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P.UI.HUD
{
    public class ChangeHandTxtUpdate : MonoBehaviour
    {
        [SerializeField] private string _BaseTxt = "Change Hand";
        private Text _Text => GetComponent<Text>();

        private void Start()
        {
        }

        private void UpdateTxt()
        {
            _Text.text = $"{_BaseTxt} {Deck.instance.nMulligan} / {GameManager.instance.startMulligans}";
        }

        private void OnDestroy()
        {
        }
    }
}
