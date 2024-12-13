using Com.IsartDigital.F2P.SO.CardSO;
using System;
using UnityEngine;

//Author : Julian Martin
namespace Com.IsartDigital.F2P
{
    [Serializable]
    public class Card
    {
        public CardSO cardSO;
        public Card(CardSO pCard)
        {
            cardSO = pCard;
        }
    }
}
