using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BattleLabCompanion.Wire.Payloads;

public sealed record StatusApplyData
{
    public required string Target { get; init; }
    public required string Status { get; init; }
    public required StatusKind Kind { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? By { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DurationMs { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Stacks { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}

public sealed record StatusRefreshData
{
    public required string Target { get; init; }
    public required string Status { get; init; }
    public required StatusKind Kind { get; init; }
    public required int DurationMs { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? By { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Stacks { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}

public sealed record StatusStackData
{
    public required string Target { get; init; }
    public required string Status { get; init; }
    public required StatusKind Kind { get; init; }
    public required int Stacks { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? By { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}

public sealed record StatusRemoveData
{
    public required string Target { get; init; }
    public required string Status { get; init; }
    public required StatusKind Kind { get; init; }
    public required StatusRemoveReason Reason { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? By { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}
