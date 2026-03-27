using UnityEngine;

public class MeleeDamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 10;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private bool canDamage = false;
    private bool hasDealtDamage = false;

    public void EnableDamage()
    {
        canDamage = true;
        hasDealtDamage = false;

        if (debugLogs)
            Debug.Log($"{name}: Melee damage ENABLED");
    }

    public void DisableDamage()
    {
        canDamage = false;

        if (debugLogs)
            Debug.Log($"{name}: Melee damage DISABLED");
    }

    private void OnTriggerStay(Collider other)
    {
        if (debugLogs)
            Debug.Log($"{name}: Touching {other.name}");

        if (!canDamage) return;
        if (hasDealtDamage) return;
        if (!other.CompareTag("Player")) return;

        IDamageable damageableTarget = other.GetComponent<IDamageable>();
        if (damageableTarget != null)
        {
            damageableTarget.TakeDamage(damageAmount);
            hasDealtDamage = true;

            Debug.Log($"{name}: SUCCESS - dealt {damageAmount} damage to player object {other.name}");
        }
        else
        {
            Debug.LogWarning($"{name}: Player was touched, but no IDamageable was found on {other.name}");
        }
    }
}
