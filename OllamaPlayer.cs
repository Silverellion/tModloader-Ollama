using System.IO;
using System.Threading.Tasks;
using OllamaPlayer.Content.Npc.OllamaNpc;
using OllamaPlayer.Ollama;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OllamaPlayer
{
    public enum OllamaPacketState : byte
    {
        OllamaSpawn,
        OllamaDespawn,
        PlayerPrompt,    // Client -> Server: Player sends a message
        OllamaResponse   // Server -> Client: Server replies with LLM response
    }

    public class OllamaPlayer : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            OllamaPacketState packetState = (OllamaPacketState)reader.ReadByte();
            if (packetState == OllamaPacketState.OllamaSpawn && Main.netMode == NetmodeID.Server)
            {
                int playerIndex = reader.ReadInt32();
                Player player = Main.player[playerIndex];

                if (player != null)
                    OllamaNpcHandler.SpawnOllamaNpc(player);
            }

            else if (packetState == OllamaPacketState.OllamaDespawn && Main.netMode == NetmodeID.Server)
            {
                int npcIndex = reader.ReadInt32();
                if (npcIndex >= 0 && npcIndex < Main.maxNPCs)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc != null && npc.active)
                    {
                        npc.active = false;
                        npc.netUpdate = true;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
                    }
                }
            }
            
            else if (packetState == OllamaPacketState.PlayerPrompt && Main.netMode == NetmodeID.Server)
            {
                int playerId = reader.ReadInt32();
                string prompt = reader.ReadString();
                
                Task.Run(async () =>
                {
                    prompt = await OllamaResponse.GetOllamaResponse(prompt, Main.player[playerId]);
                });

                ModPacket responsePacket = ModContent.GetInstance<OllamaPlayer>().GetPacket();
                responsePacket.Write((byte)OllamaPacketState.OllamaResponse);
                responsePacket.Write(playerId);
                responsePacket.Write(prompt); 
                responsePacket.Send();
            }

            else if (packetState == OllamaPacketState.OllamaResponse && Main.netMode == NetmodeID.MultiplayerClient)
            {
                int playerId = reader.ReadInt32();
                string prompt = reader.ReadString();
                Task.Run(async () =>
                {
                    await OllamaResponse.GetOllamaResponse(prompt, Main.player[playerId]);
                });
            }
        }
    }
}
