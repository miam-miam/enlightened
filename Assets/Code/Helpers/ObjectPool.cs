using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public int maxPoolSize = 20;
    public GameObject spawnPrefab;
    private readonly List<GameObject> pool = new();
    private int currentIndex = 0;
    
    public void PoolInstantiate(Vector2 position, Quaternion rotation)
    {
        if (pool.Count < maxPoolSize)
        {
            pool.Add(Instantiate(spawnPrefab, position, rotation, transform));
        }
        else
        {
            var tr = pool[currentIndex].transform;
            
            tr.position = new Vector3(position.x, position.y, tr.position.z);
            tr.rotation = rotation;

            currentIndex = (currentIndex + 1) % maxPoolSize;
        }
    }
}
