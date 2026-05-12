using System.Text.Json.Serialization;

namespace BattleLabCompanion.Wire.Payloads;

public sealed record LootDropData
{
    public required string From { get; init; }
    public required LootItem[] Items { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Position? Position { get; init; }
}

public sealed record LootPickupData
{
    public required string Actor { get; init; }
    public required LootItem[] Items { get; init; }
}
