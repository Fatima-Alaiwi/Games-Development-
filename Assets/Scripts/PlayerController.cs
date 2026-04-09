using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerInput.MainActions input;

    CharacterController controller;
   [SerializeField] private Animator animator;
    AudioSource audioSource;
public WeaponController gun;
    [Header("Controller")]
    public float moveSpeed = 5;
    public float gravity = -9.8f;
    public float jumpHeight = 1.2f;

    Vector3 _PlayerVelocity;

    bool isGrounded;

    [Header("Camera")]
    public Camera cam;
    public float sensitivity;

    float xRotation = 0f;

    void Awake()
    { 
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        playerInput = new PlayerInput();
        input = playerInput.Main;
        AssignInputs();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        // Repeat Inputs
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     if (gun != null)
        //     {
        //         gun.Shoot();
        //     }
        // }

        SetAnimations();
    }

    void FixedUpdate() 
    { MoveInput(input.Movement.ReadValue<Vector2>()); }

    void LateUpdate() 
    { LookInput(input.Look.ReadValue<Vector2>()); }

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
    { input.Enable(); }

    void OnDisable()
    { input.Disable(); }

    void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (isGrounded)
            _PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    void AssignInputs()
    {
        input.Jump.performed += ctx => Jump();
        input.Attack.started += ctx => Attack();
    }

    // ---------- //
    // ANIMATIONS //
    // ---------- //

    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string ATTACK1 = "Attack 1";
    public const string ATTACK2 = "Attack 2";

    string currentAnimationState;

    public void ChangeAnimationState(string newState) 
    {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        Debug.Log("Animator object: " + animator.gameObject.name + " | State: " + newState);
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    void SetAnimations()
    {
        // If player is not attacking
        if(!attacking)
        {
            if(_PlayerVelocity.x == 0 &&_PlayerVelocity.z == 0)
            { ChangeAnimationState(IDLE); }
            else
            { ChangeAnimationState(WALK); }
        }
    }

    // ------------------- //
    // ATTACKING BEHAVIOUR //
    // ------------------- //

    [Header("Attacking")]
    public float attackDistance = 3f;
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;

    public void Attack()
    {
        if(!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(swordSwing);

        if(attackCount == 0)
        {
            Debug.Log("Playing Attack 1");
            ChangeAnimationState(ATTACK1);
            attackCount++;
        }
        else
        {
            Debug.Log("Playing Attack 2");
            ChangeAnimationState(ATTACK2);
            attackCount = 0;
        }
    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

  void AttackRaycast()
{
    if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
    { 
        HitTarget(hit);

        if(hit.transform.TryGetComponent<Actor>(out Actor T))
        { 
            T.TakeDamage(attackDamage); 
        }
    } 
}

    void HitTarget(RaycastHit hit)
{
    audioSource.pitch = 1;
    audioSource.PlayOneShot(hitSound);

    GameObject GO = Instantiate(
        hitEffect,
        hit.point,
        Quaternion.LookRotation(hit.normal)
    );

    GO.transform.SetParent(hit.transform);

    Destroy(GO, 20);
}
    public void PlayAnimation(string anim)
{
    if(animator != null)
    {
        animator.Play(anim);
    }
}
}