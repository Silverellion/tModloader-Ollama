using Microsoft.Xna.Framework;
using Terraria;

namespace OllamaPlayer.Content.Npc.OllamaNpc;

public class OllamaNpcActionsAi
{
    private bool isAIChanged = false; 
    private int aiTimer = 0;
    private NPC _targetNpc = null;
    private const int AiDuration = 180;
    
    public void FightStrongestEnemy(NPC ollama)
    {
        _targetNpc = FindStrongestEnemy(ollama);

        if (_targetNpc != null)
        {
            isAIChanged = true;
            aiTimer = AiDuration;
            ollama.aiStyle = -1; // Disable default AI
            
            Vector2 direction = _targetNpc.Center - ollama.Center;
            direction.Normalize();
            ollama.velocity = direction * 4f;
        }
    }
    private NPC FindStrongestEnemy(NPC ollama)
    {
        NPC strongest = null;
        float maxHp = 0f;

        foreach (NPC enemy in Main.npc)
        {
            if (enemy.active && !enemy.friendly && Vector2.Distance(ollama.Center, enemy.Center) < OllamaNpcGlobalValues.OllamaNpcSight)
            {
                if (enemy.lifeMax > maxHp)
                {
                    maxHp = enemy.lifeMax;
                    strongest = enemy;
                }
            }
        }

        return strongest;
    }
    
}