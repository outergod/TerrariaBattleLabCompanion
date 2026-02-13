using Terraria;
using Terraria.ModLoader;
using CombatTracker.Domain;
using CombatTracker.Systems;

namespace CombatTracker.Mods
{
    public class CombatTrackerPlayer : ModPlayer
    {
        public override void OnHitByNPC(NPC npc, Terraria.Player.HurtInfo hurtInfo)
        {
            string attacker = npc.TypeName;
            string victim = Player.name;
            string weapon = "melee";

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
