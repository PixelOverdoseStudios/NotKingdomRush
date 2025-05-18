using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    private Transform target;
    private float moveSpeed;
    private float maxMoveSpeed;
    private float trajectoryMaxRelativeHeight;
    private int bombDamage;

    private AnimationCurve trajectoryAnimationCurve;
    private AnimationCurve axisCorrectionAnimationCurve;
    private AnimationCurve projectileSpeedAnimationCurve;

    private Vector3 trajectoryStartPoint;
    private Vector3 endPoint;

    [Header("Explosion")]
    [SerializeField] private GameObject bombExplosion;
    [SerializeField] private float explosionRange;
    [SerializeField] private LayerMask whatIsEnemy;

    private void Start()
    {
        trajectoryStartPoint = transform.position;
        endPoint = target.position;
    }

    private void Update()
    {
        UpdateProjectilePosition();

        if (Vector3.Distance(transform.position, endPoint) < 0.1f)
        {
            Instantiate(bombExplosion, transform.position, Quaternion.identity);
            DamageNearbyTargets();
            AudioManager.instance.SFXBombExplosion();
            Destroy(gameObject);
        }
    }

    private void UpdateProjectilePosition()
    {
        Vector3 trajectoryRange = endPoint - trajectoryStartPoint;

        if (trajectoryRange.x < 0)
        {
            moveSpeed = -moveSpeed;
        }

        //
        float nextPositionX = transform.position.x + moveSpeed * Time.deltaTime;
        float nextPositionXNormalized = (nextPositionX - trajectoryStartPoint.x) / trajectoryRange.x;

        float nextPositionYNormalized = trajectoryAnimationCurve.Evaluate(nextPositionXNormalized);
        float nextPositionYCorrectionNormalized = axisCorrectionAnimationCurve.Evaluate(nextPositionXNormalized);
        float nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;
        float nextPositionY = trajectoryStartPoint.y + nextPositionYNormalized * trajectoryMaxRelativeHeight + nextPositionYCorrectionAbsolute;

        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, 0);

        CalculateNextProjectileSpeed(nextPositionXNormalized);

        transform.position = newPosition;
    }

    private void CalculateNextProjectileSpeed(float nextPositionXNormalized)
    {
        float nextMoveSpeedNormalized = projectileSpeedAnimationCurve.Evaluate(nextPositionXNormalized);

        moveSpeed = nextMoveSpeedNormalized * maxMoveSpeed;
    }

    public void InitializeProjectile(Transform target, float maxMoveSpeed, float trajectoryMaxHeight, int bombDamage)
    {
        this.target = target;
        this.maxMoveSpeed = maxMoveSpeed;
        this.bombDamage = bombDamage;

        float xDistanceToTarget = target.position.x - transform.position.x;
        this.trajectoryMaxRelativeHeight = Mathf.Abs(xDistanceToTarget) * trajectoryMaxHeight;
    }

    public void InitializeAnimationCurves(AnimationCurve trajectoryAnimationCurve, AnimationCurve axisCorrectionAnimationCurve, AnimationCurve projectileSpeedAnimationCurve)
    {
        this.trajectoryAnimationCurve = trajectoryAnimationCurve;
        this.axisCorrectionAnimationCurve = axisCorrectionAnimationCurve;
        this.projectileSpeedAnimationCurve = projectileSpeedAnimationCurve;
    }

    private void DamageNearbyTargets()
    {
        Collider2D[] enemiesNearby = Physics2D.OverlapCircleAll(transform.position, explosionRange, whatIsEnemy);

        if(enemiesNearby.Length > 0)
        {
            foreach (Collider2D enemy in enemiesNearby)
            {
                enemy.gameObject.GetComponent<IDamageable>().TakeDamage(bombDamage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
