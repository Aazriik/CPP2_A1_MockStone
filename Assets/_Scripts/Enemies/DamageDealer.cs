using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 10;

    // This example uses a trigger collider, but you could easily 
    // adapt this to OnCollisionEnter or a Raycast hit depending on your game.
    private void OnTriggerEnter(Collider other)
    {
        // Try to find the IDamageable interface on the object we hit
        IDamageable damageableTarget = other.GetComponent<IDamageable>();

        // If it has the interface, deal the damage!
        if (damageableTarget != null)
        {
            damageableTarget.TakeDamage(damageAmount);
        }
    }
}