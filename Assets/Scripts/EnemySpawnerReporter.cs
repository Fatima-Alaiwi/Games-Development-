using UnityEngine;

/// Attached to spawned enemies at runtime. Reports death back to the EnemySpawner.
/// Do NOT put this on the prefab directly — EnemySpawner adds it automatically.

public class EnemySpawnerReporter : MonoBehaviour
{
    [HideInInspector]
    public EnemySpawner mySpawner;

    private bool hasReported = false;

    /// Call this from Actor.cs Death() when the enemy dies.
   
    public void ReportDeath()
    {
        if (hasReported) return;
        hasReported = true;

        if (mySpawner != null)
            mySpawner.OnEnemyKilled();
        else
            Debug.LogWarning("EnemySpawnerReporter: mySpawner is null, death not reported.");
    }

    // Removed OnDisable() — it was causing double-reports because
    // OnDisable fires on both Destroy AND simple deactivation.
    // Death is now reported ONLY via the explicit ReportDeath() call in Actor.cs.
}