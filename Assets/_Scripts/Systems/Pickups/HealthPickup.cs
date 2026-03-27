using UnityEngine;

public class HealthPickup : PickupBase
{
    [Tooltip("Amount of Health this restores. Use 100 to fully heal.")]
    public int healAmount = 100;

    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth healthScript = other.GetComponent<PlayerHealth>();
            if (healthScript != null)
            {
                // This MUST return true to proceed. If at full health, it returns false and does nothing.
                bool wasUsed = healthScript.Heal(healAmount);

                if (wasUsed)
                {
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    }

                    MarkAsCollectedAndDestroy();
                }
            }
        }
    }
}