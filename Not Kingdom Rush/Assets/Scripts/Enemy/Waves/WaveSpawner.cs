using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public List<WaveData> waves;
    public float spawnInterval = 0.75f;
    public float waveDelay = 3f;
    public bool autoStartNextWave = true;

    private int currentWaveIndex = 0;
    private PathManager pathManager;
    private bool waveInProgress = false;

    void Start()
    {
        pathManager = FindFirstObjectByType<PathManager>();
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        waveInProgress = true;

        foreach (var entry in wave.spawns)
        {
            List<Vector3> path = pathManager.GetPath(entry.pathKey);
            if (path == null)
            {
                Debug.LogWarning($"Path '{entry.pathKey}' not found.");
                continue;
            }

            
            ObjectPool.Instance.RegisterPrefab(entry.enemyPrefab.name, entry.enemyPrefab);

            for (int i = 0; i < entry.count; i++)
            {
                GameObject enemy = ObjectPool.Instance.SpawnFromPool(entry.enemyPrefab.name, path[0], Quaternion.identity);
                if (enemy.TryGetComponent(out Enemy e))
                {
                    e.ResetEnemy();
                    e.poolTag = entry.enemyPrefab.name;
                    e.SetPath(path);
                }

                yield return new WaitForSeconds(spawnInterval);
            }
        }

        waveInProgress = false;
        currentWaveIndex++;

        if (currentWaveIndex < waves.Count && autoStartNextWave)
        {
            yield return new WaitForSeconds(waveDelay);
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
    }

    public void StartNextWaveManually()
    {
        if (!waveInProgress && currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
    }


}
