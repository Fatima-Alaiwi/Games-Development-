using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public TruckCutsceneManager cutsceneManager;
    public GameObject staticCutsceneTruckProp;
    public Quest questToComplete;

    private void OnTriggerEnter(Collider other)
    {
        SciFiTruckController playableTruck = other.GetComponentInParent<SciFiTruckController>();
        if (playableTruck == null) playableTruck = other.GetComponent<SciFiTruckController>();

        if (playableTruck != null && cutsceneManager != null)
        {
            if (questToComplete != null)
            {
                cutsceneManager.SetQuestToComplete(questToComplete);
            }
            else if (playableTruck.deliverCellQuest != null)
            {
                cutsceneManager.SetQuestToComplete(playableTruck.deliverCellQuest);
            }

            playableTruck.ExitVehicleForCutscene(out MonoBehaviour playerScript, out Camera playerCamera);
            cutsceneManager.SetupPlayerReferences(playerScript, playerCamera);

            playableTruck.gameObject.SetActive(false);

            if (staticCutsceneTruckProp != null)
            {
                staticCutsceneTruckProp.SetActive(true);
            }

            cutsceneManager.StartCutscene();
            gameObject.SetActive(false);
        }
    }
}