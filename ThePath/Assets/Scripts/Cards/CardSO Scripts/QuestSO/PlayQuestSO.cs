namespace Com.IsartDigital.F2P.SO.QuestSO
{
    public enum PlayType { Turn, Game, Day }
    public class PlayQuestSO : QuestSO
    {
        public PlayType playType;
        public int numberOfPlaysRequired;
        public override void Activate()
        {
            base.isActive = true;
            EventManager.OnPlayOccurred += HandlePlayOccurred;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            EventManager.OnPlayOccurred -= HandlePlayOccurred;
        }

        public override int GetGoalAmount()
        {
            return numberOfPlaysRequired;
        }

        private void HandlePlayOccurred(PlayType type)
        {
            if (isActive && type == playType)
            {
                progress++;
                if (progress >= numberOfPlaysRequired)
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
