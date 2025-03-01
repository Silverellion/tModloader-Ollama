using System.Threading.Tasks;
using OllamaPlayer.Ollama;

namespace OllamaPlayer.Content.Npc.OllamaNpc;

public class OllamaNpcActions
{
    public static void DetectEnemy(string enemyName)
    {
        string enemyDetection = StringUtility.GetEnemyDetectionMessage(enemyName);
        Task.Run(async () =>
        {
            string responseToDetection = await HandlePromptSilent(enemyDetection);
            StringUtility.DebugMessage(enemyDetection);
            string motiveConfirmation = await HandlePromptSilent(StringUtility.GetMotiveConfirmationMessage(responseToDetection));
            
            StringUtility.ChatMessage(responseToDetection);
            StringUtility.DebugMessage(motiveConfirmation);
            
            if (motiveConfirmation.Contains("1") || motiveConfirmation.ToLower().Contains("flee"))
                OllamaNpcGlobalValues.AiState = OllamaAiState.Flee;
            if (motiveConfirmation.Contains("2") || motiveConfirmation.ToLower().Contains("fight"))
                OllamaNpcGlobalValues.AiState = OllamaAiState.Fight;
        });
    }
    
    private static async Task<string> HandlePromptSilent(string prompt) => await OllamaResponse.GetOllamaResponseSilent(prompt);
}