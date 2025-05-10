using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathManager : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase pathTile;

    [Header("Path Options")]
    public bool reversePath = false;

    private List<Vector3> orderedPath;

    void Awake()
    {
        orderedPath = GenerateOrderedPath();
        if (reversePath)
            orderedPath.Reverse();
    }

    public List<Vector3> GetOrderedPath() => orderedPath;

    List<Vector3> GenerateOrderedPath()
    {
        HashSet<Vector3Int> pathCells = new HashSet<Vector3Int>();

        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(cell) == pathTile)
                {
                    pathCells.Add(cell);
                }
            }
        }

        if (pathCells.Count == 0)
        {
            Debug.LogError("No path tiles found!");
            return new List<Vector3>();
        }

        
        Vector3Int current = FindEndpoint(pathCells);
        List<Vector3> ordered = new List<Vector3>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        while (current != null)
        {
            ordered.Add(tilemap.GetCellCenterWorld(current));
            visited.Add(current);

            Vector3Int next = GetNextNeighbor(current, pathCells, visited);
            if (next == Vector3Int.zero)
                break;

            current = next;
        }

        return ordered;
    }

    Vector3Int FindEndpoint(HashSet<Vector3Int> pathCells)
    {
        foreach (var cell in pathCells)
        {
            int count = 0;
            foreach (var dir in Get8Directions())
            {
                if (pathCells.Contains(cell + dir))
                    count++;
            }

            if (count == 1)
                return cell; 
        }

        return default; 
    }

    Vector3Int GetNextNeighbor(Vector3Int current, HashSet<Vector3Int> pathCells, HashSet<Vector3Int> visited)
    {
        foreach (var dir in Get8Directions())
        {
            Vector3Int neighbor = current + dir;
            if (pathCells.Contains(neighbor) && !visited.Contains(neighbor))
                return neighbor;
        }

        return Vector3Int.zero;
    }

    Vector3Int[] Get8Directions()
    {
        return new Vector3Int[]
        {
            new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
            new Vector3Int(1, 1, 0), new Vector3Int(-1, 1, 0),
            new Vector3Int(1, -1, 0), new Vector3Int(-1, -1, 0)
        };
    }

    void OnDrawGizmosSelected()
    {
        if (orderedPath == null || orderedPath.Count < 2)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < orderedPath.Count - 1; i++)
        {
            Gizmos.DrawSphere(orderedPath[i], 0.1f);
            Gizmos.DrawLine(orderedPath[i], orderedPath[i + 1]);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(orderedPath[0], 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(orderedPath[orderedPath.Count - 1], 0.15f);
    }
}
