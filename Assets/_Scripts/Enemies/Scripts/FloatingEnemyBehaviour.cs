using UnityEngine;

public class BeholderBehaviour : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 50;
    private int currentHealth;
    private bool isDead = false;

    [Header("Follow settings")]
    public float followRange = 15f;
    public float stopDistance = 4f;
    public float moveSpeed = 3.5f;
    public float turnSpeed = 8f;

    [Header("Attack settings")]
    public float attackRange = 2.0f;
    public float attackCooldown = 1.2f;
    public float attackDuration = 0.3f;
    public int attackDamage = 10;
    public float attackRadius = 1.5f;
    public Transform attackPoint;
    public LayerMask damageableLayers;

    [Header("Patrol settings")]
    public Transform[] patrolPoints;
    public float patrolStopDistance = 1.2f;

    [Header("References")]
    public Transform player;
    public Animator anim;

    int currentPatrolIndex = 0;
    float attackTimer = 0f;

    bool isAttacking = false;
    float attackHitTimer = 0f;

    void Awake()
    {
        currentHealth = maxHealth;

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (!anim)
            anim = GetComponentInChildren<Animator>();

        if (!attackPoint)
            attackPoint = transform;

        damageableLayers = LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (isDead || !anim) return;

        attackTimer -= Time.deltaTime;

        float distanceToPlayer = player
            ? Vector3.Distance(transform.position, player.position)
            : Mathf.Infinity;

        bool hasPlayer = player != null;
        bool inRange = hasPlayer && distanceToPlayer <= followRange;
        bool inAttackRange = hasPlayer && distanceToPlayer <= attackRange;

        anim.SetBool("HasTarget", inRange);

        // ---------------------
        // ATTACK TIMING
        // ---------------------
        if (isAttacking)
        {
            attackHitTimer -= Time.deltaTime;

            // 🔥 Cancel attack if player escapes
            if (!inAttackRange)
            {
                CancelAttack();
                return;
            }

            if (attackHitTimer <= 0f)
            {
                DealDamage();
                isAttacking = false;
            }
        }

        if (inRange)
            FollowPlayer(distanceToPlayer);
        else
            Patrol();
    }

    void FollowPlayer(float dist)
    {
        if (!player) return;

        FaceTarget(player.position);

        if (isAttacking) return;

        if (dist <= attackRange && attackTimer <= 0f)
        {
            StartAttack();
            return;
        }

        if (dist > stopDistance)
            MoveForward();
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || isAttacking)
            return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        float dist = Vector3.Distance(transform.position, targetPoint.position);

        FaceTarget(targetPoint.position);

        if (dist > patrolStopDistance)
            MoveForward();
        else
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;
        attackHitTimer = attackDuration;

        if (anim)
            anim.SetTrigger("Attack");
    }

    void CancelAttack()
    {
        isAttacking = false;
        attackHitTimer = 0f;

        if (anim)
            anim.ResetTrigger("Attack");

        Debug.Log("[Beholder] Attack cancelled - player out of range");
    }

    void DealDamage()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            IDamageable target = player.GetComponent<IDamageable>();

            if (target != null)
            {
                Debug.Log($"[Beholder] Direct hit on player for {attackDamage} damage");
                target.TakeDamage(attackDamage);
            }
            else
            {
                Debug.Log("[Beholder] Player has no IDamageable");
            }
        }
        else
        {
            Debug.Log("[Beholder] Attack missed - player out of range");
        }
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
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void OnDrawGizmosSelected()
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

       

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;

        

        this.enabled = false;
        Destroy(gameObject);
    }
}


