using UnityEngine;

public class StaminaPickup : PickupBase
{
    [Tooltip("Amount of Stamina this restores. Use 100 to fully restore.")]
    public float staminaAmount = 100f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                bool wasUsed = controller.RestoreStamina(staminaAmount);

                if (wasUsed)
                {
                    MarkAsCollectedAndDestroy();
                }
            }
        }
    }
}