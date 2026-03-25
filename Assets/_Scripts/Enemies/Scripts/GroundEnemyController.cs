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
}



