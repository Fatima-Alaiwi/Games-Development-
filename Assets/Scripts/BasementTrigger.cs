using UnityEngine;

public class BasementTrigger : MonoBehaviour
{
    [Header("Phone Reference")]
    public PhoneInteractable phone; // Drag your RingingPhoneInBasement here

    private bool hasTriggered = false; // Only triggers ONCE

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            phone.StartRinging();
            Debug.Log("Peter entered the basement! Phone is ringing!");
        }
    }
}