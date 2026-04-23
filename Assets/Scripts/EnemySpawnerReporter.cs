using UnityEngine;

public class EnemySpawnerReporter : MonoBehaviour
{
    public EnemySpawner mySpawner;
    private bool hasReported = false;

    public void ReportDeath()
    {
        if (hasReported) return;
        hasReported = true;

        if (mySpawner != null)
            mySpawner.OnEnemyKilled();
    }

    void OnDisable()
    {
        ReportDeath();
    }
}