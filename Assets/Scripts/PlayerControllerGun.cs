using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerGun : MonoBehaviour
{
    public static PlayerControllerGun instance;
    PlayerInput playerInput;
    PlayerInput.MainActions input;
    CharacterController controller;
    public bool canMove = true;
    [SerializeField] private Animator animator;
    AudioSource audioSource;

    WeaponController currentWeapon;

    [Header("Weapon")]
    public Transform weaponHolderPosition;
    public WeaponData defaultWeapon;

    [Header("Controller")]
    public float moveSpeed = 5;
    public float gravity = -9.8f;
    public float jumpHeight = 1.2f;

    Vector3 _PlayerVelocity;
    bool isGrounded;

    [Header("Camera")]
    public Camera cam;
    public float sensitivity;
    float xRotation;

    [Header("Footsteps")]
    public AudioClip footstepClip;
    public float footstepInterval = 0.5f;
    public float minimumMoveAmount = 0.1f;

    private float footstepTimer;
    private Vector2 currentMoveInput;

    void Awake()
    {
        instance = this;
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        playerInput = new PlayerInput();
        input = playerInput.Main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetWeapon(defaultWeapon);
        AssignInputs();
    }

    public void SetWeapon(WeaponData newWeapon)
    {
        if(newWeapon == null) { Debug.LogError("defaultWeapon is not assigned!"); return; }
        if(newWeapon.weaponPrefab == null) { Debug.LogError("weaponPrefab is missing!"); return; }
        if(currentWeapon != null) Destroy(currentWeapon.gameObject);
        currentWeapon = Instantiate(newWeapon.weaponPrefab, weaponHolderPosition);
        if (currentWeapon != null) currentWeapon.myController = this;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (currentWeapon != null && currentWeapon.weaponData != null)
        {
            if(input.Attack.IsPressed() && currentWeapon.weaponData.automatic)
                currentWeapon.Shoot();
        }

        HandleFootsteps();
    }

    void FixedUpdate()
    {
        // If movement is locked, stop here
        if (!canMove) 
        {
        currentMoveInput = Vector2.zero;
        return; 
        }

        currentMoveInput = input.Movement.ReadValue<Vector2>();
        MoveInput(currentMoveInput);
    }

    void LateUpdate()
    {
        // If movement is locked, stop the camera from turning
        if (!canMove) return;

        LookInput(input.Look.ReadValue<Vector2>());
    }

    void MoveInput(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);
        _PlayerVelocity.y += gravity * Time.deltaTime;
        if(isGrounded && _PlayerVelocity.y < 0) _PlayerVelocity.y = -2f;
        controller.Move(_PlayerVelocity * Time.deltaTime);
    }

    void LookInput(Vector3 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;
        xRotation -= (mouseY * Time.deltaTime * sensitivity);
        xRotation = Mathf.Clamp(xRotation, -80, 80);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime * sensitivity));
    }

    void HandleFootsteps()
    {
        if (footstepClip == null || audioSource == null) return;
        bool isMoving = currentMoveInput.magnitude > minimumMoveAmount;
        if (isGrounded && isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                audioSource.PlayOneShot(footstepClip, 0.2f);
                footstepTimer = footstepInterval;
            }
        }
        else { footstepTimer = 0f; }
    }

    void OnEnable() { input.Enable(); }
    void OnDisable() { input.Disable(); }

    void Jump()
    {
        if (isGrounded)
            _PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    void AssignInputs()
    {
        input.Jump.performed += ctx => Jump();

        input.Attack.started += ctx =>
        {
            if (currentWeapon != null) currentWeapon.Shoot();
        };

        input.Reload.started += ctx =>
        {
            if (currentWeapon != null) currentWeapon.Reload();
        };
    }

    public void PlayAnimation(string newState)
    {
        Debug.Log("Playing animation: " + newState);
        animator.Play(newState);
    }
}