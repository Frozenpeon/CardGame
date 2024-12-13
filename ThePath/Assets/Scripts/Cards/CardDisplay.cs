using Com.IsartDigital.F2P.SO.CardSO;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Author : Julian Martin
namespace Com.IsartDigital.F2P
{
    public class CardDisplay : MonoBehaviour
    {
        /*[HideInInspector] */public Card cardDisplay;
        public int displayID;

        public static int id;

        [SerializeField] private TextMeshProUGUI _CardTitle;
        public Image _CardSprite;

        private void Start()
        {
            LoadInfos(displayID);
        }

        public void SetupImage(Sprite pImage) => _CardSprite.sprite = pImage;

        public void SetupCard(Card card)
        {
            _CardTitle.text = card.cardSO.name;
            _CardSprite.sprite = card.cardSO.image;
        }

        /// <summary>
        /// Load the informations to  display on the card.
        /// </summary>
        /// <param name="pCardID">The ID of the card in the Database</param>
        public void LoadInfos(int pCardID)
        {
            cardDisplay = CardDB.cardDatabaseList[pCardID];
            displayID = pCardID;
            id = cardDisplay.cardSO.ID;
            _CardTitle.text = cardDisplay.cardSO.name;
            _CardSprite.sprite = cardDisplay.cardSO.image;
            gameObject.GetComponent<PlayableCard>().UpdateInfos();
            gameObject.GetComponent<PlayableCard>().startImage = _CardSprite.sprite;
        }

        public void UpdateSprite(Sprite pNewSprite)
        {
            _CardSprite.sprite = pNewSprite;
        }

        public void UpdateTitle(string pNewTitle)
        {
            _CardTitle.text = pNewTitle;
        }
    }
}
