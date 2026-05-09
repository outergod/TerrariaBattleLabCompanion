using Terraria;
using Terraria.ModLoader;
using BattleLabCompanion.Domain;
using BattleLabCompanion.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using System.IO;

namespace BattleLabCompanion.Globals
{
    public class CombatTrackerGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var prov = projectile.GetGlobalProjectile<CombatProvenanceGlobalProjectile>();

            string attacker =
                prov.SourcePlayerId is int p ? Main.player[p].name :
                prov.SourceNpcId is int n ? Main.npc[n].TypeName :
                projectile.owner >= 0 && projectile.owner < Main.maxPlayers ? Main.player[projectile.owner].name :
                "unknown";

            string victim = target.TypeName;

            string weapon =
                prov.SourceItemType is int it ? Lang.GetItemNameValue(it) :
                Lang.GetProjectileName(projectile.type).Value;

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

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            var prov = projectile.GetGlobalProjectile<CombatProvenanceGlobalProjectile>();

            string attacker =
                prov.SourcePlayerId is int p ? Main.player[p].name :
                prov.SourceNpcId is int n ? Main.npc[n].TypeName :
                (projectile.hostile ? "unknown (hostile)" : "unknown");

            string victim = target.name;

            string weapon =
                prov.SourceItemType is int it ? Lang.GetItemNameValue(it) :
                Lang.GetProjectileName(projectile.type).Value;

            var @event = new DamageEvent
            {
                Source = attacker,
                Target = victim,
                Weapon = weapon,
                Damage = info.Damage,
                Crit = false
            };

            Tracking.Track("damage", @event);
        }
    }
}
