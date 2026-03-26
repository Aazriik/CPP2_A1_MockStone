using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    private bool isDead = false;

    private void Start()
    {
        InitializeHealth();
    }

    private void InitializeHealth()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.CurrentData.currentHealth != -1)
        {
            currentHealth = SaveManager.Instance.CurrentData.currentHealth;

            if (currentHealth <= 0)
            {
                currentHealth = maxHealth;
            }
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateHealth(currentHealth);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        // Mathf.Max prevents health from dropping below 0
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);

        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateHealth(currentHealth);
        }

        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (isDead) return;

        // Mathf.Min prevents overhealing past maxHealth
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);

        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateHealth(currentHealth);
        }
    }

    private void Die()
    {
        isDead = true;

        if (TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.enabled = false;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
    }
}