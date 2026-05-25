using UnityEngine;

// Attach to any EnemySpawner GameObject. Give it a unique spawnerId in the Inspector.
public class SaveableSpawner : MonoBehaviour
{
    [Tooltip("Must be unique within the scene, e.g. 'dun_spawner_1'.")]
    public string spawnerId;

    [HideInInspector] public bool hasTriggered = false;

    public void MarkTriggered()
    {
        if (string.IsNullOrEmpty(spawnerId))
        {
            Debug.LogWarning($"[SaveableSpawner] '{gameObject.name}' has no spawnerId set — won't be tracked.");
            return;
        }
        hasTriggered = true;
    }

    public void RestoreTriggered()
    {
        hasTriggered = true;

        EnemySpawner spawner = GetComponent<EnemySpawner>();
        if (spawner == null) return;

        // If the kill quest is already completed, all enemies were already defeated — don't re-spawn.
        if (spawner.killQuest != null && QuestManager.Instance != null &&
            QuestManager.Instance.IsQuestCompleted(spawner.killQuest.questName))
            return;

        spawner.StartSpawning();
    }
}
