using System.Collections.Generic;
using BattleLabCompanion.Systems;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BattleLabCompanion.Globals;

public sealed class LootHookGlobalItem : GlobalItem
{
    private static readonly Dictionary<int, uint> ItemSlotToDropSeq = new();

    public override void OnSpawn(Item item, IEntitySource source)
    {
        if (source is not EntitySource_Loot loot) return;
        if (loot.Entity is not NPC npc) return;

        var from = EntityRegistry.Resolve(npc);
        if (from is null) return;

        var seq = Tracking.Emit(EventType.LootDrop, new LootDropData
        {
            From = from,
            Items = new[]
            {
                new LootItem { Name = Lang.GetItemNameValue(item.type), Count = item.stack },
            },
            Position = new Position { X = item.position.X, Y = item.position.Y },
        });

        ItemSlotToDropSeq[item.whoAmI] = seq;
    }

    public override bool OnPickup(Item item, Player player)
    {
        var actor = EntityRegistry.Resolve(player);
        if (actor is null) return true;

        uint? cause = ItemSlotToDropSeq.TryGetValue(item.whoAmI, out var s) ? s : null;
        if (cause.HasValue) ItemSlotToDropSeq.Remove(item.whoAmI);

        Tracking.Emit(EventType.LootPickup, new LootPickupData
        {
            Actor = actor,
            Items = new[]
            {
                new LootItem { Name = Lang.GetItemNameValue(item.type), Count = item.stack },
            },
        }, cause: cause);

        return true;
    }
}
