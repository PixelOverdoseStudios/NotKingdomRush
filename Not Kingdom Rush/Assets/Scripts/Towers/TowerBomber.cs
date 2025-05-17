using UnityEngine;

public class TowerBomber : Tower
{
    [Header("Tower Animation Curves")]
    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;
    [Space]
    [SerializeField] private float trajectoryMaxHeight;

    protected override void AttackLogic()
    {
        attackTimer += Time.deltaTime;

        if (FindRandomTarget() == null) return;

        if (attackTimer >= attackCooldown)
        {
            GameObject spawnProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            spawnProjectile.GetComponent<BombProjectile>().InitializeProjectile(FindRandomTarget().transform, projectileSpeed, trajectoryMaxHeight, this.GetProjectileDamage());
            spawnProjectile.GetComponent<BombProjectile>().InitializeAnimationCurves(trajectoryAnimationCurve, axisCorrectionAnimationCurve, projectileSpeedAnimationCurve);

            attackTimer = 0;
        }
    }
}
