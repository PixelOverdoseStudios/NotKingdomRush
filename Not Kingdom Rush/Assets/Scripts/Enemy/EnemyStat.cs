using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "TD/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Basic Stats")]
    public string enemyName;
    public int maxHealth = 100;
    public float speed = 1.0f;
    public int livesTaken = 1;

    // [Header("Armor")]
    // [Range(0, 80)] public float physicalArmor = 10f; // Percentage reduction
    // [Range(0, 80)] public float magicArmor = 10f;

    [Header("Combat")]
    public int attackDamage = 10;
    public float attackRange = 2f;
    public float attackRate = 1f;
    //public DamageType damageType = DamageType.Physical;
}

//public enum DamageType { Physical, Magical, True }