using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public EnemySpawner spawner;
    public float triggerDistance = 8f;
    private bool triggered = false;

    void Update()
    {
        if (triggered) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log("Distance to player: " + distance);

        if (distance < triggerDistance)
        {
            triggered = true;
            spawner.StartSpawning();
            Debug.Log("Zone triggered by distance check!");
        }
    }
}