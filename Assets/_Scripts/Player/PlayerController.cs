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

    #region Input Handling
    void OnEnable()
    {
        InputManager.Instance.OnMoveEvent += OnMove;
        InputManager.Instance.OnJumpEvent += OnJump;
    }
    void OnDisable()
    {
        InputManager.Instance.OnMoveEvent -= OnMove;
        InputManager.Instance.OnJumpEvent -= OnJump;
    }

    void OnMove(Vector2 input) => moveInput = input;
    void OnJump(bool pressed) => jumpPressed = pressed;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        CalculateJumpVariables();

        stairsLayer = LayerMask.GetMask("Stairs");
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
        Ray newRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        Debug.DrawRay(newRay.origin, newRay.direction * 10.0f, Color.red, 0.1f);
        bool hitSomething = Physics.Raycast(newRay, out hitInfo, 10.0f, stairsLayer);
        if (hitSomething)
        {
            Debug.Log("Stairs detected: " + hitInfo.collider.gameObject.name);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateCharacterVelocity();

        cc.Move(velocity * Time.fixedDeltaTime);
    }

    void UpdateCharacterVelocity()
    {
        velocity.x = moveInput.x * 5f;
        velocity.z = moveInput.y * 5f;

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

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("Collision Detected with " + collision.gameObject.name);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("Controller hit " + hit.gameObject.name);
    }
}