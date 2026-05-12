using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BattleLabCompanion.Wire.Payloads;

public sealed record EncounterStartData
{
    public required string Id { get; init; }
    public required EncounterKind Kind { get; init; }
    public required string Name { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Members { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}

public sealed record EncounterUpdateData
{
    public required string Id { get; init; }

    [JsonPropertyName("members.add")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? MembersAdd { get; init; }

    [JsonPropertyName("members.remove")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? MembersRemove { get; init; }
}

public sealed record EncounterEndData
{
    public required string Id { get; init; }
    public required EncounterOutcome Outcome { get; init; }
    public required EncounterEndCause Cause { get; init; }
}
