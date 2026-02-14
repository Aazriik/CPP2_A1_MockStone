using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Respawn")]
    public Transform respawnPoint;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player died");

        // Reset health
        currentHealth = maxHealth;

        // Respawn
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            Debug.LogWarning("No respawn point set!");
        }
    }
}