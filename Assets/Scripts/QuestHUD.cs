using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))] // Automatically adds CanvasGroup if missing
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
        if (QuestManager.Instance == null) { ShowUI(false); return; } // Raghad: null check added

        // 2. First check active quests
        if (QuestManager.Instance.activeQuests.Count > 0)
        {
            Quest active = QuestManager.Instance.activeQuests[0];

            if (active != null)
            {
                // Show the UI
                ShowUI(true);

                // You can also format this to show progress like: "Apples: 3/5"
                progressText.text = $"{active.activeMessage} {active.currentAmount}/{active.goalAmount}";
                return; // Raghad: return so we don't fall through to completed check
            }
        }

        // Raghad: added completed quest check so complete message shows after quest finishes
        if (QuestManager.Instance.completedQuests.Count > 0)
        {
            Quest lastCompleted = QuestManager.Instance.completedQuests
                [QuestManager.Instance.completedQuests.Count - 1];

            if (lastCompleted != null && lastCompleted.completeMessage != "")
            {
                // Show the complete message
                ShowUI(true);
                progressText.text = lastCompleted.completeMessage; // Raghad: shows complete message
                return;
            }
        }

        // Hide the UI but keep the script running
        ShowUI(false);

      
    }

    private void ShowUI(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1 : 0;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }
}