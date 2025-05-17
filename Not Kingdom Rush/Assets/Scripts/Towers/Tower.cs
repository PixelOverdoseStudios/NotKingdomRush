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
    protected int upgradeCost;

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
        AttackLogic();
    }

    #region Attack Methods
    protected virtual void AttackLogic()
    {
        attackTimer += Time.deltaTime;

        if (FindRandomTarget() == null) return;

        GameObject TargetSelected = FindRandomTarget();

        if (attackTimer >= attackCooldown)
        {
            Debug.Log(TargetSelected.name + " enemy targeted. Override method to get functionality for your towers.");
            attackTimer = 0;
        }
    }

    protected virtual GameObject FindRandomTarget()
    {
        Collider2D[] objectsNearby = Physics2D.OverlapCircleAll(transform.position, attackRange, whatIsEnemy);

        if (objectsNearby.Length > 0)
        {
            int randomIndex = Random.Range(0, objectsNearby.Length);

            return objectsNearby[randomIndex].gameObject;
        }
        else return null;
    }
    #endregion

    #region Upgrade Methods
    private void UpdateTowerStats()
    {
        attackRange = towerLevelStats[towerLevel].attackRange;
        projectileDamage = towerLevelStats[towerLevel].projectileDamage;
        attackCooldown = towerLevelStats[towerLevel].attackCooldown;
        upgradeCost = towerLevelStats[towerLevel].upgradeCost;
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
    #endregion

    #region Mouse Interfaces
    public void ObjectClickedOn()
    {
        inGameAttackRange.SetActive(true);
        UICanvas.SetActive(true);
    }

    public void ObjectClickedOff()
    {
        inGameAttackRange.SetActive(false);
        UICanvas.GetComponentInChildren<Animator>().SetTrigger("despawnButtons");
    }

    public void ObjectIsBeingHovered() { }
    #endregion   

    #region Getter and Setters
    public int GetTowerLevel() => towerLevel;
    public float GetAttackRange() => attackRange;
    public int GetProjectileDamage() => projectileDamage;
    public int GetUpgradeCost() => upgradeCost;
    public Vector3 GetRangeOffset() => rangeOffset;
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (!attackRangeInEditor) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + rangeOffset, towerLevelStats[towerLevel].attackRange);
    }
    #endregion
}


[System.Serializable]
public class TowerLevelStats
{
    [Header("Stats")]
    public float attackRange;
    public int projectileDamage;
    public float attackCooldown;
    public int upgradeCost;
}
