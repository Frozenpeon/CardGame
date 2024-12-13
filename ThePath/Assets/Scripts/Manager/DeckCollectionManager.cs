using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P
{
    [RequireComponent(typeof(CardDB))]
    public class DeckCollectionManager : MonoBehaviour
    {
        [SerializeField] private GameObject _CardPrefab;
        private List<Card> _CollectionCards;
        private RectTransform _RT;
        private ScrollRect _ScrollRect;
        private GridLayoutGroup _Grid;
        private float _ContentWidth;

        void Start()
        {
            _RT = GetComponent<RectTransform>();
            _ScrollRect = transform.parent.GetComponent<ScrollRect>();
            _Grid = GetComponent<GridLayoutGroup>();
            _ContentWidth = _RT.rect.width;
            _CollectionCards = new List<Card>(CardDB.cardDatabaseList);
            FillCollection();
        }

        private void FillCollection()
        {
            int lCardsAmount = _CollectionCards.Count;
            for (int i = 0; i < lCardsAmount; i++)
            {
                GameObject lCard = Instantiate(_CardPrefab, transform);
                lCard.GetComponent<CardDisplay>().displayID = _CollectionCards[i].cardSO.ID;
                lCard.GetComponent<CardDisplay>().LoadInfos(lCard.GetComponent<CardDisplay>().displayID);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_RT);
            _Grid.spacing = new(_Grid.spacing.x, (_RT.rect.height - (CalculateChildrenPerColumn() * _Grid.cellSize.y)) / (CalculateChildrenPerColumn() + 1));
            CheckBounds();
        }


        private void CheckBounds()
        {
            if (Mathf.Abs(transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>().anchoredPosition.x) > (_ContentWidth - (_CardPrefab.GetComponent<RectTransform>().rect.width / 2) - _Grid.padding.right))
            {
                float newValue = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>().anchoredPosition.x - _ContentWidth + _CardPrefab.GetComponent<RectTransform>().rect.width / 2 + _Grid.padding.right;
                _RT.sizeDelta = new(newValue, _RT.sizeDelta.y);
                _ScrollRect.horizontalNormalizedPosition = 0f;
            }
        }

        private int CalculateChildrenPerColumn()
        {
            float availableHeight = _RT.rect.height;

            float totalCellHeight = _Grid.cellSize.y + _Grid.spacing.y;

            int childrenPerColumn = Mathf.FloorToInt(availableHeight / totalCellHeight);

            return childrenPerColumn;
        }
    }
}