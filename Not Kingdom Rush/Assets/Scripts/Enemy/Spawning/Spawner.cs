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
        path = pathGenerator.GetOrderedPath();

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.GetComponent<Enemy>().SetPath(path);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
