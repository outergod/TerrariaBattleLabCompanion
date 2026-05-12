using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BattleLabCompanion.Systems;

public sealed class EncounterTracker : ModSystem
{
    private static EncounterTracker? _singleton;

    private static readonly Dictionary<int, int> BossFamily = new()
    {
        { NPCID.Retinazer, NPCID.Retinazer },
        { NPCID.Spazmatism, NPCID.Retinazer },

        { NPCID.SkeletronPrime, NPCID.SkeletronPrime },
        { NPCID.PrimeCannon, NPCID.SkeletronPrime },
        { NPCID.PrimeSaw, NPCID.SkeletronPrime },
        { NPCID.PrimeVice, NPCID.SkeletronPrime },
        { NPCID.PrimeLaser, NPCID.SkeletronPrime },

        { NPCID.EaterofWorldsHead, NPCID.EaterofWorldsHead },
        { NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead },
        { NPCID.EaterofWorldsTail, NPCID.EaterofWorldsHead },

        { NPCID.TheDestroyer, NPCID.TheDestroyer },
        { NPCID.TheDestroyerBody, NPCID.TheDestroyer },
        { NPCID.TheDestroyerTail, NPCID.TheDestroyer },
    };

    private sealed class Active
    {
        public required string Id;
        public required string Name;
        public required HashSet<string> Members;
    }

    private readonly Dictionary<int, Active> _byFamily = new();
    private readonly Dictionary<string, int> _memberToFamily = new();
    private readonly Stack<string> _stack = new();

    public static string? ActiveEncounterId =>
        _singleton is { } s && s._stack.Count > 0 ? s._stack.Peek() : null;

    public override void Load() => _singleton = this;
    public override void Unload() => _singleton = null;

    public override void OnWorldLoad()
    {
        _byFamily.Clear();
        _memberToFamily.Clear();
        _stack.Clear();
    }

    public override void OnWorldUnload()
    {
        foreach (var active in new List<Active>(_byFamily.Values))
        {
            EmitEnd(active.Id, EncounterOutcome.Abandoned, EncounterEndCause.WorldExit);
        }
        _byFamily.Clear();
        _memberToFamily.Clear();
        _stack.Clear();
    }

    public static void OnBossSpawn(NPC npc, string entityLocalId)
    {
        if (_singleton is not { } s || !npc.boss) return;

        var family = BossFamily.TryGetValue(npc.type, out var f) ? f : npc.type;

        if (s._byFamily.TryGetValue(family, out var active))
        {
            active.Members.Add(entityLocalId);
            s._memberToFamily[entityLocalId] = family;
            Tracking.Emit(EventType.EncounterUpdate, new EncounterUpdateData
            {
                Id = active.Id,
                MembersAdd = new[] { entityLocalId },
            });
            return;
        }

        var encounterId = $"enc:{Guid.NewGuid():N}";
        var name = npc.TypeName;
        active = new Active
        {
            Id = encounterId,
            Name = name,
            Members = new HashSet<string> { entityLocalId },
        };
        s._byFamily[family] = active;
        s._memberToFamily[entityLocalId] = family;
        s._stack.Push(encounterId);

        Tracking.Emit(EventType.EncounterStart, new EncounterStartData
        {
            Id = encounterId,
            Kind = EncounterKind.Boss,
            Name = name,
            Members = new[] { entityLocalId },
            X = new JsonObject
            {
                ["terraria"] = new JsonObject
                {
                    ["bossNpcType"] = npc.type,
                    ["family"] = family,
                },
            },
        });
    }

    public static void OnNpcKilled(string entityLocalId)
    {
        if (_singleton is not { } s) return;
        if (!s._memberToFamily.TryGetValue(entityLocalId, out var family)) return;
        if (!s._byFamily.TryGetValue(family, out var active)) return;

        active.Members.Remove(entityLocalId);
        s._memberToFamily.Remove(entityLocalId);

        if (active.Members.Count == 0)
        {
            EmitEnd(active.Id, EncounterOutcome.Victory, EncounterEndCause.AllMembersDead);
            s._byFamily.Remove(family);
            RemoveFromStack(s._stack, active.Id);
        }
    }

    private static void EmitEnd(string id, EncounterOutcome outcome, EncounterEndCause cause)
    {
        Tracking.Emit(EventType.EncounterEnd, new EncounterEndData
        {
            Id = id,
            Outcome = outcome,
            Cause = cause,
        });
    }

    private static void RemoveFromStack(Stack<string> stack, string id)
    {
        var remaining = new List<string>();
        while (stack.Count > 0)
        {
            var top = stack.Pop();
            if (top != id) remaining.Add(top);
        }
        for (int i = remaining.Count - 1; i >= 0; i--) stack.Push(remaining[i]);
    }
}
