using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GroundEnemyBehaviour : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Follow Settings")]
    public float followRange = 15f;
    public float stopDistance = 2.5f;
    public float moveSpeed = 3f;
    public float turnSpeed = 8f;

    [Header("Attack Settings")]
    public float attackRange = 1.8f;
    public float attackCooldown = 1.2f;
    public int attackDamage = 10;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask damageableLayers;
    public float attackDuration = 0.3f; // duration of the "hit" phase

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolStopDistance = 1.2f;

    [Header("Gravity")]
    public float gravity = -9.8f;

    [Header("References")]
    public Transform player;
    public Animator anim;

    private CharacterController controller;
    private GroundEnemyController core;

    private bool isDead = false;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float attackHitTimer = 0f;
    private int currentPatrolIndex = 0;
    private float yVelocity;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        core = GetComponent<GroundEnemyController>();
        currentHealth = maxHealth;

        if (!anim)
            anim = GetComponentInChildren<Animator>();

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p)
                player = p.transform;
        }

        if (!attackPoint)
            attackPoint = transform; // default to enemy center

        damageableLayers = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        if (isDead || core == null || anim == null)
            return;

        attackTimer -= Time.deltaTime;

        // Handle attack "hit window"
        if (isAttacking)
        {
            attackHitTimer -= Time.deltaTime;
            if (attackHitTimer <= 0f)
            {
                DealDamage();
                isAttacking = false; // attack finished
            }
        }

        ApplyGravity();

        bool hasPlayer = player != null;
        bool inRange = hasPlayer && Vector3.Distance(transform.position, player.position) <= followRange;
        bool inRangeAttack = hasPlayer && Vector3.Distance(transform.position, player.position) <= attackRange;
        core.SetHasTarget(inRange);

        if (!inRangeAttack)
        {
            isAttacking = false;
            anim.ResetTrigger("Attack");
        }

        if (inRange)
            FollowPlayer();
        else
            Patrol();


    }

    private void FollowPlayer()
    {
        if (!player)
            return;

        Vector3 targetPos = player.position;
        targetPos.y = transform.position.y;
        float dist = Vector3.Distance(transform.position, targetPos);

        FaceTarget(targetPos);

        if (isAttacking)
        {
            SetSpeed(0f);
            return;
        }

        if (dist <= attackRange && attackTimer <= 0f)
        {
            StartAttack();
            return;
        }

        if (dist > stopDistance)
        {
            MoveForward();
            SetSpeed(1f);
        }
        else
        {
            SetSpeed(0f);
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || isAttacking)
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
            SetSpeed(0.5f);
        }
        else
        {
            SetSpeed(0f);
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void FaceTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
    }

    private void MoveForward()
    {
        Vector3 move = transform.forward * moveSpeed;
        move.y = yVelocity;
        controller.Move(move * Time.deltaTime);
    }

    private void SetSpeed(float speed)
    {
        if (anim)
            anim.SetFloat("Speed", speed);
    }

    private void ApplyGravity()
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

    // ---------------------
    // ATTACK SYSTEM (timed)
    // ---------------------
    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;
        attackHitTimer = attackDuration;

        if (anim)
            anim.SetTrigger("Attack");
    }

    private void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, damageableLayers);

        Debug.Log($"[Enemy] Attack triggered. Hits found: {hits.Length}");

        foreach (Collider hit in hits)
        {
            Debug.Log($"[Enemy] Hit object: {hit.name}");

            IDamageable target = hit.GetComponent<IDamageable>();

            if (target != null && target != this)
            {
                Debug.Log($"[Enemy] Dealing {attackDamage} damage to {hit.name}");
                target.TakeDamage(attackDamage);
            }
            else
            {
                Debug.Log($"[Enemy] {hit.name} has no IDamageable component");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

    // ---------------------
    // DAMAGE SYSTEM
    // ---------------------
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (anim)
            anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        if (anim)
            anim.SetTrigger("Die");
        this.enabled = false;
    }

    public bool IsDead() => isDead;
}