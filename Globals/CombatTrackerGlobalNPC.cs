using Terraria;
using Terraria.ModLoader;
using BattleLabCompanion.Domain;
using BattleLabCompanion.Systems;

namespace BattleLabCompanion.Globals
{
    public class CombatProvenanceGlobalNPC : GlobalNPC
    {
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            string attacker = player.name;
            string victim = npc.TypeName;
            string weapon = Lang.GetItemNameValue(item.type);

            var @event = new DamageEvent
            {
                Source = attacker,
                Target = victim,
                Weapon = weapon,
                Damage = damageDone,
                Crit = hit.Crit
            };

            Tracking.Track("damage", @event);
        }
    }
}
