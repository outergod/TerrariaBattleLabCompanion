using BattleLabCompanion.Systems;
using BattleLabCompanion.Wire;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BattleLabCompanion.Globals;

public sealed class CombatProvenanceGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public int? SourcePlayerId;
    public int? SourceNpcId;

    public int? SourceItemType;
    public int? SourceAmmoType;

    public int ProjectileType;

    public string? EntityLocalId { get; private set; }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        ProjectileType = projectile.type;

        if (source is EntitySource_ItemUse itemUse)
        {
            SourcePlayerId = itemUse.Player?.whoAmI;
            SourceItemType = itemUse.Item?.type;
        }
        else if (source is EntitySource_ItemUse_WithAmmo itemUseAmmo)
        {
            SourcePlayerId = itemUseAmmo.Player?.whoAmI;
            SourceItemType = itemUseAmmo.Item?.type;
            SourceAmmoType = itemUseAmmo.AmmoItemIdUsed;
        }
        else if (source is EntitySource_Parent parent)
        {
            if (parent.Entity is Player p) SourcePlayerId = p.whoAmI;
            else if (parent.Entity is NPC n) SourceNpcId = n.whoAmI;
        }

        EntityLocalId = EntityRegistry.Instance?.Mint(ClassifyKind(projectile));
    }

    private static EntityKind ClassifyKind(Projectile projectile)
    {
        if (projectile.sentry) return EntityKind.Sentry;
        if (projectile.minion) return EntityKind.Minion;
        return EntityKind.Proj;
    }
}
