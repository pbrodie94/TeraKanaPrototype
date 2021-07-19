using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool GlobalInstance;

    [SerializeField] private ObjectsToPool[] objectsToPool;

    private void Awake()
    {
        GlobalInstance = this;
    }

    public IEnumerator SpawnObjects()
    {
        if (objectsToPool.Length > 0)
        {
            for (int i = 0; i < objectsToPool.Length; ++i)
            {

            }
        }

        return null;
    }

    public GameObject GetObjectFromPool()
    {
        GameObject obj = new GameObject();

        return obj;
    }

    public void ReturnObjectToPool(GameObject obj)
    {

    }
}

[System.Serializable]
public struct ObjectsToPool
{
    public GameObject poolObject;
    public int defaultNumToPool;
}