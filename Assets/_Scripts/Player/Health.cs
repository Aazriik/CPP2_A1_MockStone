using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    // Check this box in the Inspector ONLY on the Player prefab
    [SerializeField] private bool isPlayer = false;

    private int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    public event Action<int, int> OnHealthChanged; // Pass current health as parameter
    public event Action OnDeath;

    private bool dead;

    private void Start()
    {
        // Check if this is the player and if save data exists
        if (isPlayer && SaveManager.Instance != null && SaveManager.Instance.CurrentData.currentHealth != -1)
        {
            CurrentHealth = SaveManager.Instance.CurrentData.currentHealth;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        UpdateSaveData(); // Sync the starting health to memory
    }

    private void Die()
    {
        if (dead) return;
        dead = true;
        OnHealthChanged?.Invoke(0, MaxHealth); // Force UI to show 0
        OnDeath?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        if (dead || damage <= 0) return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        UpdateSaveData(); // Update memory silently

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (dead || amount <= 0) return;
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        UpdateSaveData(); // Update memory silently
    }

    public void ResetHealth()
    {
        if (dead) return;
        CurrentHealth = MaxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        UpdateSaveData(); // Update memory silently
    }

    // Helper method to keep our save data up to date in the background without writing to the disk
    private void UpdateSaveData()
    {
        if (isPlayer && SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentData.currentHealth = CurrentHealth;
        }
    }
}