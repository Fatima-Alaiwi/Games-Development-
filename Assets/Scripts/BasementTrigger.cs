using UnityEngine;

public class BasementTrigger : MonoBehaviour
{
    [Header("Phone Reference")]
    public PhoneInteractable phone; // Drag your RingingPhoneInBasement here

    [Header("Quest Settings")]
    public Quest answerPhoneQuest; // Raghad: drag AnswerPhoneQuest here

    private bool hasTriggered = false; // Only triggers ONCE

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            phone.StartRinging();

            // Raghad: start phone quest so HUD shows "Answer the phone!"
            if (answerPhoneQuest != null)
                QuestManager.Instance.AcceptQuest(answerPhoneQuest);

            Debug.Log("Peter entered the basement! Phone is ringing!");
        }
    }
}