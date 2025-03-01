using OllamaPlayer.Content.Npc.OllamaNpc;

namespace OllamaPlayer.Others;

public class Debug
{
    private static int _timer;
    public static void PrintOllamaAiState()
    {
        if (_timer <= 0)
        {
            StringUtility.DebugMessage(OllamaNpcGlobalValues.AiState.ToString());
            _timer = 300;
        }
        else
            _timer--;
    }
}