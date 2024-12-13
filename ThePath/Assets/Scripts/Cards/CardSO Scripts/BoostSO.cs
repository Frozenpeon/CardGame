namespace Com.IsartDigital.F2P.SO.CardSO
{
    public enum CardEffectType
    {
        None, Add, Substract, Multiplicate, Other
    }
    public enum CardAffected
    {
        Left, Right, Adjacent, Self, Path
    }
    public enum SpecialBoost
    {
        None, Copy, DailyProduction, DailyPrayer, Buff
    }
    public class BoostSO : CardSO
    {
        public CardEffectType effectType;
        public CardAffected cardAffected;
        public SpecialBoost specialBoost;
    }
}