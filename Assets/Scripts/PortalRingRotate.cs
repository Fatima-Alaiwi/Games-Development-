using UnityEngine;
// Raghad: attach this to SM_Bld_Portal_01_Ring_01
// Rotates the ring only on Z axis — no animation needed
public class PortalRingRotate : MonoBehaviour
{
    public float rotationSpeed = 30f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}