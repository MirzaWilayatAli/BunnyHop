using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class interactablesSpawnerTest
{
    private GameObject spawnerObj;
    private InteractablesSpawner spawner;
    private readonly List<Object> createdAssets = new List<Object>();

    [SetUp]
    public void SetUp()
    {
        Time.timeScale = 5f;
    }

    [TearDown]
    public void TearDown()
    {
        Time.timeScale = 1f;

        if (spawnerObj != null)
            Object.DestroyImmediate(spawnerObj);

        foreach (var asset in createdAssets)
        {
            if (asset != null)
                Object.DestroyImmediate(asset);
        }
        createdAssets.Clear();
    }

    [UnityTest]
    public IEnumerator Spawner_SpawnsInteractablesAtConfiguredPositionAndMovesThem()
    {
        var obstaclePrefab = CreateDummyPrefab("Obstacle");
        var collectablePrefab = CreateDummyPrefab("Collectable");
        var powerUpPrefab = CreateDummyPrefab("PowerUp");

        spawnerObj = new GameObject("TestSpawner");
        spawnerObj.SetActive(false);

        spawner = spawnerObj.AddComponent<InteractablesSpawner>();
        spawner.obstaclePrefabs = new[] { CreateData(obstaclePrefab, 1f) };
        spawner.collectablePrefabs = new[] { CreateData(collectablePrefab, 2f) };
        spawner.powerUpPrefabs = new[] { CreateData(powerUpPrefab, 3f) };
        spawner.spawnInterval = 0.5f;
        spawner.spawnX = 10f;
        spawner.moveSpeed = 5f;

        spawnerObj.SetActive(true);
        yield return null; // Let Start() create PooledInteractableFactory

        var spawned = new HashSet<GameObject>();
        float endTime = Time.time + 3f;

        while (Time.time < endTime)
        {
            for (int i = 0; i < spawner.transform.childCount; i++)
            {
                GameObject child = spawner.transform.GetChild(i).gameObject;
                if (!child.activeSelf || !spawned.Add(child)) continue;

                // Verify it spawned at spawnX and the matching spawnY
                Assert.AreEqual(spawner.spawnX, child.transform.position.x, 0.01f);

                if (child.name.StartsWith("Obstacle"))
                    Assert.AreEqual(1f, child.transform.position.y, 0.01f);
                else if (child.name.StartsWith("Collectable"))
                    Assert.AreEqual(2f, child.transform.position.y, 0.01f);
                else if (child.name.StartsWith("PowerUp"))
                    Assert.AreEqual(3f, child.transform.position.y, 0.01f);
                else
                    Assert.Fail($"Unexpected spawned object: {child.name}");
            }

            yield return null;
        }

        Assert.GreaterOrEqual(spawned.Count, 3, "Expected at least 3 interactables to spawn.");

        // Confirm spawned objects are moving left
        foreach (var obj in spawned)
        {
            if (obj != null && obj.activeSelf)
                Assert.Less(obj.transform.position.x, spawner.spawnX, $"{obj.name} did not move left.");
        }
    }

    private GameObject CreateDummyPrefab(string name)
    {
        var go = new GameObject(name);
        go.SetActive(false);
        createdAssets.Add(go);
        return go;
    }

    private InteractableData CreateData(GameObject prefab, float spawnY)
    {
        // If InteractableData is a plain [System.Serializable] class instead of ScriptableObject,
        // replace this with: var data = new InteractableData();
        var data = ScriptableObject.CreateInstance<InteractableData>();
        data.prefab = prefab;
        data.spawnY = spawnY;
        createdAssets.Add(data);
        return data;
    }
}