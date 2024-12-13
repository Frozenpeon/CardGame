using Com.IsartDigital.F2P.Analytics;
using Com.IsartDigital.F2P.Cards;
using Com.IsartDigital.F2P.Cards.Boosts;
using Com.IsartDigital.F2P.Game.FTUE;
using Com.IsartDigital.F2P.Game.Slot;
using Com.IsartDigital.F2P.Manager;
using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Com.IsartDigital.F2P.Game
{
    public class MovableCard : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float _SelectScaleMultiplier;

        private Animator _Animator;
        private string _SelectedState = "Selected";

        private bool _IsMouseOutside;
        private bool _IsTapMoved;

        public bool isSelected;
        private bool _IsPosed = false;
        public bool canMove = true;

        private Vector2 _DeckPos;

        [SerializeField] private LayerMask _EmptySlotLayer;
        [SerializeField] private LayerMask _FilledSlotLayer;

         public GameObject slot;
        public GameObject lastSlot;
        [HideInInspector] public GameObject changeCard;

        [SerializeField] private float _Speed;

        [SerializeField] private GameObject _CollisionBox;

        private Vector3 _DefaultScale;
        private Vector3 _SelectedScale;

        [SerializeField] private GetInfo _GetInfo;

        private Camera _Camera;

        // Change Parent variables
        public CardSO _CardSO => transform.parent.GetComponent<CardDisplay>().cardDisplay.cardSO;
        private CheckCardsSlots _CheckCardsSlots => CheckCardsSlots.instance;
        private GameManager _GameManager => GameManager.instance;
        private Deck _Deck => Deck.instance;
        private Transform _StartParent;
        private Transform _BeforeParent;
        private Cards.Slot _BeforeParentSlot;
        private PlayableCard _Card => transform.parent.gameObject.GetComponent<PlayableCard>();

        private void Start()
        {
            _Animator = transform.parent.GetComponent<Animator>();
            _Camera = Camera.main;

            _DefaultScale = transform.localScale;
            _SelectedScale = _DefaultScale * _SelectScaleMultiplier;

            _StartParent = transform.parent.parent;

            _DeckPos = _Deck.GetCardPosition(transform.parent.GetSiblingIndex());
        }

        public void OnDrag(PointerEventData eventdata)
        {
            if (!canMove) return;

            _IsTapMoved = false;
            //if (isSelected) transform.parent.position = eventdata.position;
            if (isSelected)
            {
                Vector3 worldPoint;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                transform.parent as RectTransform, eventdata.position, _Camera, out worldPoint);
                transform.parent.position = worldPoint;
            }

            if (slot != null)
            {
                UpdateParent(_StartParent);
                UpdateCards();

                transform.parent.SetAsLastSibling();
                slot.layer = (int)Mathf.Log(_EmptySlotLayer, 2);

                slot = null;
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!canMove) return;

            if (!isSelected)
            {
                _IsTapMoved = true;
                SelectedCard(true);
                PathEventManager.InvokeMoveHand();
            }
            else
            {
                SelectedCard(false);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!_IsTapMoved) SelectedCard(false);
        }

        private void Update()
        {
            if (!canMove) return;

            if (_GameManager.PointerTouched().Count > 0)
            {
                SlotDetection();
            }
            else _IsMouseOutside = true;

            if (slot != null)
            {
                CardMove();
            }
            else if (transform.parent.GetComponent<RectTransform>().anchoredPosition != _Deck.GetCardPosition(transform.parent.GetSiblingIndex()) && !isSelected)
            {
                ReturnToDeck();
            }

            if (_IsMouseOutside && isSelected && Input.GetMouseButtonDown(0))
            {
                SelectedCard(false);
            }
        }

        public void ResetSlot()
        {
            slot.layer = (int)Mathf.Log(_EmptySlotLayer, 2);
            lastSlot = null;
            slot = null;
        }

        private void SlotDetection()
        {
            foreach (var target in _GameManager.PointerTouched())
            {
                if (isSelected)
                {
                    if (target.gameObject.layer == (int)Mathf.Log(_EmptySlotLayer, 2))
                    {
                        if (!_IsTapMoved || Input.GetMouseButtonDown(0))
                        {
                            if (slot != null && target.gameObject != slot) slot.layer = (int)Mathf.Log(_EmptySlotLayer, 2);
                            slot = target.gameObject;
                        }
                    }
                    else if (target.gameObject.layer == (int)Mathf.Log(_FilledSlotLayer, 2))
                    {
                        foreach (MovableCard card in FindObjectsOfType<MovableCard>())
                        {
                            if (card.transform.position == target.gameObject.transform.position)
                            {
                                target.gameObject.layer = (int)Mathf.Log(_EmptySlotLayer, 2);
                                changeCard = card.gameObject;
                                slot = target.gameObject;
                            }
                        }
                    }
                }

                if (_GameManager.PointerTouched()[0].gameObject == _CollisionBox) _IsMouseOutside = false;
                else _IsMouseOutside = true;
            }
        }

        private void CardMove()
        {
            if (!isSelected)
            {
                if (changeCard != null )
                {
                    MovableCard lChangeCard = changeCard.GetComponent<MovableCard>();

                    if (changeCard.GetComponent<MovableCard>().slot == slot && changeCard != gameObject)
                    {
                        lChangeCard.slot = lastSlot;
                        lChangeCard.lastSlot = slot;
                        lChangeCard.SelectedCard(false);

                        lChangeCard.changeCard = null;
                        changeCard = null;

                        PutCardInSlot();
                        PathEventManager.InvokeOnCardPlayed();
                        GameStateChanges.InvokeAddToPath(_Card);
                        PathEventManager.InvokeDesactivateHand();

                        if (FTUEManager.instance && FTUEManager.instance.popEnum == PopUpEnum.Resources) FTUEManager.instance.ChangePopUp();
                    }
                }

                if (transform.position != slot.transform.position)
                {
                    transform.parent.position = Vector3.MoveTowards(transform.position, slot.transform.position, _Speed * Time.deltaTime);
                }
                else if (slot != lastSlot)
                {
                    if (slot && slot.GetComponent<Cards.Slot>() && slot.GetComponent<Cards.Slot>().btn && slot.GetComponent<Cards.Slot>().btn.interactable)
                    {
                        // Put Card
                        lastSlot = slot;
                        PutCardInSlot();
                        PathEventManager.InvokeOnCardPlayed();
                        GameStateChanges.InvokeAddToPath(_Card);
                        PathEventManager.InvokeDesactivateHand();
                        if (FTUEManager.instance && FTUEManager.instance.popEnum == PopUpEnum.Resources) FTUEManager.instance.ChangePopUp();
                    }
                    else
                    {
                        slot = null;
                        ReturnToDeck();
                    }
                }
                else if (slot.layer == (int)Mathf.Log(_EmptySlotLayer, 2))
                {                    
                    if (slot && slot.GetComponent<Cards.Slot>() && slot.GetComponent<Cards.Slot>().btn && slot.GetComponent<Cards.Slot>().btn.interactable)
                    {
                        // Put Card
                        slot.layer = (int)Mathf.Log(_FilledSlotLayer, 2);
                        PutCardInSlot();
                    }
                }
                else if (lastSlot != null && transform.parent != slot)
                {
                    if (slot && slot.GetComponent<Cards.Slot>() && slot.GetComponent<Cards.Slot>().btn && slot.GetComponent<Cards.Slot>().btn.interactable)
                    {
                        UpdateParent(slot.transform);
                        PlayCard(slot.transform, false);
                    }
                }
            }
        }


        public void ReturnToDeck()
        {
            transform.parent.GetComponent<RectTransform>().anchoredPosition = Vector3.MoveTowards(transform.parent.GetComponent<RectTransform>().anchoredPosition, _Deck.GetCardPosition(transform.parent.GetSiblingIndex()), _Speed * Time.deltaTime);

            if (lastSlot != null)
            {
                RemoveCard();
                UpdateCards();
                _Card.cardVFX.blessingEffect.SetActive(false);
                if (_Card.boost is Reproduce)
                {
                    (_Card.boost as Reproduce).ResetReproduce(_Card);
                }
                if (_Card.boost is DailyProduction)
                {
                    (_Card.boost as DailyProduction).slot = null;
                }
                lastSlot = null;
            }
            if (transform.parent != _Deck.gameObject) UpdateParent(_StartParent);
        }

        public void SelectedCard(bool pSelected)
        {
            isSelected = pSelected;

            if (pSelected)
            {
                transform.localScale = _SelectedScale;
            }
            if (!pSelected)
            {
                transform.localScale = _DefaultScale;
                _IsTapMoved = pSelected;
            }

            _Animator.SetBool(_SelectedState, pSelected);
        }
        private void PutCardInSlot()
        {
            UpdateParent(slot.transform);
            PlayCard(slot.transform);
            UpdateCards();
        }
        public void UpdateParent(Transform pNewParent)
        {
            // By Matteo Renaudin
            _BeforeParent = transform.parent.parent;
            transform.parent.SetParent(pNewParent);

            _BeforeParentSlot = _BeforeParent?.GetComponent<Cards.Slot>();

            if (_Card.isInPath && _BeforeParentSlot) _BeforeParentSlot.card = null;
        }
        public void UpdateCards()
        {
            // By Matteo Renaudin
            _CheckCardsSlots.UpdateCards();
            _CheckCardsSlots.SortCardsOnSlots();
        }
        private void PlayCard(Transform pNewParent, bool pPutCard = true)
        {
            // By Matteo Renaudin
            pNewParent.GetComponent<Cards.Slot>().card = transform.parent.gameObject;

            if (pPutCard && !_IsPosed)
            {
                _IsPosed = true;
                _CheckCardsSlots.cardsInSlot.Add(_CardSO);
            }            
            _Card.GetValues();

            if (!_Card.isInPath)
            {
                _Deck.SortDeckList();
                _Card.PlayCard();
            }
            _Card.isInPath = true;
        }
        private void RemoveCard()
        {
            // By Matteo Renaudin
            if (_Card.isInPath)
            {
                _Card.RecoverCard();
                _Card.GetValues();
                _CheckCardsSlots.cardsInSlot.Remove(_CardSO);

                _IsPosed = false;
                _Card.isInPath = false;

                if (_Card.startCardDisplay.cardSO.cardType == CardType.Boost)
                {
                    _Card.boost.ReturnToDeck();
                    _Card.cardDisplay = _Card.startCardDisplay;
                    _Card.ResetValue();
                    _Card.gameObject.GetComponent<CardDisplay>().SetupCard(_Card.cardDisplay);
                }
                _Deck.SortDeckList();
            }
        }
    }
}