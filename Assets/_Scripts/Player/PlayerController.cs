// Mockstone Player Controller
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using System;
//using UnityEditor.Experimental.GraphView;

public class PlayerController : MonoBehaviour
{
    CharacterController cc;
    Collider col;
    Animator anim;
    Camera mainCamera;
    WeaponBase curWeapon = null;
    IInteract interactableObject = null;

    public GameObject interactImage;

    [Header("Jump Settiings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float timeToJumpApex = 0.4f;

    private float gravity;
    private float initalJumpVelocity;

    [Header("Movement Settings")]
    [SerializeField] private float initSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 7.0f;
    [SerializeField] private float acceleration = 3.0f;

    [Header("Weapon Settings")]
    [SerializeField] private Transform weaponAttachPoint;
    public Transform WeaponAttachPoint => weaponAttachPoint;
    public Collider Collider => col;

    private Vector2 moveInput = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    private float currentSpeed = 0.0f;
    private bool jumpPressed = false;

    private LayerMask stairsLayer;

    #region Input Handling
    void OnEnable()
    {
        InputManager.Instance.OnMoveEvent += OnMove;
        InputManager.Instance.OnJumpEvent += OnJump;
        InputManager.Instance.OnInteractEvent += OnInteract;
    }

    //void OnDisable()
    //{
    //    InputManager.Instance.OnMoveEvent -= OnMove;
    //    InputManager.Instance.OnJumpEvent -= OnJump;
    //}

    void OnMove(Vector2 input) => moveInput = input;
    void OnJump(bool pressed) => jumpPressed = pressed;
    void OnInteract(bool pressed)
    {
        if (interactableObject != null && pressed)
        {
            WeaponBase weapon = interactableObject as WeaponBase;
            if (curWeapon != null && weapon != null)
                return;

            if (weapon != null && curWeapon == null)
                curWeapon = weapon;


            interactableObject.Interact(this);
            return;
        }

        if (pressed && curWeapon != null)
        {
            curWeapon.Drop(col);
            curWeapon = null;
        }
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        col = GetComponent<Collider>();
        anim = GetComponentInChildren<Animator>();

        CalculateJumpVariables();

        stairsLayer = LayerMask.GetMask("Stairs");
        mainCamera = Camera.main;
    }

    //this triggers when a value is changed in the inspector
    void OnValidate()
    {
        CalculateJumpVariables();
    }

    void CalculateJumpVariables()
    {
        try
        {
            if (timeToJumpApex <= 0)
                throw new System.ArgumentOutOfRangeException("timeToJumpApex must be greater than zero.");

            if (jumpHeight <= 0)
                throw new System.ArgumentOutOfRangeException("jumpHeight must be greater than zero.");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            timeToJumpApex = 0.4f;
            jumpHeight = 2f;
        }

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        initalJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    private void Update()
    {
        CheckInteractionUI();

        Ray newRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        //Debug.DrawRay(newRay.origin, newRay.direction * 10.0f, Color.red, 0.1f);
        bool hitSomething = Physics.Raycast(newRay, out hitInfo, 10.0f, stairsLayer);
        if (hitSomething)
        {
            Debug.Log("Stairs detected: " + hitInfo.collider.gameObject.name);
        }
    }

    private void CheckInteractionUI()
    {
        if (interactableObject != null && interactImage.activeSelf == false)
            interactImage.SetActive(true);
        else if (interactableObject == null && interactImage.activeSelf == true)
            interactImage.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 projectedMoveDirection = ProjectedMoveDirection();
        UpdateCharacterVelocity(projectedMoveDirection);
        UpdateCharacterRotation(projectedMoveDirection);

        cc.Move(velocity * Time.fixedDeltaTime);
        anim.SetFloat("speed", currentSpeed / maxSpeed);
    }

    #region Movement Helpers
    private Vector3 ProjectedMoveDirection()
    {
        Vector3 cameraFwd = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraFwd.y = 0;
        cameraRight.y = 0;

        cameraFwd.Normalize();
        cameraRight.Normalize();

        return cameraFwd * moveInput.y + cameraRight * moveInput.x;
    }

    void UpdateCharacterVelocity(Vector3 projectedMoveDirection)
    {
        if (moveInput == Vector2.zero)
            currentSpeed = 0f;
        else if (currentSpeed == 0.0f)
            currentSpeed = initSpeed;
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);


        velocity.x = projectedMoveDirection.x * currentSpeed;
        velocity.z = projectedMoveDirection.z * currentSpeed;

        if (cc.isGrounded)
        {
            velocity.y = -cc.skinWidth;
            if (jumpPressed)
            {
                velocity.y = initalJumpVelocity;
            }
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
    }
    private void UpdateCharacterRotation(Vector3 projectedMoveDirection)
    {
        if (moveInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(projectedMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }
    }
    #endregion

    private void OnTriggerEnter(Collider collision)
    {
        IInteract interactable = collision.GetComponent<IInteract>();
        if (interactable != null)
        {
            interactableObject = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteract interactable = other.GetComponent<IInteract>();
        if (interactable != null && interactableObject.Equals(interactable))
        {
            interactableObject = null;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {

    }
}
