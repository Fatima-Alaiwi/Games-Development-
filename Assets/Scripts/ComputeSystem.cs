using UnityEngine;

public class ComputerSystem : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject mapCanvas;
    [field: SerializeField] public string InteractionText { get; set; } = "Press E to open the Map";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    private bool isUiOpen = false;

    public void Interact()
    {
        isUiOpen = !isUiOpen;

        if (isUiOpen)
        {
            EnterUiMode();
        }
        else
        {
            ExitUiMode();
        }
    }

    void EnterUiMode()
{
    if (mapCanvas != null) mapCanvas.SetActive(true);
    
    // Change 'PlayerController' to 'PlayerControllerGun'
    if (PlayerControllerGun.instance != null)
    {
        PlayerControllerGun.instance.canMove = false;
    }
    
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}

    void ExitUiMode()
{
    if (mapCanvas != null) mapCanvas.SetActive(false);

    // Change 'PlayerController' to 'PlayerControllerGun'
    if (PlayerControllerGun.instance != null)
    {
        PlayerControllerGun.instance.canMove = true;
    }

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}
}