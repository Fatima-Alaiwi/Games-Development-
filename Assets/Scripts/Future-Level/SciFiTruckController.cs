using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SciFiTruckController : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;
    [SerializeField] private string _defaultInteractionText = "Drive Truck";

    [Header("Quest Assignment")]
    public Quest deliverCellQuest; // Drag your "Deliver Cell" quest asset here

    [Header("Cell Placement Setup")]
    [Tooltip("The visual cube model on the back of the truck bed (Set hidden by default)")]
    public GameObject truckBackCubeVisual;
    [Tooltip("The exact string identifier used in your player inventory for the power cell")]
    public string powerCellItemName = "PowerCell"; 
    private bool _hasPlacedCube = false;

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

    // Interface Implementation
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;

    // Dynamically update context text based on state
    public string InteractionText
    {
        get
        {
            if (!_hasPlacedCube)
            {
                return "Not Now";
            }
            return _defaultInteractionText;
        }
        set => _defaultInteractionText = value;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        if (truckCamera != null) truckCamera.gameObject.SetActive(false);

        // Ensure the back visual starts hidden
        if (truckBackCubeVisual != null)
        {
            truckBackCubeVisual.SetActive(false);
        }
    }

    public void Interact()
    {
        if (frontLeft.gameObject.activeSelf == false) 
        {
            Debug.Log("The truck is missing a wheel. I can't drive this!");
            return;
        }

        // STEP 1: Handle Cube placement first if it hasn't been done yet
        if (!_hasPlacedCube)
        {
            TryPlacePowerCube();
            return;
        }

        // STEP 2: Handle regular driving if cube is placed
        if (_isDriving) return;
        EnterVehicle();
    }

    private void TryPlacePowerCube()
    {
        // Replace 'Inventory.Instance' with your exact inventory manager script instance structure
        if (InventoryManager.instance != null && InventoryManager.instance.HasItem(powerCellItemName))
        {
            // Remove item from custom player inventory bag
            InventoryManager.instance.RemoveItem(powerCellItemName);

            // Show the second cube visual sitting inside the back truck bed
            if (truckBackCubeVisual != null)
            {
                truckBackCubeVisual.SetActive(true);
            }

            _hasPlacedCube = true;
            Debug.Log("<color=green>TRUCK LOADED:</color> Power cell secured to truck bed.");

            // Give the user their new quest step directly out in the world
            if (QuestManager.Instance != null && deliverCellQuest != null)
            {
                QuestManager.Instance.AcceptQuest(deliverCellQuest);
                Debug.Log("Quest Started: " + deliverCellQuest.questName);
            }

            // Force visual UI text refreshing
            UIManager.Instance.HideHoverText();
        }
        else
        {
            Debug.Log("<color=yellow>INTERACTION LOCKED:</color> I need to find the missing power cell first.");
        }
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