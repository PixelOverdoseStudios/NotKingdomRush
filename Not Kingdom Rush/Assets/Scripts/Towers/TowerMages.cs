using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TowerMages : Tower
{
    [Header("Mage Tower Variables")]
    [SerializeField] private float laserDuration;
    [SerializeField] private GameObject laserEffect;

    private LineRenderer lineRenderer;
    private GameObject enemyTargeted;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void AttackLogic()
    {
        if (lineRenderer.enabled && enemyTargeted != null)
        {
            lineRenderer.SetPosition(0, projectileSpawnPoint.position);
            lineRenderer.SetPosition(1, enemyTargeted.transform.position);

            Vector2 endPoint = lineRenderer.GetPosition(1);
            laserEffect.transform.position = endPoint;
        }

        attackTimer += Time.deltaTime;

        if (FindRandomTarget() == null) return;

        if (attackTimer >= attackCooldown)
        {
            enemyTargeted = FindRandomTarget();

            StartCoroutine(PlayLaserProjectileCo(laserDuration, enemyTargeted));

            enemyTargeted.GetComponent<IDamageable>().TakeDamage(projectileDamage);

            attackTimer = 0;
        }
    }

    private IEnumerator PlayLaserProjectileCo(float _laserDuration, GameObject _target)
    {
        lineRenderer.enabled = true;
        laserEffect.SetActive(true);

        yield return new WaitForSeconds(_laserDuration);

        lineRenderer.enabled = false;
        laserEffect.SetActive(false);
    }
}
