using Com.IsartDigital.F2P.SO.CardSO;
using Com.IsartDigital.F2P.SO.QuestSO;
using UnityEngine;

//Author : Julian Martin
namespace Com.IsartDigital.F2P
{
    public class EventManager : MonoBehaviour
    {
        static public EventManager instance;

        public delegate void CardPlayedHandler(CardEffect pEffect);
        public static event CardPlayedHandler OnCardPlayed;

        public delegate void EnemyKilled(EnemiesType pEnemyType);
        public static event EnemyKilled OnEnemyKilled;

        public delegate void PlayOccurred(PlayType pPlayType);
        public static event PlayOccurred OnPlayOccurred;

        public delegate void CardSOPlayed(CardSO pCard);
        public static event CardSOPlayed OnCardSOPlayed;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        public static void CardPlayed(CardEffect effect)
        {
            OnCardPlayed?.Invoke(effect);
        }

        public static void TriggerEnemyKilled(EnemiesType pEnemyType)
        {
            OnEnemyKilled?.Invoke(pEnemyType);
        }

        public static void TriggerPlayOccured(PlayType pPlayType)
        {
            OnPlayOccurred?.Invoke(pPlayType);
        }

        public static void TriggerCardSOPlayed(CardSO pCard)
        {
            OnCardSOPlayed?.Invoke(pCard);
        }
    }

    public class CardEffect
    {
        public int maxHealthChange = 0;
        public int healthChange = 0;
        public int attackChange = 0;
        public int wheatChange = 0;

        public CardEffect(int maxHealth = 0, int health = 0, int damage = 0, int wheat = 0)
        {
            maxHealthChange = maxHealth;
            healthChange = health;
            attackChange = damage;
            wheatChange = wheat;
        }
    }
}
