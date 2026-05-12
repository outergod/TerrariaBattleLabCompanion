using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BattleLabCompanion.Systems;

public sealed class Tracking : ModSystem
{
    private static Tracking? _singleton;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower),
            new SafeDoubleConverter(),
            new SafeFloatConverter(),
            new SafeNullableDoubleConverter(),
            new SafeNullableFloatConverter(),
        },
    };

    private const string TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    private const string LogFilenameFormat = "yyyy-MM-ddTHH-mm-ssZ";

    private StreamWriter? _writer;
    private string _sessionId = "";
    private string _origin = "";
    private string _worldId = "";
    private uint _seq;
    private readonly object _lock = new();

    public override void Load() => _singleton = this;
    public override void Unload() => _singleton = null;

    public override void OnWorldLoad()
    {
        Initialize();
        EmitSessionStart();
    }

    public override void OnWorldUnload()
    {
        EmitSessionEnd(SessionEndReason.WorldExit);
        Shutdown();
    }

    private void Initialize()
    {
        _sessionId = $"sess:{Guid.NewGuid():N}";
        _seq = 0;
        _worldId = $"world:{Main.ActiveWorldFileData.UniqueId:N}";
        _origin = ResolveOrigin();

        var player = Main.LocalPlayer.name;
        var world = Main.worldName;
        var target = Path.Combine(Main.SavePath, "Mods", "BattleLabCompanion", player, world);
        Directory.CreateDirectory(target);
        var timestamp = DateTime.UtcNow.ToString(LogFilenameFormat, CultureInfo.InvariantCulture);
        var log = Path.Combine(target, $"{timestamp}.jsonl");

        try
        {
            _writer = new StreamWriter(log, append: true, encoding: Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Main.NewText($"[BattleLabCompanion] Failed to open log: {ex.Message}", Color.Red);
        }
    }

    private void Shutdown()
    {
        lock (_lock)
        {
            _writer?.Flush();
            _writer?.Dispose();
            _writer = null;
        }
    }

    private static string ResolveOrigin()
    {
        if (Main.netMode == NetmodeID.Server)
            return "server";

        var localId = EntityRegistry.Resolve(Main.LocalPlayer);
        var stripped = localId is null ? "0" : StripKindPrefix(localId);
        return $"client:{stripped}";
    }

    private static string StripKindPrefix(string id)
    {
        var colon = id.IndexOf(':');
        return colon < 0 ? id : id[(colon + 1)..];
    }

    public static uint Emit<TData>(string type, TData data, uint? cause = null, string? encounterId = null)
    {
        var s = _singleton;
        if (s is null) return 0;

        lock (s._lock)
        {
            if (s._writer is null) return 0;

            uint seq = s._seq++;
            var ev = new Event<TData>
            {
                V = SchemaVersion.V1,
                Ts = DateTime.UtcNow.ToString(TimestampFormat, CultureInfo.InvariantCulture),
                Seq = seq,
                SessionId = s._sessionId,
                Origin = s._origin,
                WorldId = s._worldId,
                EncounterId = encounterId ?? EncounterTracker.ActiveEncounterId,
                Cause = cause,
                Type = type,
                Data = data!,
            };

            try
            {
                s._writer.WriteLine(JsonSerializer.Serialize(ev, JsonOpts));
                // Per spec D1: flush after every WriteLine. The tailer-based
                // ingest assumes sub-100ms visibility; StreamWriter.Flush()
                // pushes the char buffer through to the FileStream and then
                // calls FileStream.Flush() so inotify/fsnotify watchers fire
                // on the next OS scheduling slice.
                s._writer.Flush();
            }
            catch (Exception ex)
            {
                Main.NewText($"[BattleLabCompanion] Write failed: {ex.Message}", Color.Red);
            }

            return seq;
        }
    }

    private static void EmitSessionStart() => Emit(EventType.SessionStart, new SessionStartData
    {
        Producer = new ProducerInfo
        {
            Name = "BattleLabCompanion",
            Version = ModContent.GetInstance<BattleLabCompanion>().Version.ToString(),
        },
        GameVersion = $"Terraria {Main.versionNumber}",
    });

    private static void EmitSessionEnd(SessionEndReason reason) =>
        Emit(EventType.SessionEnd, new SessionEndData { Reason = reason });
}
