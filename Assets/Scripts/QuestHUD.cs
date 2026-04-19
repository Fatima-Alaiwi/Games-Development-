using UnityEngine;
using TMPro;

public class QuestHUD : MonoBehaviour
{
    public TextMeshProUGUI progressText;
    private GameObject panelBackground;

    void Awake()
    {
        // Assumes this script is on the Panel itself
        panelBackground = this.gameObject;
    }

    void Update()
    {
        if (QuestManager.Instance != null && QuestManager.Instance.activeQuests.Count > 0)
        {
            Quest active = QuestManager.Instance.activeQuests[0];

            if (active != null)
            {
                panelBackground.SetActive(true);

                if (active.isCompleted)
                {
                    progressText.text = $"<b>{active.questName}</b>: Done!";
                    progressText.color = Color.green;
                }
                else
                {
                    progressText.text = $"Goal: {active.currentAmount} / {active.goalAmount} {active.goalItemName}s";
                    progressText.color = Color.white;
                }
            }
        }
        else
        {
            // Hides the whole box if no quest is active
            panelBackground.SetActive(false);
        }
    }
}