using BattleLabCompanion.Systems;
using BattleLabCompanion.Wire;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BattleLabCompanion.Globals;

public sealed class EntityIdGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public string? EntityLocalId { get; private set; }

    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        EntityLocalId = EntityRegistry.Instance?.Mint(EntityKind.Npc);
    }
}
