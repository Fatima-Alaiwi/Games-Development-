using UnityEngine;
 
public class BombProjectile : MonoBehaviour
{
    public float speed = 20f;
    public AudioClip throwSound;
 
    private Vector3 direction;
 
    void Start()
{
    Rigidbody rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.useGravity = false;
        rb.isKinematic = true; // ← change to true since you move it manually in Update
    }
}
    public void Launch(Vector3 dir)
    {
        direction = dir.normalized;
 
        if (throwSound != null)
            AudioSource.PlayClipAtPoint(throwSound, transform.position);
 
        Destroy(gameObject, 5f); // auto destroy after 5 seconds if it hits nothing
    }
 
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
 
    void OnTriggerEnter(Collider other)
    {
        // Hit the Dragon
        DragonHealth dragon = other.GetComponent<DragonHealth>();
        if (dragon != null)
        {
            dragon.TakeBombDamage();
            Destroy(gameObject);
            return;
        }
 
        // Hit anything else (walls, floor) - also destroy
        // Ignore the player itself
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}