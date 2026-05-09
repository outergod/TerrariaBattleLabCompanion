using Terraria;
using Terraria.ModLoader;
using BattleLabCompanion.Domain;
using BattleLabCompanion.Systems;

namespace BattleLabCompanion.Mods
{
    public class BattleLabCompanionPlayer : ModPlayer
    {
        public override void OnHitByNPC(NPC npc, Terraria.Player.HurtInfo hurtInfo)
        {
            string attacker = npc.TypeName;
            string victim = Player.name;
            string weapon = "Touch";

            var @event = new DamageEvent
            {
                Source = attacker,
                Target = victim,
                Weapon = weapon,
                Damage = hurtInfo.Damage,
                Crit = false
            };

            Tracking.Track("damage", @event);
        }
    }
}
