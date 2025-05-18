using UnityEngine;
using System;
using Unity.VisualScripting;


public class Health : MonoBehaviour, IDamageable
{

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    SmoothFlashEffect smoothFlashEffect;

    [SerializeField] private float currentHealth;
    private bool isDead = false;

    public event Action OnDeath;
    public event Action<float> OnHealthChanged;
    public event Action OnTakeDamage;
    public event Action OnFullHPRegen;
    public event Action OnHealthReset;

    Enemy enemy;

    private void Awake()
    {
        ResetHealth();
        enemy = GetComponent<Enemy>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        // Notify listeners about health change (for UI, effects, etc.)
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        OnTakeDamage?.Invoke();

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

        if (currentHealth == maxHealth)
        {
            OnFullHPRegen?.Invoke();
        }
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
        OnHealthReset?.Invoke();
        isDead = false;
    }

    public bool IsFullHealth()
    {
        return currentHealth == maxHealth;
    }

    [HideInInspector]
    public float distanceToCheckPoint;

    //Call this to return distance
    public float ReturnDistanceToCheckPoint()
    {
        if (enemy != null)
        {
            distanceToCheckPoint = Vector3.Distance(transform.position, enemy.path[enemy.path.Count]);
            return distanceToCheckPoint;
        }
        return Mathf.Infinity;
    }
}
