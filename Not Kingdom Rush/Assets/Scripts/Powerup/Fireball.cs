using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Visual Effects")]
    public GameObject impactEffect;
    public float fallSpeed = 10f;
    public float rotationSpeed = 180f;


    private Vector2 targetPosition;
    private int damage;
    private float splashRadius;
    private LayerMask enemyLayer;
    private bool hasImpacted = false;
    

    public void Initialize(Vector2 target, int dmg, float radius, LayerMask layer)
    {
        targetPosition = target;
        damage = dmg;
        splashRadius = radius;
        enemyLayer = layer;
    }

    private void Update()
    {
        if (hasImpacted) return;

        // Move downward
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

        // Rotate for visual effect
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Scale based on distance to ground
        // float distance = Vector2.Distance(transform.position, targetPosition);
        // float scale = scaleCurve.Evaluate(1 - (distance / fallHeight));
        // transform.localScale = Vector3.one * scale;

        // Check for impact
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            Impact();
        }
    }

    private void Impact()
    {
        hasImpacted = true;

        // Visual effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Damage enemies in splash radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, splashRadius, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}