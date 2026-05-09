namespace BattleLabCompanion.Domain
{
    public class DamageEvent
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public string Weapon { get; set; }
        public int Damage { get; set; }
        public bool Crit { get; set; }
    }
}
