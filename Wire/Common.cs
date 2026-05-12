using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BattleLabCompanion.Wire;

public sealed record Point2
{
    public required double X { get; init; }
    public required double Y { get; init; }
}

public sealed record Position
{
    public required double X { get; init; }
    public required double Y { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public sbyte? Facing { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Vx { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Vy { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Rotation { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(RectGeometry), "rect")]
[JsonDerivedType(typeof(CircleGeometry), "circle")]
[JsonDerivedType(typeof(ConeGeometry), "cone")]
[JsonDerivedType(typeof(LineGeometry), "line")]
public abstract record Geometry;

public sealed record RectGeometry : Geometry
{
    public required double Width { get; init; }
    public required double Height { get; init; }
    public required Point2 Anchor { get; init; }
}

public sealed record CircleGeometry : Geometry
{
    public required double Radius { get; init; }
    public required Point2 Center { get; init; }
}

public sealed record ConeGeometry : Geometry
{
    public required double Radius { get; init; }
    public required double AngleRad { get; init; }
    public required double Direction { get; init; }
    public required Point2 Origin { get; init; }
}

public sealed record LineGeometry : Geometry
{
    public required double Length { get; init; }
    public required double Width { get; init; }
    public required Point2 Start { get; init; }
    public required Point2 End { get; init; }
}

public sealed record Via
{
    public required ViaKind Kind { get; init; }
    public required string Name { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Owner { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Weapon { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Via? Intermediate { get; init; }
}

public sealed record Mitigation
{
    public required MitigationKind Kind { get; init; }
    public required int Amount { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? By { get; init; }
}

public sealed record ResourceTuple
{
    public required string Kind { get; init; }
    public required int Value { get; init; }
    public required int Max { get; init; }
}

public sealed record PositionEntry
{
    public required string Id { get; init; }
    public required Position Position { get; init; }
}

public sealed record LootItem
{
    public required string Name { get; init; }
    public required int Count { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? X { get; init; }
}

public sealed record ProducerInfo
{
    public required string Name { get; init; }
    public required string Version { get; init; }
}
