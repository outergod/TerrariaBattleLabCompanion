using System.Text.Json.Nodes;
using BattleLabCompanion.Systems;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BattleLabCompanion.Globals;

public sealed class EntityIdGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public string? EntityLocalId { get; private set; }

    public int? PreHitLife;

    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        EntityLocalId = EntityRegistry.Instance?.Mint(EntityKind.Npc);
        if (EntityLocalId is null) return;

        Tracking.Emit(EventType.EntityDeclare, new EntityDeclareData
        {
            Id = EntityLocalId,
            Kind = EntityKind.Npc,
            Name = npc.TypeName,
            MaxHp = npc.lifeMax,
            Position = new Position { X = npc.position.X, Y = npc.position.Y },
            X = new JsonObject
            {
                ["terraria"] = new JsonObject
                {
                    ["npcType"] = npc.type,
                    ["slot"] = npc.whoAmI,
                },
            },
        });
    }
}
