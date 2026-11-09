using UnityEngine;
using System.Collections.Generic;

public class InteractablesSpawner : MonoBehaviour
{
    public InteractableData[] obstaclePrefabs; // these arrays have the details for the prefabs to be spawned
    public InteractableData[] collectablePrefabs; // these arrays have the details for the prefabs to be spawned
    public InteractableData[] powerUpPrefabs; // these arrays have the details for the prefabs to be spawned

    private PooledInteractableFactory interactableFactory;

    public float spawnInterval = 3f;
    private float timeSinceLastSpawn = 0f;

    public float difficultyIncreaseTime = 5f;
    public float minSpawnInterval = 0.5f;
    public float spawnSpeedIncrease = 0.5f;

    public float moveSpeed = 5f;
    public float spawnX = 10f;
    private float offScreenX = -30f;

    [SerializeField] private float powerUpCooldown;
    private float timeSinceLastPowerUp = 0f;
    
    private List<GameObject> activeSpawnedObjects = new List<GameObject>();

    void Start()
    {
        interactableFactory = new PooledInteractableFactory(
            GetPrefabsFromData(obstaclePrefabs),
            GetPrefabsFromData(collectablePrefabs),
            GetPrefabsFromData(powerUpPrefabs),
            this.transform
        );
    }

    void Update()
    {
        timeSinceLastPowerUp += Time.deltaTime;
        
        // Move all active objects
        for (int i = activeSpawnedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = activeSpawnedObjects[i];
            if (!obj.activeSelf) continue;

            obj.transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            // Off-screen check
            if (obj.transform.position.x < offScreenX)
            {
                ReturnObjectToPool(obj);
            }
        }

        // Handle spawning
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnObject();
            timeSinceLastSpawn = 0f;
        }
    }

    private void SpawnObject()
    {
        GameObject obj = null;
        InteractableData selectedData = null;

        // Weighted random selection
        float randomValue = Random.value;

        bool canSpawnPowerUp = timeSinceLastPowerUp >= powerUpCooldown;
        
        if (randomValue < 0.5f) // 50% chance for obstacles
        {
            if (obstaclePrefabs.Length > 0)
            {
                selectedData = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                obj = interactableFactory.CreateFromPrefab(selectedData.prefab);
            }
        }
        else if (randomValue < 0.85f || !canSpawnPowerUp) // 35% chance for collectables
        {
            if (collectablePrefabs.Length > 0)
            {
                selectedData = collectablePrefabs[Random.Range(0, collectablePrefabs.Length)];
                obj = interactableFactory.CreateFromPrefab(selectedData.prefab);
            }
        }
        else // 15% chance for power ups
        {
            if (powerUpPrefabs.Length > 0)
            {
                selectedData = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
                obj = interactableFactory.CreateFromPrefab(selectedData.prefab);
            }
            
            timeSinceLastPowerUp = 0f;
        }

        if (obj != null && selectedData != null)
        {
            obj.transform.position = new Vector3(spawnX, selectedData.spawnY, 0);
            obj.SetActive(true);
            activeSpawnedObjects.Add(obj);
        }
    }

    private void ReturnObjectToPool(GameObject obj)
    {
        if (activeSpawnedObjects.Contains(obj))
            activeSpawnedObjects.Remove(obj);

        interactableFactory.ReturnToPool(obj);
    }

    private void IncreaseDifficulty()
    {
        moveSpeed += spawnSpeedIncrease;
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - 0.2f);
    }

    private GameObject[] GetPrefabsFromData(InteractableData[] dataArray)
    {
        GameObject[] result = new GameObject[dataArray.Length];
        for (int i = 0; i < dataArray.Length; i++)
            result[i] = dataArray[i].prefab;
        return result;
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(IncreaseDifficulty), difficultyIncreaseTime, difficultyIncreaseTime);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(IncreaseDifficulty));

        var objectsToReturn = new List<GameObject>(activeSpawnedObjects);
        foreach (var obj in objectsToReturn)
        {
            if (obj != null && obj.activeSelf)
                ReturnObjectToPool(obj);
        }

        activeSpawnedObjects.Clear();
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}