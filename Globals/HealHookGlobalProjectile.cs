using System;
using BattleLabCompanion.Systems;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BattleLabCompanion.Globals;

public sealed class HealHookGlobalProjectile : GlobalProjectile
{
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (projectile.type != ProjectileID.SpiritHeal) return;
        if (source is not IEntitySource_OnHit onHit) return;
        if (onHit.Attacker is not Projectile sourceProjectile) return;

        var sourceProv = sourceProjectile.GetGlobalProjectile<CombatProvenanceGlobalProjectile>();
        var casterId = sourceProv.SourceEntityId;
        if (casterId is null) return;

        var targetPlayerSlot = (int)projectile.ai[0];
        if (targetPlayerSlot < 0 || targetPlayerSlot >= Main.maxPlayers) return;

        var targetPlayer = Main.player[targetPlayerSlot];
        if (!targetPlayer.active) return;

        var targetId = EntityRegistry.Resolve(targetPlayer);
        if (targetId is null) return;

        var rawHeal = (int)projectile.ai[1];
        var missing = Math.Max(0, targetPlayer.statLifeMax2 - targetPlayer.statLife);
        var effectiveHeal = Math.Min(rawHeal, missing);
        var overheal = rawHeal - effectiveHeal;

        Tracking.Emit(EventType.Heal, new HealData
        {
            Actor = casterId,
            Target = targetId,
            Amount = effectiveHeal,
            Kind = HealKind.SetBonus,
            Overheal = overheal > 0 ? overheal : (int?)null,
            Via = new Via
            {
                Kind = ViaKind.ArmorSet,
                Name = "Spectre Hood",
                Intermediate = new Via
                {
                    Kind = ViaKind.Projectile,
                    Name = Lang.GetProjectileName(projectile.type).Value,
                    Owner = casterId,
                },
            },
            Position = new Position { X = projectile.position.X, Y = projectile.position.Y },
        }, cause: sourceProv.LastDamageSeq);
    }
}
