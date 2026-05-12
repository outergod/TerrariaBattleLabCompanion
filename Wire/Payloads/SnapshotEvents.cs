namespace BattleLabCompanion.Wire.Payloads;

public sealed record ResourceSnapshotData
{
    public required string Entity { get; init; }
    public required ResourceTuple[] Resources { get; init; }
}

public sealed record PositionSnapshotData
{
    public required PositionEntry[] Entities { get; init; }
}
