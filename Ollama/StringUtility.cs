using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OllamaPlayer.Ollama;

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
}