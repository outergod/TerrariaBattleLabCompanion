using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CombatTracker.Systems
{
    public class Logging : ModSystem
    {
        private static Logging _singleton;
        private static JsonSerializerOptions _serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
        };

        private StreamWriter _writer;
        private object _lock = new object();

        public override void Load()
        {
            _singleton = this;
        }

        public override void Unload()
        {
            _singleton = null;
        }

        public override void OnWorldLoad()
        {
            Initialize();
        }

        public override void OnWorldUnload()
        {
            ShutDown();
        }

        private void Initialize()
        {
            var player = Main.LocalPlayer.name;
            var world = Main.worldName;
            var target = Path.Combine(Main.SavePath, "Mods", "CombatTracker", player, world);
            Directory.CreateDirectory(target);
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            var log = Path.Combine(target, $"{timestamp}.jsonl");

            try
            {
                _writer = new StreamWriter(log, append: true, encoding: Encoding.UTF8);
                Log("load", new { player, world });
            }
            catch (Exception ex)
            {
                Main.NewText($"[CombatTracker] Failed to initialize Logger: {ex.Message}", Color.Red);
            }
        }

        private void Write(string line)
        {
            Task.Run(() =>
            {
                lock (_lock)
                {
                    try
                    {
                        _writer.WriteLine(line);
                        _writer.Flush();
                    }
                    catch (Exception ex)
                    {
                        Main.NewText($"[CombatTracker] Failed to write log entry: {ex.Message}", Color.Red);
                    }
                }
            });
        }

        private void ShutDown()
        {
            _writer.Flush();
            _writer.Close();
        }

        public static void Log<T>(string @event, T data)
        {
            if (_singleton == null)
                return;

            var o = new
            {
                timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                @event,
                data,
            };

            _singleton.Write(JsonSerializer.Serialize(o, _serializeOptions));
        }
    }
}
