using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Manager Binding")]
    public TruckCutsceneManager cutsceneManager;

    [Header("Exact Truck Snap Configuration Targets")]
    [Tooltip("The designated coordinates from your inspector snapshot.")]
    public Vector3 exactTruckPosition = new Vector3(3.51f, 2.14f, -18.73f);
    [Tooltip("The rotation yaw required to face perfectly down the delivery lane.")]
    public Vector3 exactTruckRotationEuler = new Vector3(0f, 90f, 0f);

    private void OnTriggerEnter(Collider other)
    {
        // Search parent and local transforms comprehensively for the controller component
        SciFiTruckController truck = other.GetComponentInParent<SciFiTruckController>();
        if (truck == null) truck = other.GetComponent<SciFiTruckController>();

        // Ensure we only process if it is actually our valid truck hitting the zone
        if (truck != null && cutsceneManager != null)
        {
            Rigidbody truckRb = truck.GetComponent<Rigidbody>();
            if (truckRb != null)
            {
                // Kill all progressive velocity drifts immediately so it parks cleanly
                truckRb.linearVelocity = Vector3.zero;
                truckRb.angularVelocity = Vector3.zero;
                
                // Force kinematic state to prevent physics jitter during the animation sequence
                truckRb.isKinematic = true; 
            }

            // Snap the truck completely to your precise coordinates
            truck.transform.position = exactTruckPosition;
            truck.transform.rotation = Quaternion.Euler(exactTruckRotationEuler);

            // Execute the sequence process
            cutsceneManager.StartCutscene();
            
            // Turn off this trigger GameObject instance so it cannot accidentally re-fire
            gameObject.SetActive(false);
        }
    }
}