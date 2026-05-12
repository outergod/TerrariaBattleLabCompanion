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

        foreach (var (type, prevTime) in prev)
        {
            if (!current.ContainsKey(type))
            {
                var targetId = resolveTarget();
                if (targetId is not null) EmitRemove(targetId, type, prevTime);
            }
        }

        prev.Clear();
        foreach (var (t, time) in current) prev[t] = time;
    }

    private static void EmitRemove(string targetId, int buffType, int prevTime)
    {
        Tracking.Emit(EventType.StatusRemove, new StatusRemoveData
        {
            Target = targetId,
            Status = Lang.GetBuffName(buffType),
            Kind = Main.debuff[buffType] ? StatusKind.Debuff : StatusKind.Buff,
            Reason = prevTime <= 1 ? StatusRemoveReason.Expired : StatusRemoveReason.Removed,
            X = new JsonObject
            {
                ["terraria"] = new JsonObject
                {
                    ["buffType"] = buffType,
                    ["prevFramesRemaining"] = prevTime,
                },
            },
        });
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
