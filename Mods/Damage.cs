using Terraria;
using Terraria.ModLoader;
using Logging = CombatTracker.Systems.Logging;

namespace CombatTracker.Mods
{
    public class Damage : ModPlayer
    {
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            track(npc.TypeName, this.Player.name, hurtInfo.Damage);
            base.OnHitByNPC(npc, hurtInfo);
        }

        // public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        // {
        //     base.OnHitByProjectile(proj, hurtInfo);
        // }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            track(this.Player.name, target.TypeName, damageDone);
            base.OnHitNPC(target, hit, damageDone);
        }

        // public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        // {
        //     base.OnHitNPCWithItem(item, target, hit, damageDone);
        // }

        // public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        // {
        //     base.OnHitNPCWithProj(proj, target, hit, damageDone);
        // }

        // public override void OnHurt(Player.HurtInfo info)
        // {
        //     base.OnHurt(info);
        // }

        private class DamageEvent
        {
            public string Source { get; set; }
            public string Target { get; set; }
            public int Damage { get; set; }
        }

        private static void track(string source, string target, int damageDone)
        {
            var e = new DamageEvent
            {
                Source = source,
                Target = target,
                Damage = damageDone
            };

            Logging.Log("damage", e);
        }
    }
}
