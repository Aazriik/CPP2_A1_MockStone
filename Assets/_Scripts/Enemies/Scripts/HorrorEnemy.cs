using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Idle,
        Investigating,
        Searching,
        Chasing,
        EndgameEscape
    }

    public State currentState = State.Idle;

    private Animator animator;
    private Transform player;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Detection")]
    public float visionRange = 15f;
    public float visionAngle = 60f;

    [Header("Ping Settings")]
    public float pingDuration = 3.5f;
    private float pingTimer = 0f;
    private Vector3 lastKnownPosition;

    [Header("Endgame")]
    public Transform exitPoint;

    private Vector3 targetPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
       
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
               
                break;

            case State.Investigating:
                
                MoveTowards(lastKnownPosition, patrolSpeed);
                if (Vector3.Distance(transform.position, lastKnownPosition) < 0.5f)
                {
                    currentState = State.Searching;
                    PickRandomSearchPoint();
                }
                DetectPlayer();
                break;

            case State.Searching:
                
                MoveTowards(targetPosition, patrolSpeed);
                if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
                {
                    PickRandomSearchPoint();
                }
                DetectPlayer();
                break;

            case State.Chasing:
                
                MoveTowards(player.position, chaseSpeed);
                pingTimer -= Time.deltaTime;
                if (pingTimer <= 0f)
                {
                    currentState = State.Searching;
                    PickRandomSearchPoint();
                    animator.SetBool("isChase", false);
                }
                break;

            case State.EndgameEscape:
                
                MoveTowards(exitPoint.position, chaseSpeed);
                break;
        }
    }

   
    // PHASE 1: Activation
    
    public void OnCollectiblePickedUp(Vector3 collectiblePos)
    {
        lastKnownPosition = collectiblePos;
        currentState = State.Investigating;
    }

   
    // Movement helper 
    
    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotate towards target
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
        }
    }

    
    // PHASE 2: Pick random search point
    
    void PickRandomSearchPoint()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-5f, 5f),
            0f,
            Random.Range(-5f, 5f)
        );
        targetPosition = lastKnownPosition + randomOffset;
    }

    
    // Detection (line of sight)
    
    void DetectPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < visionRange)
        {
            float angle = Vector3.Angle(transform.forward, direction);
            if (angle < visionAngle)
            {
                Ray ray = new Ray(transform.position + Vector3.up * 1.5f, direction);
                if (Physics.Raycast(ray, out RaycastHit hit, visionRange))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        StartChase(player.position);
                    }
                }
            }
        }
    }

    void StartChase(Vector3 playerPos)
    {
        lastKnownPosition = playerPos;
        pingTimer = pingDuration;
        currentState = State.Chasing;
        animator.SetBool("isChase", true);
    }

    
    // PHASE 5: Endgame Escape
    
    public void TriggerEndgame()
    {
        currentState = State.EndgameEscape;
    }
}