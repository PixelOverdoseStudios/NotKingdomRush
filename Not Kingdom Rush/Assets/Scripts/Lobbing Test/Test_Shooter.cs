using UnityEngine;

public class Test_Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform target;

    [SerializeField] private float projectileMoveSpeed;
    [SerializeField] private float shootRate;
    [SerializeField] private float trajectoryMaxHeight;

    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;

    private float shootTimer;

    private void Update()
    {
        shootTimer -= Time.deltaTime;

        if(shootTimer <= 0)
        {
            shootTimer = shootRate;
            Test_Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Test_Projectile>();
            projectile.InitializeProjectile(target, projectileMoveSpeed, trajectoryMaxHeight);
            projectile.InitializeAnimationCurves(trajectoryAnimationCurve, axisCorrectionAnimationCurve, projectileSpeedAnimationCurve);
        }
    }
}
