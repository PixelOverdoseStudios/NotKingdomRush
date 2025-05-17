using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Enemy : MonoBehaviour
{   
    public enum EnemyStates{
        Run,
        Attack,
        Died
    }
    public EnemyStates enemyCurrentState = EnemyStates.Run;
    private List<Vector3> path;
    private int pathIndex = 0;

    public float speed = 2f;
    public string poolTag;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector3 lastPosition;

    [Header("Combat Settings")]
    [SerializeField] public Health health;
    private float lastAttackTime;
    public float attackRange = 2f;
    public float attackRate = 1f;
    public int attackDamage = 10;
    public int goldValue = 1;
    

    //assigned when attack mode (hero script pass reference to enemy script)
    private IDamageable damageable;
    private Transform damageablePos;

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
            case EnemyStates.Died:

            break;
        }


        
    }

    //Called when in range of hero attack (hero script calls it)
    public void ChangeToAttackState(IDamageable damageable, Transform damageablePos)
    {   
        this.damageablePos = damageablePos;
        this.damageable = damageable;
        enemyCurrentState = EnemyStates.Attack;
    }

    //Called when no longer attacking hero (hero script calls it)
    public void ChangeToMoveState()
    {   
        if (enemyCurrentState != EnemyStates.Died)
        {
            enemyCurrentState = EnemyStates.Run;
            damageable = null;
        }
    }

    void Attack()
    {   
        Vector2 direction = damageablePos.position - transform.position;
        if (direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
        if (Time.time - lastAttackTime >= attackRate)
        {   
            SetAnimationState("IsAttackingSword");
            
            lastAttackTime = Time.time;
        }
    }

    public void AttackInAnimation()
    {
        if (damageable != null)
        {
            damageable.TakeDamage(attackDamage);
        }
    }

    void Move()
    {
        Vector3 target = path[pathIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        Vector3 movement = target - transform.position;

        if (animator != null)
        {
            SetAnimationState("IsWalking");
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
             //Player Loses life here
                
                MoveOverCheckpoint(); 
            }
        }
    

    void MoveOverCheckpoint()
    {
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
        GameManager.instance.CastleTakeDamage(1);
    }

    void Die()
    {   
        //Set to default so wont be targetted no more
        gameObject.layer = 0;
        GameManager.instance.AddGold(goldValue);

        enemyCurrentState = EnemyStates.Died;
        SetAnimationState("IsDead");
    }

    public void DiedOnAnimation()
    {
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
        ResetEnemy();
    }

    public void ResetEnemy()
    {   
        health.ResetHealth();
        pathIndex = 0;
    }

    void OnEnable() 
    {   
        //Set to enemy so could be targeted
        gameObject.layer = 6;

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
            case "IsAttackingSword":
                animator.SetTrigger("IsAttackingSword");
                break;
            case "IsWalking":
                animator.SetBool("IsWalking", true); 
                break;
            case "IsDead":
                animator.SetBool("IsDead", true);
                break;
        }
    }
    #endregion
}