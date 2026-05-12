using System;
using System.Collections.Generic;
using BattleLabCompanion.Globals;
using BattleLabCompanion.Wire;
using Terraria;
using Terraria.ModLoader;

namespace BattleLabCompanion.Systems;

public sealed class EntityRegistry : ModSystem
{
    private static EntityRegistry? _singleton;

    private readonly Dictionary<EntityKind, uint> _counters = new();
    private readonly Dictionary<int, string> _playerIds = new();

    public override void Load() => _singleton = this;
    public override void Unload() => _singleton = null;

    public override void OnWorldLoad() => Reset();
    public override void OnWorldUnload() => Reset();

    private void Reset()
    {
        _counters.Clear();
        _playerIds.Clear();
    }

    internal string Mint(EntityKind kind)
    {
        _counters.TryGetValue(kind, out uint n);
        _counters[kind] = n + 1;
        return $"{KindPrefix(kind)}:{n}";
    }

    public static string? Resolve(Player? player)
    {
        if (_singleton is null || player is null) return null;
        if (_singleton._playerIds.TryGetValue(player.whoAmI, out var existing)) return existing;
        var id = _singleton.Mint(EntityKind.Player);
        _singleton._playerIds[player.whoAmI] = id;
        return id;
    }

    public static string? Resolve(NPC? npc)
    {
        if (npc is null) return null;
        return npc.GetGlobalNPC<EntityIdGlobalNPC>().EntityLocalId;
    }

    public static string? Resolve(Projectile? proj)
    {
        if (proj is null) return null;
        return proj.GetGlobalProjectile<CombatProvenanceGlobalProjectile>().EntityLocalId;
    }

    internal static EntityRegistry? Instance => _singleton;

    private static string KindPrefix(EntityKind kind) => kind switch
    {
        EntityKind.Player => "player",
        EntityKind.Npc => "npc",
        EntityKind.Proj => "proj",
        EntityKind.Minion => "minion",
        EntityKind.Sentry => "sentry",
        EntityKind.Pet => "pet",
        EntityKind.Env => "env",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };
}

public static class EnvIds
{
    public const string Lava = "env:lava";
    public const string DartTrap = "env:dart-trap";
    public const string SpikeBall = "env:spike-ball";
    public const string Spike = "env:spike";
    public const string Drowning = "env:drowning";
    public const string Fall = "env:fall";
    public const string Suffocation = "env:suffocation";
    public const string Electrified = "env:electrified";
    public const string Demonscythe = "env:demon-altar";
    public const string Unknown = "env:unknown";
}
