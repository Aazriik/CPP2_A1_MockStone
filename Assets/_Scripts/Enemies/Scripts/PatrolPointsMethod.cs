using UnityEngine;

public class BeholderBehaviour : MonoBehaviour
{
    // Follow settings
    public float followRange = 15f;
    public float stopDistance = 4f;
    public float moveSpeed = 3.5f;
    public float turnSpeed = 8f;

    // Attack settings
    public float attackRange = 2.0f;        // “close enough” for Attack03
    public float attackCooldown = 1.2f;

    // Patrol settings
    public Transform[] patrolPoints;
    public float patrolStopDistance = 1.2f;

    // References
    public Transform player;
    public Animator anim;

    int currentPatrolIndex = 0;
    float attackTimer = 0f;

    void Awake()
    {
        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (!anim)
            anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!anim) return;

        attackTimer -= Time.deltaTime;

        bool hasPlayer = (player != null);
        bool inRange = hasPlayer && Vector3.Distance(transform.position, player.position) <= followRange;

        // Drives IdleNormal vs IdleBattle
        anim.SetBool("HasTarget", inRange);

        if (inRange)
            FollowPlayer();
        else
            Patrol();
    }

    void FollowPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        FaceTarget(player.position);

        // Move in until stop distance (your chase behavior)
        if (dist > stopDistance)
            MoveForward();

        // If very close → Attack03
        if (dist <= attackRange && attackTimer <= 0f)
        {
            anim.SetTrigger("Attack");
            attackTimer = attackCooldown;
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        float dist = Vector3.Distance(transform.position, targetPoint.position);

        FaceTarget(targetPoint.position);

        if (dist > patrolStopDistance)
            MoveForward();
        else
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            turnSpeed * Time.deltaTime
        );
    }

    void MoveForward()
    {
        Vector3 forward = transform.forward;
        transform.position += forward.normalized * moveSpeed * Time.deltaTime;
    }
}


