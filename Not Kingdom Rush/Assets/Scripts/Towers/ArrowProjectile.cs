using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public GameObject target;
    private float speed = 5f;
    private int projectileDamage = 1;

    private void Update()
    {
        Vector2 direction = (Vector2)target.transform.position - (Vector2)transform.position;
        float angel = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angel);

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
        {
            if(target.GetComponent<IDamageable>() != null)
                target.GetComponent<IDamageable>().TakeDamage(projectileDamage);
            Destroy(this.gameObject);
        }
    }

    public void SpawnProjectileData(GameObject _enemyTargeted, int _projectileDamage, float _speed)
    {
        target = _enemyTargeted;
        projectileDamage = _projectileDamage;
        speed = _speed;
    }
}
