using Com.IsartDigital.F2P.UI.HUD.Augment;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Com.IsartDigital.F2P
{
    public class CollectionManager : MonoBehaviour
    {
        public static CollectionManager instance;

        [Space(10), Header("Buttons")]
        [SerializeField] private Button _DeckButton;
        [SerializeField] private Button _AugmentsButton;

        [Space(10), Header("Containers")]
        [SerializeField] private GameObject _CardsContainer;
        [SerializeField] private GameObject _AugmentsContainer;

        [SerializeField] private ScrollRect _ScrollRect;
        [SerializeField] private TextMeshProUGUI UnlockedCardText;

        [Space(10), Header("Collection")]
        [SerializeField] private Image _FillArea;
        [SerializeField] private Image _Stroke;

        [Space(10), Header("Colors")]
        [SerializeField] private Color _DeckFillAreaColor = new(173f / 255f, 209f / 255f, 230f / 255f, 1f);
        [SerializeField] private Color _DeckStrokeColor = new(58f / 255f, 111f / 255f, 143f / 255f, 1f);
        [SerializeField] private Color _AugmentsFillAreaColor = new(207f / 255f, 242f / 255f, 194f / 255f, 1f);
        [SerializeField] private Color _AugmentsStrokeColor = new(110f / 255f, 139f / 255f, 95f / 255f, 1f);

        [Space(10), Header("Hidden image")]
        [SerializeField] private Sprite _HiddenCardSprite;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void Start()
        {
            ButtonShapeMatchSpriteShape(_DeckButton);
            ButtonShapeMatchSpriteShape(_AugmentsButton);

            _DeckButton.onClick.AddListener(() => SetButtonAsLastSibling(_DeckButton));
            _DeckButton.onClick.AddListener(() => ChangeColor(_DeckFillAreaColor, _DeckStrokeColor));
            _DeckButton.onClick.AddListener(() => StartCoroutine(DelayedUpdateCardsUnlockedText(_CardsContainer, _AugmentsContainer)));

            _AugmentsButton.onClick.AddListener(() => SetButtonAsLastSibling(_AugmentsButton));
            _AugmentsButton.onClick.AddListener(() => ChangeColor(_AugmentsFillAreaColor, _AugmentsStrokeColor));
            _AugmentsButton.onClick.AddListener(() => StartCoroutine(DelayedUpdateCardsUnlockedText(_AugmentsContainer, _CardsContainer)));

            _DeckButton.onClick.Invoke();
        }

        public void InvokeClick()
        {
            _DeckButton.onClick.Invoke();
        }

        private void ChangeColor(Color pFillAreaColor, Color pStrokeColor)
        {
            _FillArea .color = pFillAreaColor;
            _Stroke.color = pStrokeColor;
        }

        private void SetButtonAsLastSibling(Button pButton)
        {
            pButton.transform.SetAsLastSibling();
        }

        private void ButtonShapeMatchSpriteShape(Button pButton)
        {
            pButton.transform.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }

        private IEnumerator DelayedUpdateCardsUnlockedText(GameObject pContainerToSee, GameObject pContainerToHide)
        {
            yield return null; // Wait for the end of the frame to ensure all scripts are loaded
            SwapVisibleContainer(pContainerToSee, pContainerToHide);
        }

        private void SwapVisibleContainer(GameObject pContainerToSee, GameObject pContainerToHide)
        {
            pContainerToHide.SetActive(false);
            pContainerToSee.SetActive(true);
            ChangeScrollRectContent(pContainerToSee.GetComponent<RectTransform>());
            UpdateUnlockedText(pContainerToSee);
            _ScrollRect.horizontalNormalizedPosition = 0f;
        }

        private void ChangeScrollRectContent(RectTransform pRectTransform)
        {
            _ScrollRect.content = pRectTransform;
        }

        private void UpdateUnlockedText(GameObject pContainer)
        {
            int lUnlockedObjects = 0;
            int lChildCount = pContainer.transform.childCount;
            for (int i = 0; i < lChildCount; i++)
            {
                if (pContainer.transform.GetChild(i).GetComponent<LockStatus>().IsUnlocked)
                {
                    lUnlockedObjects++;
                }
                else
                {
                    if (pContainer == _CardsContainer)
                    {
                        CardDisplay lCardDisplay = pContainer.transform.GetChild(i).GetComponent<CardDisplay>();
                        lCardDisplay.UpdateSprite(_HiddenCardSprite);
                        lCardDisplay.UpdateTitle(string.Empty);
                    }
                    else //augment container case
                    {
                        AugmentDisplay lAugmentDisplay = pContainer.transform.GetChild(i).GetComponent<AugmentDisplay>();
                        lAugmentDisplay.UpdateSprite(_HiddenCardSprite);
                        lAugmentDisplay.DeactivateIcon();
                        lAugmentDisplay.UpdateDescription(string.Empty);
                        lAugmentDisplay.UpdateTitle(string.Empty);
                    }
                }
            }
            UnlockedCardText.text = lUnlockedObjects.ToString() + "/" + lChildCount.ToString();
        }
    }
}
