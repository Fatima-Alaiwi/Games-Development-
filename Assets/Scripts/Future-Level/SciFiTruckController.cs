using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SciFiTruckController : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;
    [SerializeField] private string _defaultInteractionText = "Drive Truck";

    [Header("Quest Assignment")]
    public Quest deliverCellQuest; 

    [Header("Cell Placement Setup")]
    public GameObject truckBackCubeVisual;
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
    public Camera truckCamera;      
    public Camera playerCamera;     
    public MonoBehaviour playerScript; 

    [Header("Reset / Respawn Settings")]
    [Tooltip("The exact string name of the layer that triggers a reset (e.g., 'Obstacle' or 'Water')")]
    public string resetLayerName = "Obstacle";
    [Tooltip("Drag a Transform here to act as a respawn point. If empty, uses starting coordinates.")]
    public Transform respawnPoint;
    
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private Rigidbody _rb;
    private float _currentSteerAngle;
    private bool _isDriving = false;

    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;

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

        if (truckBackCubeVisual != null)
        {
            truckBackCubeVisual.SetActive(false);
        }

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(resetLayerName))
        {
            ResetTruckPosition();
        }
    }

    private void ResetTruckPosition()
    {
        Debug.Log($"Truck Reset: Hit object on layer '{resetLayerName}'. Resetting position.");

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }
        else
        {
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
        }
    }

    public void Interact()
    {
        if (frontLeft.gameObject.activeSelf == false)  
        {
            Debug.Log("The truck is missing a wheel. I can't drive this!");
            return;
        }

        if (!_hasPlacedCube)
        {
            TryPlacePowerCube();
            return;
        }

        if (_isDriving) return;
        EnterVehicle();
    }

    private void TryPlacePowerCube()
    {
        if (InventoryManager.instance != null)
        {
            if (truckBackCubeVisual != null)
            {
                truckBackCubeVisual.SetActive(true);
            }

            _hasPlacedCube = true;

            if (QuestManager.Instance != null && deliverCellQuest != null)
            {
                QuestManager.Instance.AcceptQuest(deliverCellQuest);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideHoverText();
            }
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
        }

        if (playerCamera != null) playerCamera.gameObject.SetActive(false);
        if (truckCamera != null) truckCamera.gameObject.SetActive(true);
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
        if (wheel != null) wheel.Rotate(Vector3.right * amount);
    }

    // ==========================================
    // 🎬 NEW HELPER HOOK FOR THE CUTSCENE 
    // ==========================================
    /// <summary>
    /// Safely forces the player out of driving state, passes player references back to the 
    /// cutscene manager sequence, and re-enables the primary gameplay camera.
    /// </summary>
    public void ExitVehicleForCutscene(out MonoBehaviour outPlayerScript, out Camera outPlayerCamera)
    {
        _isDriving = false;
        
        // Disable vehicle perspective camera view
        if (truckCamera != null) truckCamera.gameObject.SetActive(false);
        
        // Safely map the references inside the out-parameters
        outPlayerScript = playerScript;
        outPlayerCamera = playerCamera;

        // Restore player camera tracking layer state
        if (playerCamera != null) 
        {
            playerCamera.gameObject.SetActive(true);
        }
    }
}