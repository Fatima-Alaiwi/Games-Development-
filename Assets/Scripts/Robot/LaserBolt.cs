using UnityEngine;

public class LaserBolt : MonoBehaviour 
{
    public float speed = 30f; // Faster for that Star Wars feel
    public float lifetime = 3f;

    void Start()
    {
        // Self-destruct after a few seconds so your hierarchy doesn't get messy
        Destroy(gameObject, lifetime);
    }

    void Update() 
    {
        // Critical: Check your PauseMenuManager
        if (PauseMenuManager.isPaused) return; 

        // Move the bolt forward relative to its own orientation
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}