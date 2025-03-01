using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria.Chat;
using Terraria.Localization;

namespace OllamaPlayer;

public static class StringUtility
{
    private static List<string> _splitMessage;
    public static List<string> Initiate(string input) => SplitMessage(RemoveEmojis(input));
    private static string RemoveEmojis(string input) => Regex.Replace(input, @"\p{Cs}", "");
    private static List<string> SplitMessage(string input)
    {
        _splitMessage = new List<string>();
        if (string.IsNullOrEmpty(input))
            return _splitMessage;
        foreach (string line in input.Split("\n"))
        {
            if(!string.IsNullOrEmpty(line))
                _splitMessage.Add(line);
        }
        return _splitMessage;
    }

    public static void DebugMessage(string message)
    {
        string debugMessage = "[DEBUG] " + message;
        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(debugMessage), Color.Yellow);
    }

    public static void ChatMessage(string message)
    {
        string debugMessage = "<Ollama> " + message;
        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(debugMessage), Color.White);
    }

    public static string GetEnemyDetectionMessage(string enemyName) => "You are an NPC in Terraria and you see "
                                                                       + enemyName
                                                                       + ", what will you do? I suggest we fight!\n"
                                                                       + "Write a short response under 50 words";
    public static string GetMotiveConfirmationMessage(string responseToDetection) => "Do you think this sentence\n"
                                                                                     + responseToDetection
                                                                                     + "implies the sender is\n"
                                                                                     + "1. avoiding the threat\n"
                                                                                     + "2. fighting the threat\n"
                                                                                     + "respond in 1 or 2";
}