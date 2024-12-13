using UnityEngine;
using UnityEngine.TextCore.Text;

//By Matteo Renaudin
namespace Com.IsartDigital.F2P.SO.CardSO
{
    public enum CardType
    {
        CropField, Sharpening, Life, Boost, Monster
    }

    public class CardSO : ScriptableObject
    {
        public int ID;
        public string cardName;
        public string description;
        public Sprite image;
        public CardType cardType;
        public int effectValue;
        public bool isUnlocked;
    }
}