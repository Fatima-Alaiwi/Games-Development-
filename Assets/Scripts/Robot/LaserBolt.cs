using UnityEngine;

public class LaserBolt : MonoBehaviour 
{
    public float speed = 30f;
    public float lifetime = 3f;
    public int damageAmount = 1;
    public float hitRadius = 0.25f;
    public bool destroyOnHit = true;

    private Vector3 _lastPosition;
    private bool _hasHitPlayer = false;

    void Start()
    {
        _lastPosition = transform.position;

        Destroy(gameObject, lifetime);
    }

    void Update() 
    {
        if (PauseMenuManager.isPaused) return; 

        Vector3 nextPosition = transform.position + transform.forward * speed * Time.deltaTime;
        CheckForPlayerHitBetween(_lastPosition, nextPosition);

        transform.position = nextPosition;
        _lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamagePlayer(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void TryDamagePlayer(Collider other)
    {
        if (_hasHitPlayer) return;

        Actor playerActor = other.GetComponent<Actor>();
        if (playerActor == null)
        {
            playerActor = other.GetComponentInParent<Actor>();
        }
        if (playerActor == null)
        {
            playerActor = other.GetComponentInChildren<Actor>();
        }

        bool hitPlayer = playerActor != null && playerActor.isPlayer;
        if (!hitPlayer && other.CompareTag("Player"))
        {
            hitPlayer = playerActor != null;
        }

        if (!hitPlayer) return;

        _hasHitPlayer = true;
        playerActor.TakeDamage(damageAmount);

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    private void CheckForPlayerHitBetween(Vector3 startPosition, Vector3 endPosition)
    {
        if (_hasHitPlayer) return;

        Vector3 direction = endPosition - startPosition;
        float distance = direction.magnitude;
        if (distance <= 0f) return;

        RaycastHit[] hits = Physics.SphereCastAll(
            startPosition,
            hitRadius,
            direction.normalized,
            distance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide
        );

        for (int i = 0; i < hits.Length; i++)
        {
            TryDamagePlayer(hits[i].collider);
            if (_hasHitPlayer) return;
        }
    }
}
