using Com.IsartDigital.F2P.SO.CardSO;
using System;
using UnityEngine;

namespace Com.IsartDigital.F2P.Game
{
    // By Matteo Renaudin
    [Serializable]
    public class Monster
    {
        public MonsterSO monsterSO;
        public Monster(MonsterSO pMonster)
        {
            monsterSO = pMonster;
        }
    }
}