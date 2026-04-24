using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class QuestHUD : MonoBehaviour
{
    public TextMeshProUGUI progressText;
    private CanvasGroup canvasGroup;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // 1. Check if the Manager exists and has quests
        if (QuestManager.Instance != null && QuestManager.Instance.activeQuests.Count > 0)
        {
            Quest active = QuestManager.Instance.activeQuests[0];

            if (active != null)
            {
                // Show the UI
                ShowUI(true);

                // 2. Logic fix: Show Complete message if finished, Active if not
                if (active.isCompleted)
                {
                    progressText.text = active.completeMessage;
                }
                else
                {
                    // You can also format this to show progress like: "Apples: 3/5"
                    progressText.text = $"{active.activeMessage} {active.currentAmount}/{active.goalAmount}";
                }
            }
        }
        else
        {
            // Hide the UI but keep the script running
            ShowUI(false);
        }
    }

    public void ShowUI(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1 : 0;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }
}