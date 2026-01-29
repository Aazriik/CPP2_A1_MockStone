using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController cc;

    [Header("Jump Settiings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float timeToJumpApex = 0.4f;

    private float gravity;
    private float initalJumpVelocity;

    private Vector2 moveInput = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    private bool jumpPressed = false;

    private LayerMask stairsLayer;

    // --- LOOK ADDED (minimal) ---
    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity = 2f;

    private Vector2 lookInput = Vector2.zero;
    private float xRotation = 0f;
    // ---------------------------

    #region Input Handling
    void OnEnable()
    {
        InputManager.Instance.OnMoveEvent += OnMove;
        InputManager.Instance.OnJumpEvent += OnJump;

        InputManager.Instance.OnLookEvent += OnLook;
    }
    void OnDisable()
    {
        InputManager.Instance.OnMoveEvent -= OnMove;
        InputManager.Instance.OnJumpEvent -= OnJump;

        InputManager.Instance.OnLookEvent -= OnLook;
    }

    void OnMove(Vector2 input) => moveInput = input;
    void OnJump(bool pressed) => jumpPressed = pressed;
    void OnLook(Vector2 input) => lookInput = input;
    #endregion

    void Start()
    {
        cc = GetComponent<CharacterController>();
        CalculateJumpVariables();

        stairsLayer = LayerMask.GetMask("Stairs");

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
        initalJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    private void Update()
    {
        UpdateLook();

        Ray newRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        Debug.DrawRay(newRay.origin, newRay.direction * 10.0f, Color.red, 0.1f);
        bool hitSomething = Physics.Raycast(newRay, out hitInfo, 10.0f, stairsLayer);
        if (hitSomething)
        {
            Debug.Log("Stairs detected: " + hitInfo.collider.gameObject.name);
        }
    }

    void FixedUpdate()
    {
        UpdateCharacterVelocity();
        cc.Move(velocity * Time.fixedDeltaTime);
    }

    void UpdateCharacterVelocity()
    {
        // Camera-relative movement using camera YAW only (best practice)
        float yaw = (cameraTransform != null) ? cameraTransform.eulerAngles.y : transform.eulerAngles.y;
        Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);

        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize(); // no faster diagonals

        Vector3 moveDir = yawRotation * inputDir;

        velocity.x = moveDir.x * 5f;
        velocity.z = moveDir.z * 5f;

        // Jump / gravity 
        if (cc.isGrounded)
        {
            velocity.y = -cc.skinWidth;
            if (jumpPressed)
                velocity.y = initalJumpVelocity;
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }

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

    void UpdateLook()
    {
        // Horizontal look (rotate player body)
        float mouseX = lookInput.x * lookSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        // Vertical look (rotate camera up/down)
        float mouseY = lookInput.y * lookSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("Collision Detected with " + collision.gameObject.name);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("Controller hit " + hit.gameObject.name);
    }
}