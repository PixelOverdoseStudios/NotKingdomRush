using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour, IObjectInteractable
{
    [SerializeField] protected TowerLevelStats[] towerLevelStats;

    [Header("Tower Stats")]
    [SerializeField] protected int towerLevel;
    [SerializeField] protected float projectileSpeed;
    protected float attackRange;
    protected int projectileDamage;
    protected float attackCooldown;
    protected float attackTimer;

    [Header("References")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform projectileSpawnPoint;
    [SerializeField] protected GameObject inGameAttackRange;
    [SerializeField] protected GameObject UICanvas;
    [SerializeField] protected LayerMask whatIsEnemy;

    [Header("Misc")]
    [SerializeField] protected Vector3 rangeOffset;
    [SerializeField] protected bool attackRangeInEditor = true;

    protected virtual void Start()
    {
        UpdateTowerStats();
        attackTimer = attackCooldown - 0.5f;
    }

    protected virtual void Update()
    {
        AttackTimer();
    }

    protected virtual void AttackTimer()
    {
        attackTimer += Time.deltaTime;

        Collider2D[] objectsFound = Physics2D.OverlapCircleAll(transform.position + rangeOffset, attackRange, whatIsEnemy);

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

            AttackLogic(EnemiesInRange);  
        }
    }

    protected virtual void AttackLogic(List<GameObject> _enemiesInRange)
    {
        if (_enemiesInRange.Count > 0)
        {
            int randomIndex = Random.Range(0, _enemiesInRange.Count);

            GameObject TargetSelected = _enemiesInRange[randomIndex];

            Debug.Log(TargetSelected.name + " enemy targeted");

            GameObject newProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            newProjectile.GetComponent<Projectile>().SpawnProjectileData(TargetSelected, projectileDamage, projectileSpeed);
        }

        attackTimer = 0;
    }

    public void ObjectClickedOn()
    {
        inGameAttackRange.SetActive(true);
        //inGameAttackRange.GetComponent<InGameAttackRange>().UpdateScale(attackRange);
        UICanvas.SetActive(true);
    }

    public void ObjectClickedOff()
    {
        inGameAttackRange.SetActive(false);
        UICanvas.GetComponentInChildren<Animator>().SetTrigger("despawnButtons");
    }

    public void ObjectIsBeingHovered() { }

    void OnDrawGizmos()
    {
        if (!attackRangeInEditor) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + rangeOffset, towerLevelStats[towerLevel].attackRange);
    }

    public float GetAttackRange() => attackRange;

    private void UpdateTowerStats()
    {
        attackRange = towerLevelStats[towerLevel].attackRange;
        projectileDamage = towerLevelStats[towerLevel].projectileDamage;
        attackCooldown = towerLevelStats[towerLevel].attackCooldown;
    }

    public void UpgradeTower()
    {
        if(towerLevel < 2)
        {
            towerLevel++;
            UpdateTowerStats();
            inGameAttackRange.GetComponent<InGameAttackRange>().UpdateScale();
        }
        else
        {
            Debug.Log("ERROR: This tower is maxed out.");
        }
    }

    public int GetTowerLevel() => towerLevel;

    public Vector3 GetRangeOffset() => rangeOffset;
}


[System.Serializable]
public class TowerLevelStats
{
    [Header("Stats")]
    public float attackRange;
    public int projectileDamage;
    public float attackCooldown;
}
