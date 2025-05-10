using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathManager : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase pathTile;
    public List<Vector3Int> startTiles = new List<Vector3Int>();

    

    private Dictionary<string, List<Vector3>> allPaths = new Dictionary<string, List<Vector3>>();

    void Awake()
    {
        GeneratePathsFromDefinedStarts();
    }



    public List<Vector3> GetPath(string key)
    {
        return allPaths.ContainsKey(key) ? allPaths[key] : null;
    }

    public List<string> GetAllPathKeys()
    {
        return new List<string>(allPaths.Keys);
    }

    void GeneratePathsFromDefinedStarts()
    {
        HashSet<Vector3Int> pathTiles = new HashSet<Vector3Int>();
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(cell) == pathTile)
                    pathTiles.Add(cell);
            }
        }

        List<Vector3Int> endTiles = new List<Vector3Int>();
        foreach (var tile in pathTiles)
        {
            if (startTiles.Contains(tile)) continue;

            int neighborCount = 0;
            foreach (var dir in Get8Directions())
            {
                if (pathTiles.Contains(tile + dir))
                    neighborCount++;
            }

            if (neighborCount == 1)
                endTiles.Add(tile);
        }

        int pathIndex = 0;
        HashSet<string> seenPaths = new HashSet<string>();

        foreach (var start in startTiles)
        {
            foreach (var end in endTiles)
            {
                List<Vector3Int> path = FindShortestPath(start, end, pathTiles);
                if (path != null && path.Count > 1)
                {
                    string pathHash = string.Join("-", path);
                    if (seenPaths.Contains(pathHash)) continue;

                    seenPaths.Add(pathHash);

                    List<Vector3> worldPath = new List<Vector3>();
                    foreach (var p in path)
                        worldPath.Add(tilemap.GetCellCenterWorld(p));

                    allPaths.Add($"Path_{pathIndex++}", worldPath);
                }
            }
        }
    }

    List<Vector3Int> FindShortestPath(Vector3Int start, Vector3Int end, HashSet<Vector3Int> validTiles)
    {
        Queue<List<Vector3Int>> queue = new Queue<List<Vector3Int>>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(new List<Vector3Int> { start });
        visited.Add(start);

        while (queue.Count > 0)
        {
            List<Vector3Int> currentPath = queue.Dequeue();
            Vector3Int current = currentPath[currentPath.Count - 1];

            if (current == end)
                return currentPath;

            foreach (var dir in Get8Directions())
            {
                Vector3Int neighbor = current + dir;
                if (validTiles.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    var newPath = new List<Vector3Int>(currentPath) { neighbor };
                    queue.Enqueue(newPath);
                }
            }
        }

        return null;
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
        if (allPaths == null) return;

#if UNITY_EDITOR
        UnityEditor.Handles.BeginGUI();
#endif

        int index = 0;

        foreach (var kvp in allPaths)
        {
            List<Vector3> path = kvp.Value;
            if (path.Count < 2) continue;

            Color pathColor = Color.HSVToRGB((index * 0.15f) % 1f, 1f, 1f);

            Vector3 offset = new Vector3(0, 0, -5f * index); 

            Gizmos.color = pathColor;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawSphere(path[i] + offset, 0.05f);
                Gizmos.DrawLine(path[i] + offset, path[i + 1] + offset);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(path[0] + offset, 0.1f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(path[path.Count - 1] + offset, 0.1f);

#if UNITY_EDITOR
            GUI.color = pathColor;
            GUI.Label(new Rect(10, 40 + (index * 18), 250, 20), $"Path {index}: {kvp.Key}");
#endif

            index++;
        }

#if UNITY_EDITOR
        UnityEditor.Handles.EndGUI();
#endif
    }
}
#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(PathManager))]
public class PathManagerEditor : UnityEditor.Editor
{
    void OnSceneGUI()
    {
        PathManager manager = (PathManager)target;
        if (manager.tilemap == null) return;

        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
            Vector3Int cell = manager.tilemap.WorldToCell(mousePos);

            if (manager.tilemap.GetTile(cell) == manager.pathTile)
            {
                if (manager.startTiles.Contains(cell))
                    manager.startTiles.Remove(cell);
                else
                    manager.startTiles.Add(cell);

                UnityEditor.EditorUtility.SetDirty(manager);
                e.Use();
            }
        }

        foreach (var tile in manager.startTiles)
        {
            Vector3 pos = manager.tilemap.GetCellCenterWorld(tile);
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawSolidDisc(pos, Vector3.forward, 0.15f);
        }
    }
}
#endif