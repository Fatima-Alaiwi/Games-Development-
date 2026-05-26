using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance;

    [Header("Portal Objects to Reveal")]
    public GameObject[] portalObjects;

    private bool portalsRevealed = false;

    void Awake()
    {
        Instance = this;
    }

    public void RevealPortals()
    {
        if (portalsRevealed) return;
        portalsRevealed = true;

        foreach (var obj in portalObjects)
            if (obj != null) obj.SetActive(true);

        Debug.Log("Portals revealed!");
    }
}