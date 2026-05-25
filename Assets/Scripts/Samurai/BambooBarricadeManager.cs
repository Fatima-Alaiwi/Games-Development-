using UnityEngine;

public class BambooBarricadeManager : MonoBehaviour
{
    [Header("Bamboos to cut")]
    public BarricadeBamboo[] bamboos;

    [Header("Barricade Collider")]
    public Collider barricadeCollider;

    [Header("Portal")]
    public SamuraiPortalController portal;

    [Header("Quest")]
    public Quest bambooQuest;

    [Header("Voice Lines")]
    public AudioClip voiceOnFirstCut;  // "These bamboos are in the way..."
    public AudioClip voiceOnLastCut;   // "Come on... come on—"
    public AudioClip voiceOnCleared1;  // "I just... I need to get out of here."

    private int cutCount = 0;

    void Start()
    {
        foreach (var b in bamboos)
            if (b != null) b.SetManager(this);
    }

    public void OnBambooCut()
    {
        cutCount++;
        Debug.Log($"Bamboos cut: {cutCount}/{bamboos.Length}");

        if (cutCount == 1)
            PlayVoice(voiceOnFirstCut);

        if (cutCount == bamboos.Length)
            PlayVoice(voiceOnLastCut);

        if (QuestManager.Instance != null && bambooQuest != null)
        {
            if (!QuestManager.Instance.activeQuests.Contains(bambooQuest) &&
                !QuestManager.Instance.completedQuests.Contains(bambooQuest))
                QuestManager.Instance.AcceptQuest(bambooQuest);

            QuestManager.Instance.UpdateProgress("Bamboo", 1);
        }

        if (cutCount >= bamboos.Length)
            ClearBarricade();
    }

    void ClearBarricade()
    {
        if (barricadeCollider != null)
            barricadeCollider.enabled = false;

        if (QuestManager.Instance != null && bambooQuest != null)
            QuestManager.Instance.CompleteQuestPublic(bambooQuest);

        if (portal != null)
            portal.ActivatePortal();

        StartCoroutine(PlayClearedVoices());

        Debug.Log("Barricade cleared! Portal activated.");
    }

    System.Collections.IEnumerator PlayClearedVoices()
    {
        yield return new WaitForSeconds(
            voiceOnLastCut != null ? voiceOnLastCut.length + 0.5f : 1.5f);

        PlayVoice(voiceOnCleared1);
    }

    void PlayVoice(AudioClip clip)
    {
        if (clip == null) return;

        GameObject voiceObj = GameObject.Find("VoiceAudioSource");
        if (voiceObj == null) return;

        AudioSource source = voiceObj.GetComponent<AudioSource>();
        if (source != null)
            source.PlayOneShot(clip);
    }
}