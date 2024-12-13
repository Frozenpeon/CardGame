using Com.IsartDigital.F2P.SO.CardSO;
using System.Collections.Generic;
using UnityEngine;
using Com.IsartDigital.F2P.Utils;
using System.Linq;

//Author : Julian Martin
namespace Com.IsartDigital.F2P
{
    public class CardDB : MonoBehaviour
    {
        public static List<Card> cardDatabaseList = new();
        private const string CARDS_DIRECTORY_PATH = "ScriptableObject/Card/Cards/";

        private void Awake()
        {
            ClearList();
            GetCards();
            SortCardDatabase(); //Added because the getcards() method is asynchonous and doesn't get cards in the perfect order every time
        }

        private void ClearList() //Added because the list duplicates itself each time a new game is started
        {
            cardDatabaseList.Clear();
        }

        private void GetCards()
        {
            UnityEngine.Object[] assets = Resources.LoadAll(CARDS_DIRECTORY_PATH, typeof(CardSO));

            CardSO[] cardSOs = assets.OfType<CardSO>().ToArray();

            foreach (var cardSO in cardSOs)
            {
                cardDatabaseList.Add(new Card(cardSO));
            }
        }

        private void SortCardDatabase()
        {
            cardDatabaseList = cardDatabaseList.OrderBy(card => card.cardSO.ID).ToList();
        }
    }
}