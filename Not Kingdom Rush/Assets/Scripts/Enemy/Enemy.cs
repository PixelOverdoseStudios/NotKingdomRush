using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private List<Vector3> path;
    private int pathIndex = 0;
    public float health = 100f;
    public float speed = 2f;
    public string poolTag;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 lastPosition;

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

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }

    public void ResetEnemy()
    {
        health = 100f;
        pathIndex = 0;
    }

    void OnEnable()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

    }
}
