using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BattleLabCompanion.Wire.Payloads;

public sealed record ChatData
{
    public required ChatChannel Channel { get; init; }
    public required string Content { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? From { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? To { get; init; }
}

public sealed record SystemMessageData
{
    public required Severity Severity { get; init; }
    public required string Content { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}
