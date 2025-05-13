using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TowerArcher : Tower
{
    [Header("Archer Tower Variables")]
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private Animator archerAnimator;
    private GameObject archerTarget;

    protected override void Update()
    {
        attackTimer += Time.deltaTime;

        Collider2D[] objectsFound = Physics2D.OverlapCircleAll(transform.position, attackRange, whatIsEnemy);

        List<GameObject> EnemiesInRange = new List<GameObject>();

        if (objectsFound.Length <= 0) return; //if no enemy is in range don't reset the timer yet

        if (attackTimer >= attackCooldown)
        {
            foreach (Collider2D obj in objectsFound)
            {
                if (obj.gameObject.GetComponent<IDamageable>() != null)
                {
                    EnemiesInRange.Add(obj.gameObject);
                }
            }

            if (EnemiesInRange.Count > 0)
            {
                int randomIndex = Random.Range(0, EnemiesInRange.Count);

                GameObject TargetSelected = EnemiesInRange[randomIndex];

                archerTarget = TargetSelected;

                Debug.Log(TargetSelected.name + " enemy targeted");

                if (TargetSelected.transform.position.x > archerPrefab.transform.position.x)
                    archerPrefab.transform.localScale = Vector3.one;
                else
                    archerPrefab.transform.localScale = new Vector3(-1, 1, 1);

                archerAnimator.SetTrigger("attack");
            }

            attackTimer = 0;
        }
    }

    public void FireArrow()
    {
        GameObject newProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        newProjectile.GetComponent<ArrowProjectile>().SpawnProjectileData(archerTarget, projectileDamage, projectileSpeed);
    }

}

