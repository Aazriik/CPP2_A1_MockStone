using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerSimple : MonoBehaviour
{
    // Reference to the camera for mouse look
    [SerializeField] private Transform cameraTransform;

    // Movement settings
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float runSpeed = 7.5f;
    [SerializeField] private float acceleration = 12f;

    // Jump and gravity settings
    [SerializeField] private float jumpHeight = 1.6f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundStickForce = -2f;

    // Look settings
    [SerializeField] private float lookSensitivity = 2.0f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private CharacterController cc;

    private float pitch;
    private Vector3 velocity;          // y velocity lives here
    private Vector3 moveVel;           // smoothed horizontal velocity

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
        JumpAndGravity();
    }

    void Look()
    {
        if (!cameraTransform) return;

        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // yaw (player)
        transform.Rotate(Vector3.up * mouseX);

        // pitch (camera)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(x, 0f, z);
        input = Vector3.ClampMagnitude(input, 1f);

        bool running = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = running ? runSpeed : walkSpeed;

        // camera-relative movement (yaw only)
        Vector3 moveDir = transform.right * input.x + transform.forward * input.z;
        Vector3 targetVel = moveDir * targetSpeed;

        // smooth acceleration
        moveVel = Vector3.MoveTowards(moveVel, targetVel, acceleration * Time.deltaTime);

        cc.Move(moveVel * Time.deltaTime);
    }

    void JumpAndGravity()
    {
        if (cc.isGrounded && velocity.y < 0f)
            velocity.y = groundStickForce;

        if (cc.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // v = sqrt(2 * jumpHeight * -gravity)
            velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}

