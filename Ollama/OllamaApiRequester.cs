using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria;

namespace OllamaPlayer.Ollama;

public class OllamaApiRequester
{
    private static readonly HttpClient Client = new HttpClient();
    private const string ApiUrl = "http://localhost:11434/api/generate";

    private static List<string> _chatHistory;
    #nullable enable
    public static async Task<string?>  GenerateResponseAsync(string prompt) => await ProcessResponseAsync(prompt);
    public static async Task<string?>  GenerateResponseAsync(string playerName, string prompt)
    {
        string fullPrompt = _chatHistory?.Count > 0 
            ? string.Join("\n", _chatHistory) + $"\n{playerName}: {prompt}" 
            : $"{playerName}: {prompt}";

        string? response = await ProcessResponseAsync(fullPrompt);
        if (response != null)
        {
            _chatHistory ??= new List<string>();
            _chatHistory.Add($"Player {playerName}: {prompt}");
            _chatHistory.Add($"You: {string.Join(" ", response)}");

            if (_chatHistory.Count > 10)
                _chatHistory.RemoveAt(0);
        }

        return response;
    }
    
    private static async Task<string?> ProcessResponseAsync(string prompt)
    {
        try
        {
            var requestData = new
            {
                model = "gemma2",
                prompt,
                stream = false
            };

            string jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await Client.PostAsync(ApiUrl, content);
            response.EnsureSuccessStatusCode(); // Throws if not 2xx status

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(jsonResponse);
            string rawResponse = jsonObject["response"]?.ToString() ?? "No response.";

            return rawResponse;
        }
        catch (Exception ex)
        {
            Main.NewText($"An error occurred! {ex}");
            return null;
        }
    }
}