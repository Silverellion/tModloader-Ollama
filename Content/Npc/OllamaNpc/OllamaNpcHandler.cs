using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OllamaPlayer.Content.Npc.OllamaNpc;

public static class OllamaNpcHandler
{
    public static void Initiate(string rawResponse)
    {
        int npcIndex = GetOllamaNpc();
        OllamaNpcTalk(rawResponse, npcIndex); 
    }
    public static void Initiate(Player player, string rawResponse)
    {
        int npcIndex = GetOllamaNpc();
        if (npcIndex == -1)
        {
            SpawnOllamaNpc(player);
            npcIndex = GetOllamaNpc();
        }

        if (npcIndex != -1) 
            OllamaNpcTalk(rawResponse, npcIndex);
    }

    private static void OllamaNpcTalk(string rawResponse, int npcIndex)
    {
        if (npcIndex < 0 || npcIndex >= Main.npc.Length || !Main.npc[npcIndex].active)
            return;
        List<string> response = StringUtility.Initiate(rawResponse);
        Task.Run(async () =>
        {
            foreach (string responseLine in response)
            {
                OllamaNpcMainProjectile.SetChat(responseLine);
                StringUtility.ChatMessage(responseLine);
                await Task.Delay(800);
            }
        });
    }

    public static void SpawnOllamaNpc(Player player)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient) 
        {
            ModPacket packet = ModContent.GetInstance<OllamaPlayer>().GetPacket();
            packet.Write((byte)OllamaPacketState.OllamaSpawn);
            packet.Write(player.whoAmI); // Player requesting the spawn
            packet.Send();
            return;
        }

        Vector2 spawnPos = player.Center + new Vector2(20, 0);
        int npcIndex = NPC.NewNPC(null, (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<OllamaNpcMainProjectile>());

        // Sync NPC with all clients
        if (npcIndex != -1 && Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
    }
        
    private static int GetOllamaNpc()
    {
        for (int i = 0; i < Main.npc.Length; i++)
        {
            NPC npc = Main.npc[i];
            if (npc.active && npc.type == ModContent.NPCType<OllamaNpcMainProjectile>())
                return i;
        }
        return -1;
    }
}