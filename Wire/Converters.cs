using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BattleLabCompanion.Wire;

internal sealed class SafeDoubleConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetDouble();

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        => writer.WriteNumberValue(double.IsFinite(value) ? value : 0);
}

internal sealed class SafeFloatConverter : JsonConverter<float>
{
    public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetSingle();

    public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        => writer.WriteNumberValue(float.IsFinite(value) ? value : 0f);
}

internal sealed class SafeNullableDoubleConverter : JsonConverter<double?>
{
    public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Null ? null : reader.GetDouble();

    public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
    {
        if (value is double v && double.IsFinite(v)) writer.WriteNumberValue(v);
        else writer.WriteNullValue();
    }
}

internal sealed class SafeNullableFloatConverter : JsonConverter<float?>
{
    public override float? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Null ? null : reader.GetSingle();

    public override void Write(Utf8JsonWriter writer, float? value, JsonSerializerOptions options)
    {
        if (value is float v && float.IsFinite(v)) writer.WriteNumberValue(v);
        else writer.WriteNullValue();
    }
}

internal static class SafeNumber
{
    public static JsonNode? Json(float v) => float.IsFinite(v) ? JsonValue.Create(v) : null;
    public static JsonNode? Json(double v) => double.IsFinite(v) ? JsonValue.Create(v) : null;
}
