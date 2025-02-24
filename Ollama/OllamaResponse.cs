using System.Collections.Generic;
using System.Threading.Tasks;
using OllamaPlayer.Content.Npc.OllamaNpc;
using Terraria;
namespace OllamaPlayer.Ollama;

public static class OllamaResponse
{
#nullable enable
    public static async Task<string> GetOllamaResponse(string prompt)
        => await ProcessOllamaResponse(prompt);

    public static async Task<string> GetOllamaResponse(string prompt, Player player)
        => await ProcessOllamaResponse(prompt, player);
    public static async Task<string> GetOllamaResponseSilent(string prompt) 
        => await OllamaApiRequester.GenerateResponseAsync(prompt) ?? "No response.";

    private static async Task<string> ProcessOllamaResponse(string prompt, Player? player = null)
    {
        string? rawResponse = await (player != null
            ? OllamaApiRequester.GenerateResponseAsync(player.name, prompt)
            : OllamaApiRequester.GenerateResponseAsync(prompt));

        if (string.IsNullOrEmpty(rawResponse))
            return "No response.";

        List<string> response = StringUtility.Initiate(rawResponse);
        string fullResponse = string.Join(" ", response);

        if (player != null)
            OllamaNpcHandler.Initiate(player, rawResponse);
        else
            OllamaNpcHandler.Initiate(rawResponse);
        return fullResponse;
    }
}


