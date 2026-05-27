using UnityEngine;

// Clears stale ScriptableObject data that persists between Editor Play sessions.
// Safe with saves: LoadFromSave runs after Awake (via SaveApplyHelper), so it
// overwrites this reset with the correct saved values on Continue.
public class SamuraiQuestResetter : MonoBehaviour
{
    [Tooltip("Assign every Samurai quest ScriptableObject here.")]
    public Quest[] quests;

    void Awake()
    {
        foreach (Quest q in quests)
        {
            if (q != null)
                q.ResetQuest();
        }
    }
}
