using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Door Parts")]
    public Transform topDoorPart;
    public Transform bottomDoorPart;
    
    [Header("Settings")]
    public float slideDistance = 1.5f; // How far each part moves
    public float openSpeed = 2.0f;

    private Vector3 topClosedPos;
    private Vector3 bottomClosedPos;
    private Vector3 topOpenPos;
    private Vector3 bottomOpenPos;
    
    private bool isOpen = false;

    void Start()
    {
        // Store starting local positions
        topClosedPos = topDoorPart.localPosition;
        bottomClosedPos = bottomDoorPart.localPosition;

        // Calculate targets: Top moves Up, Bottom moves Down
        topOpenPos = topClosedPos + (Vector3.up * slideDistance);
        bottomOpenPos = bottomClosedPos + (Vector3.down * slideDistance);
    }

    void Update()
    {
        if (isOpen)
        {
            // Smoothly lerp to the vertical open positions
            topDoorPart.localPosition = Vector3.Lerp(topDoorPart.localPosition, topOpenPos, Time.deltaTime * openSpeed);
            bottomDoorPart.localPosition = Vector3.Lerp(bottomDoorPart.localPosition, bottomOpenPos, Time.deltaTime * openSpeed);
        }
    }

    public void Open()
    {
        isOpen = true;
    }
}