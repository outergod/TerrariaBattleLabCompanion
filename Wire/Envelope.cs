using System.Text.Json.Serialization;

namespace BattleLabCompanion.Wire;

public sealed record Event<TData>
{
    public required string V { get; init; }
    public required string Ts { get; init; }
    public required uint Seq { get; init; }
    public required string SessionId { get; init; }
    public required string Origin { get; init; }
    public required string WorldId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EncounterId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? Cause { get; init; }

    public required string Type { get; init; }
    public required TData Data { get; init; }
}

public static class SchemaVersion
{
    public const string V1 = "1.0";
}

public static class EventType
{
    public const string SessionStart = "session.start";
    public const string SessionEnd = "session.end";

    public const string EncounterStart = "encounter.start";
    public const string EncounterUpdate = "encounter.update";
    public const string EncounterEnd = "encounter.end";

    public const string EntityDeclare = "entity.declare";
    public const string EntityUpdate = "entity.update";
    public const string EntityDeath = "entity.death";

    public const string Damage = "damage";
    public const string Heal = "heal";
    public const string Absorb = "absorb";

    public const string ResourceSnapshot = "resource.snapshot";
    public const string PositionSnapshot = "position.snapshot";

    public const string StatusApply = "status.apply";
    public const string StatusRefresh = "status.refresh";
    public const string StatusStack = "status.stack";
    public const string StatusRemove = "status.remove";

    public const string CastBegin = "cast.begin";
    public const string CastEnd = "cast.end";

    public const string LootDrop = "loot.drop";
    public const string LootPickup = "loot.pickup";

    public const string Chat = "chat";
    public const string SystemMessage = "system.message";
}
