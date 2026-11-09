// Mirza Wilayat Ali (1034162)
// Object Pool Script

using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    // The prefab to be reused (copied) in the pool
    private GameObject prefab;

    // Queue to store all the pooled (inactive) objects
    private Queue<GameObject> pool = new Queue<GameObject>();

    // Parent transform to keep pooled objects organized in the hierarchy
    private Transform parent;

    // Public property to get the prefab being used in this pool
    public GameObject Prefab
    {
        get { return prefab; }
    }

    public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        // Create the initial set of objects and keep them inactive
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent); // create object
            obj.SetActive(false); // hide it
            pool.Enqueue(obj); // add it to the pool
        }
    }

    // Get an object from the pool
    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            // If there's an available object, use it
            GameObject obj = pool.Dequeue();
            obj.SetActive(true); // make it visible/usable
            return obj;
        }

        // If no object is available, create a new one
        return GameObject.Instantiate(prefab, parent);
    }

    // Return an object back to the pool
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false); // hide it again
        pool.Enqueue(obj); // put it back in the queue
    }
}
