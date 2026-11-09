using System.Collections.Generic;
using UnityEngine;

// Factory Interface
// Factory Interface
public interface IInteractableFactory
{
    GameObject CreateObstacle(GameObject specificPrefab = null);
    GameObject CreateCollectable(GameObject specificPrefab = null);
    GameObject CreatePowerUp(GameObject specificPrefab = null);
    GameObject CreateFromPrefab(GameObject prefab); // New method for specific prefabs
}

// Pool-based Factory
public class PooledInteractableFactory : IInteractableFactory
{
    private Dictionary<GameObject, ObjectPool> prefabToPool = new();
    private Transform parent;

    // Keep prefab groups for random selection (for backward compatibility)
    private GameObject[] obstaclePrefabs;
    private GameObject[] collectablePrefabs;
    private GameObject[] powerUpsPrefabs;

    public PooledInteractableFactory(
        GameObject[] obstacles,
        GameObject[] collectables,
        GameObject[] powerUps,
        Transform poolParent = null)
    {
        parent = poolParent;

        // Store arrays for later random picks
        obstaclePrefabs = obstacles;
        collectablePrefabs = collectables;
        powerUpsPrefabs = powerUps;

        // Initialize pools
        InitPools(obstacles, 2);
        InitPools(collectables, 2);
        InitPools(powerUps, 2);
    }

    private void InitPools(GameObject[] prefabs, int initialSize)
    {
        foreach (var prefab in prefabs)
        {
            if (prefab != null && !prefabToPool.ContainsKey(prefab))
                prefabToPool[prefab] = new ObjectPool(prefab, initialSize, parent);
        }
    }

    // For random selection (backward compatibility)
    public GameObject CreateObstacle() => GetRandomFromGroup(obstaclePrefabs);
    public GameObject CreateCollectable() => GetRandomFromGroup(collectablePrefabs);
    public GameObject CreatePowerUp() => GetRandomFromGroup(powerUpsPrefabs);

    // For specific prefab selection
    public GameObject CreateObstacle(GameObject specificPrefab) => CreateFromPrefab(specificPrefab);
    public GameObject CreateCollectable(GameObject specificPrefab) => CreateFromPrefab(specificPrefab);
    public GameObject CreatePowerUp(GameObject specificPrefab) => CreateFromPrefab(specificPrefab);

    public GameObject CreateFromPrefab(GameObject prefab)
    {
        if (prefab == null || !prefabToPool.ContainsKey(prefab)) return null;

        GameObject obj = prefabToPool[prefab].GetObject();

        // Ensure the PooledObject component exists
        var pooledObj = obj.GetComponent<PooledObject>();
        if (pooledObj == null)
        {
            pooledObj = obj.AddComponent<PooledObject>();
        }
        pooledObj.OriginalPrefab = prefab;

        return obj;
    }

    private GameObject GetRandomFromGroup(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) return null;

        var prefab = prefabs[Random.Range(0, prefabs.Length)];
        return CreateFromPrefab(prefab);
    }

    public void ReturnToPool(GameObject obj)
    {
        var pooledObj = obj.GetComponent<PooledObject>();
        if (pooledObj != null && pooledObj.OriginalPrefab != null &&
            prefabToPool.TryGetValue(pooledObj.OriginalPrefab, out var pool))
        {
            pool.ReturnObject(obj);
        }
        else
        {
            GameObject.Destroy(obj);
        }
    }
}

// Holds reference to original prefab
public class PooledObject : MonoBehaviour
{
    public GameObject OriginalPrefab { get; set; }
}