using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> prefabLookup = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

  
    public void RegisterPrefab(string tag, GameObject prefab)
    {
        if (!prefabLookup.ContainsKey(tag))
            prefabLookup[tag] = prefab;

        if (!poolDictionary.ContainsKey(tag))
            poolDictionary[tag] = new Queue<GameObject>();
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!prefabLookup.ContainsKey(tag))
        {
            Debug.LogWarning("Prefab with tag '" + tag + "' has not been registered.");
            return null;
        }

        if (!poolDictionary.ContainsKey(tag))
            poolDictionary[tag] = new Queue<GameObject>();

        GameObject obj;

        if (poolDictionary[tag].Count > 0)
        {
            obj = poolDictionary[tag].Dequeue();
        }
        else
        {
            obj = Instantiate(prefabLookup[tag], position, rotation);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }
}
