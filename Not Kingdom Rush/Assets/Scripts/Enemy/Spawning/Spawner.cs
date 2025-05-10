using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1.5f;
    public int enemiesPerWave = 10;

    private PathManager pathGenerator;
    private List<Vector3> path;

    void Start()
    {
        pathGenerator = FindFirstObjectByType<PathManager>();

        
        ObjectPool.Instance.RegisterPrefab("Enemy", enemyPrefab);

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject enemy = ObjectPool.Instance.SpawnFromPool("Enemy", path[0], Quaternion.identity);
            if (enemy != null)
                enemy.GetComponent<Enemy>().SetPath(path);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
