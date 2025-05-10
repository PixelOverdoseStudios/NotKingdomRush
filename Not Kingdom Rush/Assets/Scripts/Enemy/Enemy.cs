using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private List<Vector3> path;
    private int pathIndex = 0;

    public float speed = 2f;

    public void SetPath(List<Vector3> newPath)
    {
        path = newPath;
        pathIndex = 0;
        transform.position = path[pathIndex];
    }

    void Update()
    {
        if (path == null || pathIndex >= path.Count)
            return;

        transform.position = Vector3.MoveTowards(transform.position, path[pathIndex], speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, path[pathIndex]) < 0.05f)
        {
            pathIndex++;

            if (pathIndex >= path.Count)
            {
                Destroy(gameObject);
            }
        }
    }
}
