using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TowerArcher : Tower
{
    private void Update()
    {
        AttackLogic();
    }

    protected override void AttackLogic()
    {
        attackTimer += Time.deltaTime;

        Collider2D[] objectsFound = Physics2D.OverlapCircleAll(transform.position, attackRange);

        if (objectsFound.Length <= 0) return;

        if (attackTimer >= attackCooldown)
        {
            foreach (Collider2D obj in objectsFound)
            {
                if (obj.gameObject.CompareTag("Enemy"))
                    Debug.Log(obj.gameObject.name + " is in range");
            }

            attackTimer = 0;
        }
    }
}
