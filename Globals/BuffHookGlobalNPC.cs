using System.Collections.Generic;
using BattleLabCompanion.Systems;
using Terraria;
using Terraria.ModLoader;

namespace BattleLabCompanion.Globals;

public sealed class BuffHookGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    private readonly Dictionary<int, int> _prevBuffs = new();

    public override void PostAI(NPC npc)
    {
        BuffDiff.EmitTransitions(_prevBuffs, npc.buffType, npc.buffTime, () => EntityRegistry.Resolve(npc));
    }
}
