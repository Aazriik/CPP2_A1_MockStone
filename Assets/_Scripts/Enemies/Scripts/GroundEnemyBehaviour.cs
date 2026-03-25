using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GroundEnemyBehaviour : MonoBehaviour
{
    // Follow settings
    public float followRange = 15f;
    public float stopDistance = 2.5f;
    public float moveSpeed = 3f;
    public float turnSpeed = 8f;

    // Attack settings
    public float attackRange = 1.8f;
    public float attackCooldown = 1.2f;

    // Patrol settings
    public Transform[] patrolPoints;
    public float patrolStopDistance = 1.2f;

    // Gravity
    public float gravity = -9.8f;

    // References
    public Transform player;
    public Animator anim;

    bool isAttacking = false;

    CharacterController controller;
    GroundEnemyController core;

    int currentPatrolIndex = 0;
    float attackTimer = 0f;
    float yVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        core = GetComponent<GroundEnemyController>();

        if (!anim) anim = GetComponentInChildren<Animator>();

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void Update()
    {

        if (!anim || core == null || core.IsDead) return;

        attackTimer -= Time.deltaTime;

        ApplyGravity();

        bool hasPlayer = player != null;
        bool inRange = hasPlayer && Vector3.Distance(transform.position, player.position) <= followRange;

        core.SetHasTarget(inRange);

        // If player is no longer in range, cancel attack state
        if (!inRange)
        {
            isAttacking = false;

            if (anim)
                anim.ResetTrigger("Attack");
        }

        if (inRange)
            FollowPlayer();
        else
            Patrol();
    }

    void FollowPlayer()
    {
        if (!player) return;

        Vector3 targetPos = player.position;
        targetPos.y = transform.position.y;

        float dist = Vector3.Distance(transform.position, targetPos);

        // If currently attacking, do not move
        if (isAttacking)
        {
            SetSpeed(0f);
            FaceTarget(targetPos); // optional: keep facing player while attacking
            return;
        }

        // If close enough to attack, stop and trigger attack
        if (dist <= attackRange)
        {
            SetSpeed(0f);
            FaceTarget(targetPos);

            if (attackTimer <= 0f)
            {
                isAttacking = true;
                core.TriggerAttack();
                attackTimer = attackCooldown;
            }

            return;
        }

        // Otherwise chase
        FaceTarget(targetPos);

        if (dist > stopDistance)
        {
            MoveForward();
            SetSpeed(1f); // RUN
        }
        else
        {
            SetSpeed(0f);
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        // If attacking, do not patrol
        if (isAttacking)
        {
            SetSpeed(0f);
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];

        Vector3 targetPos = targetPoint.position;
        targetPos.y = transform.position.y;

        float dist = Vector3.Distance(transform.position, targetPos);

        FaceTarget(targetPos);

        if (dist > patrolStopDistance)
        {
            MoveForward();
            SetSpeed(0.5f); // WALK
        }
        else
        {
            SetSpeed(0f);
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;

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
        Vector3 move = transform.forward * moveSpeed;
        move.y = yVelocity;

        controller.Move(move * Time.deltaTime);
    }

    void SetSpeed(float speed)
    {
        if (!anim) return;
        anim.SetFloat("Speed", speed);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (yVelocity < 0f)
                yVelocity = -2f;
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }
    }

    // Animation Events
    public void AnimEvent_AttackStart()
    {
        isAttacking = true;
    }

    public void AnimEvent_AttackEnd()
    {
        isAttacking = false;
    }
}
