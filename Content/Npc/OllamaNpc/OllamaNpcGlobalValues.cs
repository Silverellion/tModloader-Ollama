namespace OllamaPlayer.Content.Npc.OllamaNpc;

public enum OllamaAiState
{
    Default = 0,
    Flee = 1,
    Fight = 2,
}

public static class OllamaNpcGlobalValues
{
    public static readonly float OllamaNpcSight = 600f;
    public static OllamaAiState AiState { get; set; }
}