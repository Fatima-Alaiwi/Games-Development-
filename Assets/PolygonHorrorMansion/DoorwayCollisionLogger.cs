using UnityEngine;

public class DoorwayCollisionLogger : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[DoorwayCollisionLogger] OnCollisionEnter: {collision.collider.name} | Tag: {collision.collider.tag} | Layer: {LayerMask.LayerToName(collision.collider.gameObject.layer)}");
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log($"[DoorwayCollisionLogger] OnCollisionStay: {collision.collider.name} | Tag: {collision.collider.tag}");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log($"[DoorwayCollisionLogger] OnCollisionExit: {collision.collider.name} | Tag: {collision.collider.tag}");
    }

    // If the doorway uses a Trigger collider instead of a physics collider
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[DoorwayCollisionLogger] OnTriggerEnter: {other.name} | Tag: {other.tag} | Layer: {LayerMask.LayerToName(other.gameObject.layer)}");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"[DoorwayCollisionLogger] OnTriggerStay: {other.name} | Tag: {other.tag}");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[DoorwayCollisionLogger] OnTriggerExit: {other.name} | Tag: {other.tag}");
    }
}