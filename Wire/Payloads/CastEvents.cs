using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BattleLabCompanion.Wire.Payloads;

public sealed record CastBeginData
{
    public required string Id { get; init; }
    public required string Actor { get; init; }
    public required string Ability { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Position? Position { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}

public sealed record CastEndData
{
    public required string Id { get; init; }
    public required CastEndReason Reason { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}
