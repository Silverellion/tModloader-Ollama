using System.Threading.Tasks;
using OllamaPlayer.Content.Npc.OllamaNpc;
using OllamaPlayer.Ollama;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OllamaPlayer.Content.Commands;

public class OllamaCommandTalk : ModCommand
{
        public override CommandType Type => CommandType.Chat;
        public override string Command => "talk";
        public override string Description => "Talk to Master O'Llama";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
                ProcessTalkCommand.Initiate(caller, args);
        }
}

public class OllamaCommandT : ModCommand
{
        public override CommandType Type => CommandType.Chat;
        public override string Command => "t";
        public override string Description => "Talk to Master O'Llama";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
                ProcessTalkCommand.Initiate(caller, args);
        }
}

public class KillOllamaNpcCommand : ModCommand
{
        public override CommandType Type => CommandType.Chat; 
        public override string Command => "killollama"; 
        public override string Description => "Kill Master O'Llama";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
                foreach (NPC npc in Main.npc)
                {
                        if (npc.active && npc.type == ModContent.NPCType<OllamaNpcMainProjectile>())
                        {
                                NPC.HitInfo hitInfo = new NPC.HitInfo();
                                hitInfo.Damage = npc.life + 1;
                                npc.StrikeNPC(hitInfo, true, true);
                                NetMessage.SendStrikeNPC(npc, hitInfo);
                                SoundEngine.PlaySound(SoundID.NPCDeath1, npc.Center);
                                
                                if (Main.netMode == NetmodeID.Server)
                                {
                                        ModPacket packet = ModContent.GetInstance<OllamaPlayer>().GetPacket();
                                        packet.Write((byte)OllamaPacketState.OllamaDespawn);
                                        packet.Write(npc.whoAmI);
                                        packet.Send();
                                }
                        }
                }
        }
}

public static class ProcessTalkCommand
{
        public static void Initiate(CommandCaller caller, string[] args)
        {
                string prompt = string.Join(" ", args);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                        ModPacket packet = ModContent.GetInstance<OllamaPlayer>().GetPacket();
                        packet.Write((byte)OllamaPacketState.PlayerPrompt);
                        
                        packet.Write(caller.Player.whoAmI);
                        packet.Write(prompt);
                        packet.Send();
                }
                else
                {
                        Task.Run(async () =>
                        {
                                await OllamaResponse.GetOllamaResponse(prompt, caller.Player);
                        });
                }       
        }
}