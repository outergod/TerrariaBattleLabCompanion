using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BattleLabCompanion.Wire.Payloads;

public sealed record DamageData
{
    public required string Actor { get; init; }
    public required string Target { get; init; }
    public required int Amount { get; init; }
    public required bool Crit { get; init; }
    public required DamageKind Kind { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Tags { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Overkill { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Mitigation[]? Mitigation { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Via? Via { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CastId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Position? Position { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Geometry? Geometry { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}

public sealed record HealData
{
    public required string Actor { get; init; }
    public required string Target { get; init; }
    public required int Amount { get; init; }
    public required HealKind Kind { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Overheal { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Via? Via { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CastId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Position? Position { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}

public sealed record AbsorbData
{
    public required string Actor { get; init; }
    public required int Amount { get; init; }
    public required string Source { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Via? Via { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}
