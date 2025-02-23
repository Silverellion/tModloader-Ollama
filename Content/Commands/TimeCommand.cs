using Terraria;
using Terraria.ModLoader;

namespace OllamaPlayer.Content.Commands;

public class TimeCommand : ModCommand
{
    public override string Command => "time";
    public override CommandType Type => CommandType.Chat;
    public override string Usage => "/time <day/night>";
    public override string Description => "Set the time to day or night.";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length < 1)
        {
            caller.Reply("Usage: /time <day/night>");
            return;
        }

        string timeOption = args[0].ToLower();
        if (timeOption == "day")
        {
            Main.dayTime = true;
            Main.time = 0;
        }
        else if (timeOption == "night")
        {
            Main.dayTime = false;
            Main.time = 0;
        }
        else
        {
            caller.Reply("Invalid option! Use /time day or /time night.");
        }
    }
}
