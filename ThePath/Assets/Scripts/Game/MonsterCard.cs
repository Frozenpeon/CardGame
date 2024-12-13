using TMPro;
using UnityEngine;
using UnityEngine.UI;

//By Matteo Renaudin
namespace Com.IsartDigital.F2P.Cards
{
    public class MonsterCard : MonoBehaviour
    {
        public bool isBoss = false;
        public enum MonsterType { gobelin,  bandit}

        public MonsterType monsterType;
        [SerializeField] private Image _Image = default;
        public TextMeshProUGUI title = default;
        public TextMeshProUGUI MobArrowValue = default;
        public TextMeshProUGUI MobWheatValue = default;

        public TextMeshProUGUI BossArrowValue = default;
        public TextMeshProUGUI BossWheatValue = default;

        [SerializeField] GameObject MonsterLayout;
        [SerializeField] GameObject BossLayout;

        [SerializeField] GameObject _MobArrowContainer;
        [SerializeField] GameObject _MobWheatContainer;

        [SerializeField] private Sprite _BanditSprite;
        [SerializeField] private Sprite _GobelinSprite;
        [SerializeField] private Sprite _BossSprite;

        public MonsterCard(bool pIsBoos)
        {
            isBoss = pIsBoos;
        }

        public void LoadInfo()
        {
            if (isBoss)
            {
                BossLayout.SetActive(true);
                MonsterLayout.SetActive(false);
                _Image.sprite = _BossSprite;
            }
            else
            {
                BossLayout.SetActive(false);
                MonsterLayout.SetActive(true);
                switch (monsterType)
                {
                    case MonsterType.gobelin:
                        _MobArrowContainer.SetActive(true);
                        _MobWheatContainer.SetActive(false);
                        _Image.sprite = _GobelinSprite;
                        break;
                    case MonsterType.bandit:
                        _MobArrowContainer.SetActive(false);
                        _MobWheatContainer.SetActive(true);
                        _Image.sprite = _BanditSprite;
                        break;
                }
            }
        }

        private void Start()
        {
            LoadInfo();
        }
    }
}