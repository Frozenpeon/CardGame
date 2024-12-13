namespace Com.IsartDigital.F2P.SO.QuestSO
{
    public enum CardsType { SpecificCard }
    public class CardQuestSO : QuestSO
    {
        public CardSO.CardSO cardToPlay;
        public int numberOfCardToPlay;
        public override void Activate()
        {
            base.isActive = true;
            EventManager.OnCardSOPlayed += HandleCardPlayed;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            EventManager.OnCardSOPlayed -= HandleCardPlayed;
        }

        public override int GetGoalAmount()
        {
            return numberOfCardToPlay;
        }

        private void HandleCardPlayed(CardSO.CardSO card)
        {
            if (isActive && card == cardToPlay)
            {
                progress++;
                if (progress >= numberOfCardToPlay)
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
