using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject enemyTargeted;
    private float speed = 5f;
    private int projectileDamage = 1;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, enemyTargeted.transform.position, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, enemyTargeted.transform.position) < 0.1f)
        {
            enemyTargeted.GetComponent<IDamageable>().TakeDamage(projectileDamage);
            Destroy(this.gameObject);
        }
    }

    public void SpawnProjectileData(GameObject _enemyTargeted, int _projectileDamage, float _speed)
    {
        enemyTargeted = _enemyTargeted;
        projectileDamage = _projectileDamage;
        speed = _speed;
    }
}
