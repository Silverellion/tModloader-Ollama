using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OllamaPlayer.Content;

public class ChatMessageHandler : ModSystem
{
    private string _message = string.Empty;
    public override void PreUpdateEntities()
    {
        if (!string.IsNullOrWhiteSpace(Main.chatText))
        {
            _message = Main.chatText;
        }
        if (Main.inputTextEnter && !string.IsNullOrWhiteSpace(_message))
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(_message, Color.Yellow);

                Main.chatText = string.Empty;
                _message = string.Empty;
                Main.inputTextEnter = false; 
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                StringUtility.DebugMessage(_message);
            }
        }
    }
}
