using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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

    [Header("6-Wheel Transforms")]
    public Transform frontLeft; 
    public Transform frontRight;
    public Transform midLeft;   
    public Transform midRight;
    public Transform rearLeft;  
    public Transform rearRight;

    [Header("Camera & Player Assignment")]
    [Tooltip("The camera attached to the truck (Rear View)")]
    public Camera truckCamera;      
    [Tooltip("The player's main camera (First/Third Person)")]
    public Camera playerCamera;     
    [Tooltip("The Player GameObject that has the movement script")]
    public MonoBehaviour playerScript; 

    private Rigidbody _rb;
    private float _currentSteerAngle;
    private bool _isDriving = false;

    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;
    public string InteractionText => _interactionText;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        if (truckCamera != null) truckCamera.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (frontLeft.gameObject.activeSelf == false) 
        {
            Debug.Log("The truck is missing a wheel. I can't drive this!");
            return;
        }

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
            if (field != null) 
            {
                field.SetValue(playerScript, false);
            }
            else 
            {
                Debug.LogWarning("Truck: 'canMove' field not found on assigned player script. Make sure it is PUBLIC.");
            }
        }

        if (playerCamera != null) playerCamera.gameObject.SetActive(false);
        if (truckCamera != null) truckCamera.gameObject.SetActive(true);

        Debug.Log("Truck: Control Active. Physics Collisions Engaged.");
    }

    void FixedUpdate()
    {
        if (!_isDriving) return;

        float moveInput = Input.GetAxis("Vertical");  
        float steerInput = Input.GetAxis("Horizontal"); 

        ApplyPhysicsMovement(moveInput);
        ApplyPhysicsSteering(steerInput, moveInput);
    }

    void Update()
    {
        if (!_isDriving) return;

        float moveInput = Input.GetAxis("Vertical");
        float steerInput = Input.GetAxis("Horizontal");

        UpdateWheelVisuals(moveInput, steerInput);
    }

    private void ApplyPhysicsMovement(float moveInput)
    {
        Vector3 moveDistance = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + moveDistance);
    }

    private void ApplyPhysicsSteering(float steerInput, float moveInput)
    {
        if (Mathf.Abs(moveInput) > 0.05f)
        {
            float direction = moveInput > 0 ? 1 : -1;
            float turnAmount = steerInput * turnSpeed * direction * Time.fixedDeltaTime;
            
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            _rb.MoveRotation(_rb.rotation * turnRotation);
        }
    }

    private void UpdateWheelVisuals(float moveInput, float steerInput)
    {
        float spin = moveInput * moveSpeed * 100f * Time.deltaTime;

        RotateWheelMesh(frontLeft, spin); RotateWheelMesh(frontRight, spin);
        RotateWheelMesh(midLeft, spin);   RotateWheelMesh(midRight, spin);
        RotateWheelMesh(rearLeft, spin);  RotateWheelMesh(rearRight, spin);

        _currentSteerAngle = steerInput * maxSteerAngle;
        if (frontLeft) frontLeft.localEulerAngles = new Vector3(frontLeft.localEulerAngles.x, _currentSteerAngle, 0);
        if (frontRight) frontRight.localEulerAngles = new Vector3(frontRight.localEulerAngles.x, _currentSteerAngle, 0);
    }

    private void RotateWheelMesh(Transform wheel, float amount)
    {
        if (wheel != null) 
        {
            wheel.Rotate(Vector3.right * amount);
        }
    }
}