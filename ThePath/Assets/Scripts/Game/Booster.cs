using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.IsartDigital.F2P.SO;
using TMPro;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P
{
    public class Booster : MonoBehaviour
    {
        public BoosterSO boosterSO;
        public GameObject card;

        public bool openBooster;

        private int _CardCounter = 0;
        private int _CardNumber = 0;

        [SerializeField] private GameObject _Preview;
        [SerializeField] private GameObject _OpenEffect;

        private float _OpenEffectValue = 0;
        private float _OpenEffectSpeed = 0.1f;

        private List<AugmentSO> _Drops = new List<AugmentSO>();
        private List<AugmentSO> _DropsSort = new List<AugmentSO>();
        private List<int> _DropsSortNumber = new List<int>();

        [SerializeField] private Image _BoosterVisual;

        [SerializeField] private float _CardsSpawnSpeed = 1;

        public void ChooseBooster(BoosterSO pBooster)
        {
            boosterSO = pBooster;
        }

        public void CreateBooster(Sprite pBoosterVisual)
        {
            if (_Drops.Count < 1)
            {
                ClearBooster();
                _Drops = boosterSO.GetRandomCards();
                Sort();

                _BoosterVisual.gameObject.SetActive(true);
                _BoosterVisual.sprite = pBoosterVisual;
            }
        }

        private void Sort()
        {
            _CardCounter = 0;

            for (int i = 0; i < _Drops.Count; i++)
            {
                _CardNumber = 0;

                for (int j = 0; j < _DropsSort.Count; j++)
                {
                    if (_Drops[i] == _DropsSort[j])
                    {
                        _DropsSort.RemoveAt(j);
                        _DropsSortNumber.RemoveAt(j);
                    }
                }

                for (int j = 0; j < _Drops.Count; j++)
                {
                    if (_Drops[i] == _Drops[j])
                    {
                        _CardNumber++;
                    }
                }

                _DropsSort.Add(_Drops[i]);
                _DropsSortNumber.Add(_CardNumber);
            }
        }

        public void OpenBooster()
        {
            if (!openBooster)
            {
                if (_Preview != null)
                {
                    ClearBooster();
                }
                else if (_Drops.Count > 0)
                {
                    _BoosterVisual.gameObject.SetActive(false);
                    _Preview = transform.GetChild(0).GetChild(_DropsSort.Count - 1).gameObject;
                    _Preview.SetActive(true);

                    StartCoroutine(OpenEffect());

                    openBooster = true;
                }
            }
        }

        private IEnumerator OpenEffect()
        {
            if (_OpenEffectValue <= 5.5f)
            {
                _OpenEffect.GetComponent<Image>().material.SetFloat("_Progress", _OpenEffectValue);
                yield return new WaitForFixedUpdate();
                _OpenEffectValue += _OpenEffectSpeed;
                StartCoroutine(OpenEffect());
            }
            else
            {
                StartCoroutine(ShowCard());
                _OpenEffectValue = 0;
                _OpenEffect.GetComponent<Image>().material.SetFloat("_Progress", _OpenEffectValue);
            }
        }

        private IEnumerator ShowCard()
        {
            for (int i = 0; i < _DropsSort.Count; i++)
            {
                card = _Preview.transform.GetChild(i).gameObject;

                card.transform.localScale = Vector3.one * .5f;

                card.GetComponent<AugmentLoader>().LoadAnAugment(_DropsSort[_CardCounter]);
                card.GetComponent<AugmentLoader>().numberAugment.text = "X" + _DropsSortNumber[_CardCounter];
                _CardCounter++;

                yield return new WaitForSeconds(_CardsSpawnSpeed);
            }

            openBooster = false;
        }

        public void ClearBooster()
        {
            if (_Preview != null)
            {
                StopAllCoroutines();

                for (int i = 0; i < _Preview.transform.childCount; i++)
                {
                    _Preview.transform.GetChild(i).localScale = Vector3.one * 0.0001f;
                }
                _OpenEffectValue = 0;
                _OpenEffect.GetComponent<Image>().material.SetFloat("_Progress", _OpenEffectValue);
                _Preview.SetActive(false);
                _Preview = null;
                _Drops.Clear();
                _DropsSort.Clear();
                _DropsSortNumber.Clear();
            }
        }
    }
}
