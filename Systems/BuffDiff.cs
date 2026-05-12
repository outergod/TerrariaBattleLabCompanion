using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;

namespace BattleLabCompanion.Systems;

internal static class BuffDiff
{
    public static void EmitTransitions(Dictionary<int, int> prev, int[] currentTypes, int[] currentTimes, Func<string?> resolveTarget)
    {
        var current = new Dictionary<int, int>(currentTypes.Length);
        for (int i = 0; i < currentTypes.Length; i++)
        {
            var t = currentTypes[i];
            if (t > 0) current[t] = currentTimes[i];
        }

        foreach (var (type, time) in current)
        {
            if (!prev.ContainsKey(type))
            {
                var targetId = resolveTarget();
                if (targetId is not null) EmitApply(targetId, type, time);
            }
        }

        prev.Clear();
        foreach (var (t, time) in current) prev[t] = time;
    }

    private static void EmitApply(string targetId, int buffType, int durationFrames)
    {
        Tracking.Emit(EventType.StatusApply, new StatusApplyData
        {
            Target = targetId,
            Status = Lang.GetBuffName(buffType),
            Kind = Main.debuff[buffType] ? StatusKind.Debuff : StatusKind.Buff,
            DurationMs = durationFrames > 0 ? durationFrames * 1000 / 60 : (int?)null,
            X = new JsonObject
            {
                ["terraria"] = new JsonObject
                {
                    ["buffType"] = buffType,
                    ["durationFrames"] = durationFrames,
                },
            },
        });
    }
}
