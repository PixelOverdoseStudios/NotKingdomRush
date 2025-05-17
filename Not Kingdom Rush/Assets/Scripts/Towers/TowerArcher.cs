using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TowerArcher : Tower
{
    [Header("Archer Tower Variables")]
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private Animator archerAnimator;
    private GameObject archerTarget;

    protected override void AttackLogic()
    {
        attackTimer += Time.deltaTime;

        if (FindRandomTarget() == null) return;  

        if (attackTimer >= attackCooldown)
        {
            archerTarget = FindRandomTarget();

            if (archerTarget.transform.position.x > archerPrefab.transform.position.x)
                archerPrefab.transform.localScale = Vector3.one;
            else
                archerPrefab.transform.localScale = new Vector3(-1, 1, 1);

            archerAnimator.SetTrigger("attack");

            attackTimer = 0;
        }
    }

    public void FireArrow()
    {
        GameObject newProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        newProjectile.GetComponent<ArrowProjectile>().SpawnProjectileData(archerTarget, projectileDamage, projectileSpeed);
    }
}

