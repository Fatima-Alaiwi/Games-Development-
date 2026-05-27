using UnityEngine;

public class QuestZoneTrigger : MonoBehaviour
{
    public EnemySpawner spawner;
    public float triggerDistance = 8f;

    [Tooltip("Spawner only triggers after this quest is completed.")]
    public Quest requiredCompletedQuest;

    private bool triggered = false;

    void Update()
    {
        if (triggered) return;

        if (requiredCompletedQuest != null)
        {
            if (QuestManager.Instance == null) return;
            if (!QuestManager.Instance.IsQuestCompleted(requiredCompletedQuest.questName)) return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        if (Vector3.Distance(transform.position, player.transform.position) < triggerDistance)
        {
            triggered = true;
            spawner.StartSpawning();
        }
    }
}
