using UnityEngine;

public class Hero : MonoBehaviour, IDamageable
{
    public enum HeroState { Idle, Moving, Attacking, Die }
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stoppingDistance = 0.5f;
    
    [Header("Combat Settings")]
    public Health health;
    public float attackRange = 2f;
    public float attackRate = 1f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;
    bool hasEnemy = false;

    [Header("Regen Settings")]
    public float idleTimeBeforeRegen = 3f; // Time in seconds before regen starts
    public float regenRate = 1.8f;          // How often regen occurs (in seconds)
    public int regenAmount = 10;            // How much health to restore each regen

    private float idleTimer = 0f;
    private float regenTimer = 0f;
    
    private HeroState currentState = HeroState.Idle;
    private Transform currentTarget;
    private float lastAttackTime;
    private Vector2 targetPosition;
    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        UpdateStateMachine();
    }

    void OnEnable()
    {
        health.OnDeath += ChangeToDiedState;
    }

    void OnDisable()
    {
        health.OnDeath += ChangeToDiedState;
    }
    
    private void UpdateStateMachine()
    {
        switch (currentState)
        {
            case HeroState.Idle:
                HandleIdleState();
                break;
                
            case HeroState.Moving:
                HandleMovingState();
                break;
                
            case HeroState.Attacking:
                HandleAttackingState();
                break;
            case HeroState.Die:
                Die();
                break;
        }
    }
    
    private void HandleIdleState()
    {
        // Look for enemies when idle
        FindNearestEnemy();
        SetAnimationState("IsIdling");

         // Increment idle timer
        idleTimer += Time.deltaTime;
    
        // If we've been idle long enough, start regenerating health
        if (idleTimer >= idleTimeBeforeRegen)
        {
            if (health.IsFullHealth()) return;
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenRate)
            {
                health.Heal(regenAmount);
                regenTimer = 0f;
            }
        }


        // If we found a target, transition to attacking state
        if (currentTarget != null)
        {
            currentState = HeroState.Attacking;
        }
    }
    
    private void HandleMovingState()
    {   
        idleTimer = 0f;
        regenTimer = 0f;

        // Calculate movement direction
        SetAnimationState("IsWalking");
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        
        // Flip sprite based on direction
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
        
        // Move towards target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        // Check if we've reached our destination
        if (Vector2.Distance(transform.position, targetPosition) <= stoppingDistance)
        {
            currentState = HeroState.Idle;
        }
    }
    
    private void HandleAttackingState()
    {
        // Check if target is still valid
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            hasEnemy = false;
            currentState = HeroState.Idle;
            return;
        }
        
        // Calculate distance to target
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
        
        if (distanceToTarget > attackRange)
        {
            // Move toward target
            targetPosition = currentTarget.position;
            currentState = HeroState.Moving;
        }
        else
        {   

            // Face the target
            Vector2 direction = currentTarget.position - transform.position;
            if (direction.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
            }
            
            // Attack if cooldown is over
            if (Time.time - lastAttackTime >= attackRate)
            {
                SetAnimationState("IsAttacking");
                Attack();
                lastAttackTime = Time.time;
            } 
        }
    }
    
    // Call this from your mouse tracking system when player clicks
    public void MoveToPosition(Vector2 destination)
    {
        // Clear current target when moving to new position
        currentTarget = null;
        
        // Set target position
        targetPosition = destination;
        currentState = HeroState.Moving;
    }

    public void TakeDamage(int amount)
    {
        health.TakeDamage(amount);
    }
    
    private void ChangeToDiedState()
    {
        currentState = HeroState.Die;
    }

    private void Die()
    {
        SetAnimationState("IsDead");
    }
    
    private void FindNearestEnemy()
    {   
        if (hasEnemy) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange * 1.5f, enemyLayer);
        
        if (hits.Length > 0)
        {
            // Find closest enemy
            Transform closest = null;
            float closestDistance = Mathf.Infinity;
            
            foreach (Collider2D hit in hits)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = hit.transform;
                }
            }
            
            currentTarget = closest;

            currentTarget.GetComponent<Enemy>().ChangeToAttackState(health);
            hasEnemy = true;
        }
    }
    
    private void Attack()
    {
        // Check if target is still in range
        if (currentTarget != null && Vector2.Distance(transform.position, currentTarget.position) <= attackRange)
        {
            // Apply damage to target
            IDamageable damageable = currentTarget.GetComponent<Health>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    //Called in mouse tracker everytime hero moves reset target
    public void ResetEnemyState()
    {
        if (currentTarget != null)
        {
            currentTarget.GetComponent<Enemy>().ChangeToMoveState();

            hasEnemy = false;
        }
    }
    
    // Visualize attack range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    #region Animation

    public void SetAnimationState(string boolName)
    {
        if (animator == null) return;

        // First reset all animation bools to false
        
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsIdling", false);
        animator.SetBool("IsDead", false);

        // Then set only the desired bool to true
        switch (boolName)
        {   
            case "IsIdling" :
                animator.SetBool("IsIdling", true);
                break;
            case "IsAttacking":
                animator.SetTrigger("IsAttackingSword");
                break;
            case "IsWalking":
                animator.SetBool("IsWalking", true); 
                break;
            case "IsDead":
                animator.SetBool("IsDead", true);
                break;


            default:
                Debug.LogWarning($"Unknown animation bool parameter: {boolName}");
                break;
        }
    }
    #endregion
}