using System.Collections.Generic;

namespace OllamaPlayer.Content.Npc.OllamaNpc;

public class OllamaNpcOrbProjectileTracker
{
    private static readonly Dictionary<int, HashSet<int>> NpcOrbs = new Dictionary<int, HashSet<int>>();
    private static void EnsureNpcEntry(int npcId)
    {
        if (!NpcOrbs.ContainsKey(npcId))
            NpcOrbs[npcId] = new HashSet<int>();
    }

    public static void RegisterOrb(int npcId, int projId)
    {
        EnsureNpcEntry(npcId);
        NpcOrbs[npcId].Add(projId);
    }

    public static void UnregisterOrb(int npcId, int projId)
    {
        if (NpcOrbs.ContainsKey(npcId))
        {
            NpcOrbs[npcId].Remove(projId);
            if (NpcOrbs[npcId].Count == 0)
                NpcOrbs.Remove(npcId); // Remove entry if no orbs remain
        }
    }

    public static bool OrbsExist(int npcId)
    {
        return NpcOrbs.ContainsKey(npcId) && NpcOrbs[npcId].Count > 0;
    }
}