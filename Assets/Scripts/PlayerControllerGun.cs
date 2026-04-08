using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerGun : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerInput.MainActions input;

    CharacterController controller;
    Animator animator;
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

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
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
        if(newWeapon == null)
        {
            Debug.LogError("PlayerControllerGun: defaultWeapon is not assigned!");
            return;
        }

        if(newWeapon.weaponPrefab == null)
        {
            Debug.LogError("PlayerControllerGun: weaponPrefab in WeaponData is missing!");
            return;
        }

        if(currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        currentWeapon = Instantiate(newWeapon.weaponPrefab, weaponHolderPosition);

        if (currentWeapon != null)
        {
            currentWeapon.myController = this;
        }
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (currentWeapon != null && currentWeapon.weaponData != null)
        {
            // ✅ FIXED: Shoot → Attack
            if(input.Attack.IsPressed() && currentWeapon.weaponData.automatic)
            {
                currentWeapon.Shoot();
            }
        }
    }

    void FixedUpdate()
    {
        MoveInput(input.Movement.ReadValue<Vector2>());
    }

    void LateUpdate()
    {
        LookInput(input.Look.ReadValue<Vector2>());
    }

    void MoveInput(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        controller.Move(transform.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime);

        _PlayerVelocity.y += gravity * Time.deltaTime;

        if(isGrounded && _PlayerVelocity.y < 0)
            _PlayerVelocity.y = -2f;

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

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Jump()
    {
        if (isGrounded)
            _PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    void AssignInputs()
    {
        input.Jump.performed += ctx => Jump();

        // ✅ FIXED: Shoot → Attack
        input.Attack.started += ctx =>
        {
            if (currentWeapon != null)
                currentWeapon.Shoot();
        };

        // ❌ REMOVED Reload (does not exist in Input System)
    }

    public void PlayAnimation(string newState)
    {
        animator.Play(newState);
    }
}