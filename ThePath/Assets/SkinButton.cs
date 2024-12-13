using Com.IsartDigital.F2P.Manager.Currency;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public class SkinButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _StatusText;
        [SerializeField] private float _Price;
        public enum Currency {  gold,  diamond, real}
        public Currency currency;

        public bool owned;

        public string StatusText
        {
            get { return _StatusText.text; }
            set { _StatusText.text = value; }
        }

        public void UpdateText()
        {
            if (owned)
            {
                SetSelected();
                return;
            }
            switch (currency)
            {
                case Currency.gold:
                    if (_Price <= CurrencyManager.instance.gold)
                    {
                        owned = true;
                        SetSelected();
                        //CurrencyManager.instance.ManageCurrency(IAP.CurrencyType.Soft, (int)_Price);
                    }
                    break;
                case Currency.diamond:
                    if (_Price <= CurrencyManager.instance.diamond)
                    {
                        owned = true;
                        SetSelected();
                        //CurrencyManager.instance.ManageCurrency(IAP.CurrencyType.Hard, (int)_Price);
                    }
                    break;
                case Currency.real:
                    owned = true;
                    SetSelected();
                    break;
                default:
                    break;
            }
        }

        private void SetSelected()
        {
            Transform parent = transform.parent;

            // Iterate over all siblings and set their text
            foreach (Transform sibling in parent)
            {
                if (sibling.GetComponent<SkinButton>().owned)
                {
                    sibling.GetComponent<SkinButton>().StatusText = "OWNED";
                }
            }
            _StatusText.text = "SELECTED";
        }
    }
}
