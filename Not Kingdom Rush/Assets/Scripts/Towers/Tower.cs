using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackCooldown;
    protected float attackTimer;

    [Header("Misc")]
    [SerializeField] protected bool attackRangeInEditor = true;

    protected virtual void AttackLogic() { }

    void OnDrawGizmos()
    {
        if (!attackRangeInEditor) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
