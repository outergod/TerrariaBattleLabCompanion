using System.Text.Json.Nodes;
using BattleLabCompanion.Systems;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;
using Terraria.ModLoader;

namespace BattleLabCompanion.Globals;

public sealed class DamageHookGlobalProjectile : GlobalProjectile
{
    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        var prov = projectile.GetGlobalProjectile<CombatProvenanceGlobalProjectile>();
        var targetId = EntityRegistry.Resolve(target);
        if (prov.SourceEntityId is null || targetId is null) return;

        var overkill = target.GetGlobalNPC<DamageHookGlobalNPC>().ComputeOverkill(damageDone);

        var seq = Tracking.Emit(EventType.Damage, new DamageData
        {
            Actor = prov.SourceEntityId,
            Target = targetId,
            Amount = damageDone,
            Crit = hit.Crit,
            Kind = DamageKind.Hit,
            Overkill = overkill,
            CastId = prov.CastId,
            Via = BuildVia(projectile, prov),
            Position = new Position
            {
                X = projectile.position.X,
                Y = projectile.position.Y,
                Vx = projectile.velocity.X,
                Vy = projectile.velocity.Y,
                Rotation = projectile.rotation,
            },
            X = new JsonObject
            {
                ["terraria"] = new JsonObject
                {
                    ["projectileType"] = projectile.type,
                    ["itemType"] = prov.SourceItemType,
                    ["hitDirection"] = hit.HitDirection,
                    ["knockback"] = SafeNumber.Json(hit.Knockback),
                    ["instantKill"] = hit.InstantKill,
                },
            },
        });

        prov.LastDamageSeq = seq;
    }

    private static Via BuildVia(Projectile projectile, CombatProvenanceGlobalProjectile prov) => new()
    {
        Kind = ViaKind.Projectile,
        Name = Lang.GetProjectileName(projectile.type).Value,
        Id = prov.EntityLocalId,
        Owner = prov.SourceEntityId,
        Weapon = prov.SourceItemType is int it ? Lang.GetItemNameValue(it) : null,
    };

}
