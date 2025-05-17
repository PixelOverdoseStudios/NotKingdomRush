using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class PathManager : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase pathTile;
    public List<Vector3Int> startTiles = new List<Vector3Int>();
    [SerializeField] private List<PathData> serializedPaths = new List<PathData>();
    [SerializeField] private bool turnDebugOn = true;

    private Dictionary<Vector3Int, TileBase> lastTilemapState = new Dictionary<Vector3Int, TileBase>();
    private List<Vector3Int> lastStartTiles = new List<Vector3Int>();
    private bool needsRegeneration = true;


    private Dictionary<string, List<Vector3>> allPaths = new Dictionary<string, List<Vector3>>();

    void Awake()
    {
        GeneratePathsFromDefinedStarts();
        CacheTilemapState();
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

        serializedPaths.Clear();
        foreach (var kvp in allPaths)
        {
            serializedPaths.Add(new PathData { key = kvp.Key, waypoints = kvp.Value });
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

    void CheckForChanges()
    {
        // Check if start tiles changed
        if (!startTiles.SequenceEqual(lastStartTiles))
        {
            lastStartTiles = new List<Vector3Int>(startTiles);
            needsRegeneration = true;
            return;
        }

        // Check if tilemap changed
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                TileBase currentTile = tilemap.GetTile(cell);

                if (lastTilemapState.TryGetValue(cell, out TileBase lastTile))
                {
                    if (currentTile != lastTile)
                    {
                        needsRegeneration = true;
                        CacheTilemapState();
                        return;
                    }
                }
                else if (currentTile != null)
                {
                    needsRegeneration = true;
                    CacheTilemapState();
                    return;
                }
            }
        }
    }

    void CacheTilemapState()
    {
        lastTilemapState.Clear();
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                lastTilemapState[cell] = tilemap.GetTile(cell);
            }
        }
    }

    void GeneratePathsIfNeeded()
    {
        if (!needsRegeneration) return;
        needsRegeneration = false;

        GeneratePathsFromDefinedStarts(); // Your existing method
    }

void OnDrawGizmosSelected()
{
    if (!turnDebugOn || allPaths == null || allPaths.Count == 0) 
        return;

#if UNITY_EDITOR
        // --- Clean up old debug objects (if any) ---
    if (Event.current.type == EventType.Repaint)
    {
        ClearDebugObjects();
    }

    // --- GUI Labels ---
    UnityEditor.Handles.BeginGUI();
    int index = 0;
    foreach (var kvp in allPaths)
    {
        List<Vector3> path = kvp.Value;
        if (path.Count < 2) continue;

        Color pathColor = Color.HSVToRGB((index * 0.15f) % 1f, 1f, 1f);
        GUI.color = pathColor;
        GUI.Label(new Rect(10, 40 + (index * 18), 250, 20), $"Path {index}: {kvp.Key}");
        index++;
    }
    UnityEditor.Handles.EndGUI();

    // --- Create Editor-Only GameObjects for paths ---
    index = 0;
    foreach (var kvp in allPaths)
    {
        List<Vector3> path = kvp.Value;
        if (path.Count < 2) continue;

        Color pathColor = Color.HSVToRGB((index * 0.15f) % 1f, 1f, 1f);
        Vector3 offset = new Vector3(0, 0, -5f * index);

        // Parent GameObject for this path
        GameObject pathParent = new GameObject($"Path_{index}_DEBUG");
        pathParent.transform.SetParent(this.transform);
        pathParent.tag = "EditorOnly";

        // Create a shared material for this path
        Material sharedMaterial = new Material(Shader.Find("Unlit/Color")) { color = pathColor };

        // Draw spheres and lines (with offset)
        for (int i = 0; i < path.Count; i++)
        {
            // Sphere for waypoint
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = path[i] + offset;
            sphere.transform.localScale = Vector3.one * 0.2f;
            sphere.GetComponent<Renderer>().sharedMaterial = sharedMaterial; // Fixed: Using sharedMaterial
            sphere.transform.SetParent(pathParent.transform);
            sphere.tag = "EditorOnly";

            // Line between waypoints
            if (i < path.Count - 1)
            {
                GameObject lineObj = new GameObject($"Line_{i}");
                lineObj.transform.SetParent(pathParent.transform);
                lineObj.tag = "EditorOnly";
                LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                lr.startWidth = lr.endWidth = 0.1f;
                lr.positionCount = 2;
                lr.SetPositions(new Vector3[] { path[i] + offset, path[i + 1] + offset });
                lr.sharedMaterial = sharedMaterial; // Fixed: Using sharedMaterial
            }
        }

        // Start/End markers (using shared materials)
        Material startMaterial = new Material(Shader.Find("Unlit/Color")) { color = Color.green };
        Material endMaterial = new Material(Shader.Find("Unlit/Color")) { color = Color.red };

        GameObject startSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        startSphere.transform.position = path[0] + offset;
        startSphere.transform.localScale = Vector3.one * 0.3f;
        startSphere.GetComponent<Renderer>().sharedMaterial = startMaterial;
        startSphere.transform.SetParent(pathParent.transform);
        startSphere.tag = "EditorOnly";

        GameObject endSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        endSphere.transform.position = path[path.Count - 1] + offset;
        endSphere.transform.localScale = Vector3.one * 0.3f;
        endSphere.GetComponent<Renderer>().sharedMaterial = endMaterial;
        endSphere.transform.SetParent(pathParent.transform);
        endSphere.tag = "EditorOnly";

        index++;
    }
#endif
}

// Helper to clean up debug objects
void ClearDebugObjects()
{
    #if UNITY_EDITOR
        var editorObjects = new List<GameObject>();
        foreach (Transform child in this.transform)
        {
            if (child.CompareTag("EditorOnly"))
                editorObjects.Add(child.gameObject);
        }

        // Destroy in reverse to avoid modification during iteration
        for (int i = editorObjects.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(editorObjects[i]);
        }
    #endif
}

#if UNITY_EDITOR
    void OnEnable()
    {
        UnityEditor.EditorApplication.update += OnEditorUpdate;
    }

    void OnDisable()
    {
        UnityEditor.EditorApplication.update -= OnEditorUpdate;
    }

    void OnEditorUpdate()
    {
        if (!Application.isPlaying && tilemap != null)
        {
            CheckForChanges();
            GeneratePathsIfNeeded();
        }
    }
#endif
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

[System.Serializable]
public class PathData
{
    public string key;
    public List<Vector3> waypoints;
}





