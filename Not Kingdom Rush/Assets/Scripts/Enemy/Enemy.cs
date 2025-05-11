using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Enemy : MonoBehaviour
{   
    public enum EnemyStates{
        Run,
        Attack
    }
    public EnemyStates enemyCurrentState = EnemyStates.Run;
    private List<Vector3> path;
    private int pathIndex = 0;
    public float speed = 2f;
    public string poolTag;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 lastPosition;

    [Header("Combat Settings")]
    [SerializeField] public Health health;
    private float lastAttackTime;
    public float attackRange = 2f;
    public float attackRate = 1f;
    public int attackDamage = 10;

    //assigned when attack mode (hero script pass reference to enemy script)
    private IDamageable damageable;

    //Reference to hero script
    Hero hero;

    public void SetPath(List<Vector3> newPath)
    {
        path = newPath;
        pathIndex = 0;
        transform.position = path[pathIndex];
    }

    void Update()
    {
        if (path == null || pathIndex >= path.Count)
            return;

        switch (enemyCurrentState){
            case EnemyStates.Run:
                Move();
            break;

            case EnemyStates.Attack:
                Attack();
            break;
        }


        
    }

    //Called when in range of hero attack (hero script calls it)
    public void ChangeToAttackState(IDamageable damageable)
    {   
        this.damageable = damageable;
        enemyCurrentState = EnemyStates.Attack;
    }

    //Called when no longer attacking hero (hero script calls it)
    public void ChangeToMoveState()
    {
        enemyCurrentState = EnemyStates.Run;
        damageable = null;
    }

    void Attack()
    {
        if (Time.time - lastAttackTime >= attackRate)
        {
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
            lastAttackTime = Time.time;
        }
    }

    void Move()
    {
        Vector3 target = path[pathIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        Vector3 movement = target - transform.position;

        if (animator != null)
        {
            animator.SetBool("IsWalking", movement.magnitude > 0.01f);
        }

        if (spriteRenderer != null)
        {
            if (movement.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (movement.x < -0.01f)
                spriteRenderer.flipX = true;
        }

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            pathIndex++;

            if (pathIndex >= path.Count)
            {
                //Player Loses life here
                Die(); 
            }
        }
    }

    void Die()
    {
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
        
    }

    public void ResetEnemy()
    {
        health.ResetHealth();
        pathIndex = 0;
    }

    void OnEnable() 
    {   
        enemyCurrentState = EnemyStates.Run;
        health.OnDeath += Die;

        if (animator == null)
            animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnDisable()
    {
        health.OnDeath -= Die;
    }
}
