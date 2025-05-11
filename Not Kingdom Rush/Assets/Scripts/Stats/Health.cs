using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead = false;

    public event Action OnDeath;
    public event Action<float> OnHealthChanged; 

    private void Awake()
    {
        ResetHealth();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        // Notify listeners about health change (for UI, effects, etc.)
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
    }

    //public bool IsDead() => isDead;

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public bool IsFullHealth()
    {
        return currentHealth == maxHealth;
    }
}