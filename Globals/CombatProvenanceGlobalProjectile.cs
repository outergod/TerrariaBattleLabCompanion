using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CombatTracker.Globals
{
    public class CombatProvenanceGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int? SourcePlayerId;
        public int? SourceNpcId;

        public int? SourceItemType;
        public int? SourceAmmoType;

        public int ProjectileType;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            ProjectileType = projectile.type;

            // Projectile fired by player using an item
            if (source is EntitySource_ItemUse itemUse)
            {
                SourcePlayerId = itemUse.Player?.whoAmI;
                SourceItemType = itemUse.Item?.type;
            }
            // Same but with ammo
            else if (source is EntitySource_ItemUse_WithAmmo itemUseAmmo)
            {
                SourcePlayerId = itemUseAmmo.Player?.whoAmI;
                SourceItemType = itemUseAmmo.Item?.type;
                SourceAmmoType = itemUseAmmo.AmmoItemIdUsed;
            }
            // Projectile spawned by entity
            else if (source is EntitySource_Parent parent)
            {
                if (parent.Entity is Player p) SourcePlayerId = p.whoAmI;
                else if (parent.Entity is NPC n) SourceNpcId = n.whoAmI;
            }
        }
    }
}
