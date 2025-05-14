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

    protected override void Update()
    {
        if(lineRenderer.enabled && enemyTargeted != null)
        {
            lineRenderer.SetPosition(0, projectileSpawnPoint.position);
            lineRenderer.SetPosition(1, enemyTargeted.transform.position);

            Vector2 endPoint = lineRenderer.GetPosition(1);
            laserEffect.transform.position = endPoint;
        }
        

        //base.Update();
        attackTimer += Time.deltaTime;

        Collider2D[] objectsFound = Physics2D.OverlapCircleAll(transform.position, attackRange, whatIsEnemy);

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

            if (EnemiesInRange.Count > 0)
            {
                int randomIndex = Random.Range(0, EnemiesInRange.Count);

                GameObject TargetSelected = EnemiesInRange[randomIndex];

                enemyTargeted = TargetSelected;

                StartCoroutine(PlayLaserProjectileCo(laserDuration, TargetSelected));

                TargetSelected.GetComponent<IDamageable>().TakeDamage(projectileDamage);
            }

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
