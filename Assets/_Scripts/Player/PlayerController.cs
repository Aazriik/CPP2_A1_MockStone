// Mockstone Player Controller
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using System;
using UnityEngine.UI;
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
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private bool enableSprint = true;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Weapon Settings")]
    [SerializeField] private Transform weaponAttachPoint;
    public Transform WeaponAttachPoint => weaponAttachPoint;
    public Collider Collider => col;

    private Vector2 moveInput = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    private float currentSpeed = 0.0f;
    private bool jumpPressed = false;
    private bool crouchPressed = false;
    private bool sprintPressed = false;

    private LayerMask stairsLayer;

    #region Input Handling
    void OnEnable()
    {
        InputManager.Instance.OnMoveEvent += OnMove;
        InputManager.Instance.OnJumpEvent += OnJump;
        InputManager.Instance.OnInteractEvent += OnInteract;
        InputManager.Instance.OnCrouchEvent += OnCrouch;
        InputManager.Instance.OnSprintEvent += OnSprint;
    }

    //void OnDisable()
    //{
    //    InputManager.Instance.OnMoveEvent -= OnMove;
    //    InputManager.Instance.OnJumpEvent -= OnJump;
    //}

    void OnMove(Vector2 input) => moveInput = input;
    void OnJump(bool pressed) => jumpPressed = pressed;
    void OnCrouch(bool pressed)
    {
        //crouchPressed = pressed;
        if (crouchPressed == false)
        {
            crouchPressed = true;
            anim.SetBool("isCrouching", true);
        }
        else
        {
            crouchPressed = false;
            anim.SetBool("isCrouching", false);
        }

        
    }
    void OnSprint(bool pressed) => sprintPressed = pressed;
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
        #region Crouch Animation Layer
        // Index 0 is the base layer, index 1 is the Crouching layer, and index 2 is the Aiming layer.
        // This is Index 1, Crouching Layer.
        float currentLayerWeight = anim.GetLayerWeight(1);
        // The targetLayerWeight is determined by whether the crouch button is pressed. If it is, we want the layer weight to be 1 (fully active), otherwise we want it to be 0 (inactive).
        float targetLayerWeight;
        // If the crouchPressed is TRUE, set the targetLayerWeight to 1.0f, which means the crouching layer will fully influence the animation.
        if (crouchPressed)
        {
            targetLayerWeight = 1.0f;
        }
        // Otherwise, we set targetLayerWeight to 0.0f, which means the crouching layer will not influence the animation at all.
        else
        {
            targetLayerWeight = 0.0f;
        }
        // Mathf.MoveTowards will smoothly transition the current layer weight towards the target layer weight. The speed of this transition is determined by the second parameter (5 in this case), which you can adjust to make the transition faster or slower.
        float newLayerWeight = Mathf.MoveTowards(
            currentLayerWeight,
            targetLayerWeight,
            Time.deltaTime * 5); // 0 -> 1 in 1/5th of a second.
        // Finally, we set the new layer weight for the crouching layer using anim.SetLayerWeight. This will ensure that the animation transitions smoothly between standing and crouching states based on the player's input.
        anim.SetLayerWeight(1, newLayerWeight);
        #endregion
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
        float targetSpeed = maxSpeed;

        if (crouchPressed)
        {
            targetSpeed = crouchSpeed;
        }
        else if (enableSprint && sprintPressed)
        {
            targetSpeed = maxSpeed * sprintMultiplier;
        }

        if (moveInput == Vector2.zero)
        {
             currentSpeed = 0;
        }
        else if (currentSpeed == 0.0f)
        {
            currentSpeed = initSpeed;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }


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
