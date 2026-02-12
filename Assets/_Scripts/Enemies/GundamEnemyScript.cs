using UnityEngine;

public class GundamEnemyScript : MonoBehaviour
{
    Animator anim;

    public float speed = 2f;
    public float distance = 3f;

    public Vector3 direction = Vector3.forward;

    private Vector3 startPos;

    [Header("Movement Settings")]
    [SerializeField] private float initSpeed = 2.0f;
    [SerializeField] private float maxSpeed = 7.0f;
    [SerializeField] private float acceleration = 3.0f;

    private LayerMask playerLayer;
    private float currentSpeed = 0.0f;

    [SerializeField] float turnSpeed = 10f;

    Rigidbody rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        direction = direction.normalized;
        currentSpeed = Mathf.Max(initSpeed, 0.1f);

    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * currentSpeed * Time.fixedDeltaTime);

        CheckDistance();

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime
            );

            rb.MoveRotation(newRotation);
        }
    }
        void Update()
    {
        

        Ray newRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        Debug.DrawRay(newRay.origin, newRay.direction * 10.0f, Color.red, 0.1f);

        if (Physics.Raycast(newRay, out hitInfo, 10.0f, playerLayer))
        {
            //add all info regarding detecting/chasing/speeding up
            Debug.Log("Cube detected: " + hitInfo.collider.gameObject.name);
        }
    }

    void CheckDistance()
    {
        if (Vector3.Distance(startPos, rb.position) >= distance)
        {
            direction *= -1;
            startPos = rb.position;
        }
    }

    void TurnTowardsDirection()
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        rb.MoveRotation(targetRotation);
    }
}
