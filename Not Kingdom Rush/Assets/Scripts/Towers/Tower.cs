using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    [SerializeField] protected float attackRange;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected int projectileDamage;
    [SerializeField] protected float attackCooldown;
    protected float attackTimer;

    [Header("References")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform projectileSpawnPoint;

    [Header("Misc")]
    [SerializeField] protected bool attackRangeInEditor = true;

    protected virtual void Update()
    {
        AttackLogic();
    }

    protected virtual void AttackLogic()
    {
        attackTimer += Time.deltaTime;

        Collider2D[] objectsFound = Physics2D.OverlapCircleAll(transform.position, attackRange);

        List<GameObject> EnemiesInRange = new List<GameObject>();

        if (objectsFound.Length <= 0) return;

        if (attackTimer >= attackCooldown)
        {
            foreach (Collider2D obj in objectsFound)
            {
                if (obj.gameObject.GetComponent<IDamageable>() != null)
                {
                    EnemiesInRange.Add(obj.gameObject);
                }
            }

            int randomIndex = Random.Range(0, EnemiesInRange.Count);

            GameObject TargetSelected = EnemiesInRange[randomIndex];

            Debug.Log(TargetSelected.name + " enemy targeted");

            GameObject newProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            newProjectile.GetComponent<Projectile>().SpawnProjectileData(TargetSelected, projectileDamage, projectileSpeed);

            attackTimer = 0;    
        }
    }

    void OnDrawGizmos()
    {
        if (!attackRangeInEditor) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
