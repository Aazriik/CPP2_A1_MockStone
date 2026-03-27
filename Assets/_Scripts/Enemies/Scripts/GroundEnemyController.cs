using UnityEngine;

public class GroundEnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator anim;

    [Header("Health")]
    public int maxHP = 50;

    public bool IsDead => isDead;

    int currentHP;
    bool isDead;

    [Header("Attack")]
    public int attackDamage = 10;
    public float attackRange = 2f;
    public LayerMask playerLayer;

    public float attackDelay = 2.0f;

    void Awake()
    {
        currentHP = maxHP;

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (!anim)
            anim = GetComponentInChildren<Animator>();
    }

    public void SetHasTarget(bool hasTarget)
    {
        if (!anim) return;
        anim.SetBool("HasTarget", hasTarget);
    }

    public void TriggerAttack()
    {
        if (!anim || isDead) return;

        anim.SetTrigger("Attack");
        Invoke(nameof(DealDamage), attackDelay);
    }

    public void TriggerHit()
    {
        if (!anim || isDead) return;
        anim.SetTrigger("Hit");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            TriggerHit();
        }
    }

    void Die()
    {
        isDead = true;

        if (anim)
        {
            anim.SetBool("IsDead", true);
            anim.SetTrigger("Die");
        }

        CharacterController cc = GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }

    public void DealDamage()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            IDamageable damageable = player.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }
}



