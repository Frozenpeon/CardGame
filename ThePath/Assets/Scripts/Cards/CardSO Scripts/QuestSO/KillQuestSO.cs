using Com.IsartDigital.F2P.SO.QuestSO;

namespace Com.IsartDigital.F2P.SO.QuestSO
{
    public enum EnemiesType { Monster, Boss }
    public class KillQuestSO : QuestSO
    {
        public EnemiesType enemyType;
        public int killAmount;
        public override void Activate()
        {
            base.isActive = true;
            EventManager.OnEnemyKilled += HandleEnemyKilled;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            EventManager.OnEnemyKilled -= HandleEnemyKilled;
        }

        public override int GetGoalAmount()
        {
            return killAmount;
        }

        private void HandleEnemyKilled(EnemiesType type)
        {
            if (isActive && type == enemyType)
            {
                progress++;
                if (progress >= killAmount)
                {
                    Deactivate();
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
        }
    }

}

