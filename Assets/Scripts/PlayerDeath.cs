using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hazard"))
        {
            Actor actor = GetComponentInParent<Actor>();
            if (actor != null)
                actor.Kill();
        }
    }
}
