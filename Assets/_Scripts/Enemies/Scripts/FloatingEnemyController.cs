using UnityEngine;

public class BeholderController : MonoBehaviour
{
    // References
    public Animator anim;
    public Transform player;

    // Health settings
    public int maxHP = 40;

    public bool IsDead => isDead;

    int hp;
    bool isDead;

    void Awake()
    {
        hp = maxHP;

        if (!anim) anim = GetComponentInChildren<Animator>();

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    public void SetHasTarget(bool hasTarget)
    {
        if (!anim) return;
        anim.SetBool("HasTarget", hasTarget);
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        hp -= dmg;

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            if (anim) anim.SetTrigger("Hit");
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

        // optional: disable colliders
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }
}



