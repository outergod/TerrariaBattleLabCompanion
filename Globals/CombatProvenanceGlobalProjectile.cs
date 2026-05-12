using System.Text.Json.Nodes;
using BattleLabCompanion.Systems;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BattleLabCompanion.Globals;

public sealed class CombatProvenanceGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public string? SourceEntityId;

    public int? SourceItemType;
    public int? SourceAmmoType;

    public int ProjectileType;

    public EntityKind Kind { get; private set; }
    public string? EntityLocalId { get; private set; }

    public uint? LastDamageSeq;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        ProjectileType = projectile.type;
        Kind = ClassifyKind(projectile);

        if (source is EntitySource_ItemUse itemUse)
        {
            SourceEntityId = EntityRegistry.Resolve(itemUse.Player);
            SourceItemType = itemUse.Item?.type;
        }
        else if (source is EntitySource_ItemUse_WithAmmo itemUseAmmo)
        {
            SourceEntityId = EntityRegistry.Resolve(itemUseAmmo.Player);
            SourceItemType = itemUseAmmo.Item?.type;
            SourceAmmoType = itemUseAmmo.AmmoItemIdUsed;
        }
        else if (source is EntitySource_Parent parent)
        {
            SourceEntityId = parent.Entity switch
            {
                Player p => EntityRegistry.Resolve(p),
                NPC n => EntityRegistry.Resolve(n),
                Projectile pj => PropagateFromParentProjectile(pj),
                _ => null,
            };
        }

        EntityLocalId = EntityRegistry.Instance?.Mint(Kind);

        if (Kind is EntityKind.Minion or EntityKind.Sentry or EntityKind.Pet)
        {
            EmitDeclare(projectile);
        }
    }

    private string? PropagateFromParentProjectile(Projectile parent)
    {
        var parentProv = parent.GetGlobalProjectile<CombatProvenanceGlobalProjectile>();
        SourceItemType ??= parentProv.SourceItemType;
        SourceAmmoType ??= parentProv.SourceAmmoType;
        return parentProv.EntityLocalId;
    }

    private void EmitDeclare(Projectile projectile)
    {
        if (EntityLocalId is null) return;

        Tracking.Emit(EventType.EntityDeclare, new EntityDeclareData
        {
            Id = EntityLocalId,
            Kind = Kind,
            Name = Lang.GetProjectileName(projectile.type).Value,
            Parent = SourceEntityId,
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
                    ["slot"] = projectile.whoAmI,
                },
            },
        });
    }

    private static EntityKind ClassifyKind(Projectile projectile)
    {
        if (projectile.sentry) return EntityKind.Sentry;
        if (projectile.minion) return EntityKind.Minion;
        return EntityKind.Proj;
    }
}
