using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    CharacterController cc;
    Collider col;
    Animator anim;
    Camera mainCamera;
    WeaponBase curWeapon = null;
    IInteract interactableObject = null;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float timeToJumpApex = 0.4f;

    private float gravity;
    private float initialJumpVelocity;

    [Header("Movement Settings")]
    [SerializeField] private float initSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 7.0f;
    [SerializeField] private float acceleration = 3.0f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private bool enableSprint = true;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private float staminaRegenDelay = 1.0f;
    [SerializeField] private float minStaminaToSprint = 10f;

    private float currentStamina;
    private float staminaRegenTimer = 0f;
    private bool isExhausted = false;

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
    // Animator - Target Layer Weight
    private float targetLayerWeight;

    #region Input Handling
    void OnEnable()
    {
        InputManager.Instance.OnMoveEvent += OnMove;
        InputManager.Instance.OnJumpEvent += OnJump;
        InputManager.Instance.OnInteractEvent += OnInteract;
        InputManager.Instance.OnCrouchEvent += OnCrouch;
        InputManager.Instance.OnSprintEvent += OnSprint;
    }

    void OnDisable()
    {
        // Unsubscribe to prevent memory leaks if the player is destroyed
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveEvent -= OnMove;
            InputManager.Instance.OnJumpEvent -= OnJump;
            InputManager.Instance.OnInteractEvent -= OnInteract;
            InputManager.Instance.OnCrouchEvent -= OnCrouch;
            InputManager.Instance.OnSprintEvent -= OnSprint;
        }
    }

    void OnMove(Vector2 input) => moveInput = input;
    void OnJump(bool pressed) => jumpPressed = pressed;
    void OnCrouch(bool pressed)
    {
        if (!crouchPressed)
        {
            crouchPressed = true;
            anim.SetBool("isCrouching", true);
            // Instantly drop to crouch speed if we weren't already crouching
            currentSpeed = crouchSpeed;
            // Set targetLayerWeight to 1 to transition to crouch animation layer
            targetLayerWeight = 1;

            // Character Controller Settings | Crouch
            cc.height = 1.32f;
            cc.center = new Vector3(0.06f, -0.22f, 0.11f);
            cc.radius = 0.34f;

        }
        else
        {
            crouchPressed = false;
            anim.SetBool("isCrouching", false);
            // Recover speed to initSpeed to allow acceleration back to max when we stop crouching
            currentSpeed = initSpeed;
            // Set targetLayerWeight to 0 to transition back to base animation layer
            targetLayerWeight = 0;

            // Character Controller Settings | Base
            cc.height = 1.83f;
            cc.center = new Vector3(0, 0, 0);
            cc.radius = 0.22f;
        }
    }
    void OnSprint(bool pressed) => sprintPressed = pressed;
    void OnInteract(bool pressed)
    {
        Debug.Log($"Interact Key Pressed: {pressed}. Interactable found: {interactableObject != null}");

        if (interactableObject != null && pressed)
        {
            Debug.Log("Executing Interaction...");

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

    void Start()
    {

        cc = GetComponent<CharacterController>();
        col = GetComponent<Collider>();
        anim = GetComponentInChildren<Animator>();

        if (SaveManager.Instance != null && SaveManager.Instance.CurrentData.hasSavedPosition)
        {
            // Disable the controller so it doesn't fight the teleportation
            cc.enabled = false;

            transform.position = new Vector3(
                SaveManager.Instance.CurrentData.playerPosX,
                SaveManager.Instance.CurrentData.playerPosY,
                SaveManager.Instance.CurrentData.playerPosZ
            );

            // Re-enable the controller now that we are in the correct spot
            cc.enabled = true;
        }

        CalculateJumpVariables();

        stairsLayer = LayerMask.GetMask("Stairs");
        mainCamera = Camera.main;

        // Initialize Stamina from Save Data, or default to max for a new game
        if (SaveManager.Instance != null && SaveManager.Instance.CurrentData.currentStamina != -1f)
        {
            currentStamina = SaveManager.Instance.CurrentData.currentStamina;
        }
        else
        {
            currentStamina = maxStamina;
        }

        // Lock and hide the cursor for standard gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

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
        initialJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    private void Update()
    {
        CheckInteractionUI();

        Ray newRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(newRay, out hitInfo, 10.0f, stairsLayer))
        {
            Debug.Log("Stairs detected: " + hitInfo.collider.gameObject.name);
        }
        
        CrouchTransition();
    }

    private void CrouchTransition()
    {
        // Crouching layer weight transition handled here for smooth animation blending
        // Set currentLayerWeight to Anim Layer index 1 (Crouch Layer). Set targetLayerWeight.
        float currentLayerWeight = anim.GetLayerWeight(1);
        // Use Mathf.MoveTowards in Update to transition the layer weight over time for a smooth animation blend.
        float newLayerWeight = Mathf.MoveTowards
            (currentLayerWeight,
            targetLayerWeight,
            Time.deltaTime * 5);
        anim.SetLayerWeight(1, newLayerWeight);
    }
    private void CheckInteractionUI()
    {
        // Delegate UI visibility entirely to the UIManager
        if (UIManager.Instance != null)
        {
            bool isNearInteractable = interactableObject != null;
            UIManager.Instance.SetInteractionPromptVisible(isNearInteractable);
        }
    }

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
        bool isActuallySprinting = false;

        if (crouchPressed)
        {
            targetSpeed = crouchSpeed;
        }
        // ONLY sprint if we have the button pressed, are moving, and aren't exhausted
        else if (enableSprint && sprintPressed && !isExhausted && moveInput != Vector2.zero)
        {
            targetSpeed = maxSpeed * sprintMultiplier;
            isActuallySprinting = true; // Flag for our stamina calculation
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
            // If Crouching, prevent Jump.
            if (jumpPressed && !crouchPressed)
            {
                velocity.y = initialJumpVelocity;
            }
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }

        HandleStamina(isActuallySprinting);
    }

    private void HandleStamina(bool isSprinting)
    {
        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
            staminaRegenTimer = 0f;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isExhausted = true; // Force the player to stop sprinting
            }
        }
        else if (currentStamina < maxStamina)
        {
            staminaRegenTimer += Time.fixedDeltaTime;

            if (staminaRegenTimer >= staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.fixedDeltaTime;

                // Once we regen past the minimum threshold, allow sprinting again
                if (currentStamina >= minStaminaToSprint)
                {
                    isExhausted = false;
                }

                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
            }
        }

        // Send the updated stamina value to the HUD
        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateStamina(currentStamina);
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

        // Null check added here to prevent errors if interactableObject was already cleared
        if (interactable != null && interactableObject != null && interactableObject.Equals(interactable))
        {
            interactableObject = null;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Reserved for future physics push interactions
    }
}