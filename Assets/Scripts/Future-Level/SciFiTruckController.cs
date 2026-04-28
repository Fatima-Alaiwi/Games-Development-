using UnityEngine;

public class SciFiTruckController : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;
    [SerializeField] private string _interactionText = "Drive Truck";

    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float turnSpeed = 40f;
    public float maxSteerAngle = 25f;

    [Header("6-Wheel Configuration")]
    public Transform frontLeft; public Transform frontRight;
    public Transform midLeft;   public Transform midRight;
    public Transform rearLeft;  public Transform rearRight;

    [Header("Camera & Player Assignment")]
    public Camera truckCamera;    
    public Camera playerCamera;     
    public MonoBehaviour playerScript; 

    private float _currentSteerAngle;
    private bool _isDriving = false;

    // Interface Requirements
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;
    public string InteractionText => _interactionText;

    void Start()
    {
        if (truckCamera != null) truckCamera.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (_isDriving) return;
        EnterVehicle();
    }

    private void EnterVehicle()
    {
        _isDriving = true;
        _isInteractable = false;

        if (playerScript != null)
        {
            var field = playerScript.GetType().GetField("canMove");
            if (field != null) field.SetValue(playerScript, false);
            else Debug.LogError("Could not find 'canMove' variable on the assigned Player Script!");
        }

        if (playerCamera != null) playerCamera.gameObject.SetActive(false);
        if (truckCamera != null) truckCamera.gameObject.SetActive(true);

        Debug.Log("Truck: Control Active. Player movement locked.");
    }

    void Update()
    {
        if (!_isDriving) return;

        float moveInput = Input.GetAxis("Vertical");
        float steerInput = Input.GetAxis("Horizontal");

        HandleMovement(moveInput);
        HandleSteering(steerInput, moveInput);
    }

    private void HandleMovement(float moveInput)
    {
        transform.Translate(Vector3.forward * moveInput * moveSpeed * Time.deltaTime);

        float spin = moveInput * moveSpeed * 100f * Time.deltaTime;
        RotateWheel(frontLeft, spin); RotateWheel(frontRight, spin);
        RotateWheel(midLeft, spin);   RotateWheel(midRight, spin);
        RotateWheel(rearLeft, spin);  RotateWheel(rearRight, spin);
    }

    private void HandleSteering(float steerInput, float moveInput)
    {
        _currentSteerAngle = steerInput * maxSteerAngle;
        
        if (frontLeft) frontLeft.localEulerAngles = new Vector3(frontLeft.localEulerAngles.x, _currentSteerAngle, 0);
        if (frontRight) frontRight.localEulerAngles = new Vector3(frontRight.localEulerAngles.x, _currentSteerAngle, 0);

        if (Mathf.Abs(moveInput) > 0.1f)
        {
            float direction = moveInput > 0 ? 1 : -1;
            transform.Rotate(Vector3.up * steerInput * turnSpeed * direction * Time.deltaTime);
        }
    }

    private void RotateWheel(Transform wheel, float amount)
    {
        if (wheel != null) wheel.Rotate(Vector3.right * amount);
    }
}