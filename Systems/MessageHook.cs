using BattleLabCompanion.Wire;
using BattleLabCompanion.Wire.Payloads;
using Terraria;
using Terraria.Chat;
using Terraria.Chat.Commands;
using Terraria.ModLoader;

namespace BattleLabCompanion.Systems;

public sealed class MessageHook : ModSystem
{
    public override void Load()
    {
        On_Main.NewText_string_byte_byte_byte += OnNewText;
        On_ChatCommandProcessor.ProcessIncomingMessage += OnIncomingChat;
    }

    public override void Unload()
    {
        On_Main.NewText_string_byte_byte_byte -= OnNewText;
        On_ChatCommandProcessor.ProcessIncomingMessage -= OnIncomingChat;
    }

    private static void OnNewText(On_Main.orig_NewText_string_byte_byte_byte orig, string newText, byte r, byte g, byte b)
    {
        orig(newText, r, g, b);
        if (string.IsNullOrEmpty(newText)) return;

        Tracking.Emit(EventType.SystemMessage, new SystemMessageData
        {
            Severity = ClassifySeverity(r, g, b),
            Content = newText,
        });
    }

    private static void OnIncomingChat(On_ChatCommandProcessor.orig_ProcessIncomingMessage orig, ChatCommandProcessor self, ChatMessage message, int clientId)
    {
        orig(self, message, clientId);
        if (string.IsNullOrEmpty(message.Text)) return;

        var from = clientId >= 0 && clientId < Main.maxPlayers
            ? EntityRegistry.Resolve(Main.player[clientId])
            : null;

        Tracking.Emit(EventType.Chat, new ChatData
        {
            Channel = ChatChannel.Say,
            From = from,
            Content = message.Text,
        });
    }

    private static Severity ClassifySeverity(byte r, byte g, byte b)
    {
        if (r > 200 && g < 100 && b < 100) return Severity.Error;
        if (r > 200 && g > 200 && b < 100) return Severity.Warn;
        return Severity.Info;
    }
}
