using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private GameObject[] itemSpawnPoints;
    private LinkedList<GameObject> availableSpawnPoints = new LinkedList<GameObject>();

    [SerializeField] private GameObject[] itemObjects;
    [SerializeField] private InventoryItem[] items;

    private void Start()
    {
        itemSpawnPoints = GameObject.FindGameObjectsWithTag("ItemSpawn");

        if (itemSpawnPoints.Length > 0)
        {
            foreach (GameObject go in itemSpawnPoints)
            {
                availableSpawnPoints.AddBack(go);
            }
        }
    }

    private void OnDestroy()
    {
        availableSpawnPoints.Clear();

        /*if (itemSpawnPoints.Length > 0)
        {
            foreach (GameObject go in itemSpawnPoints)
            {
                Destroy(go);
            }
        }*/
    }

    public void SpawnItems(int min, int max)
    {
        if (availableSpawnPoints.size <= 0 || itemObjects.Length <= 0)
            return;

        int minItems, maxItems;

        minItems = min < availableSpawnPoints.size ? min : availableSpawnPoints.size;
        maxItems = max < availableSpawnPoints.size ? max : availableSpawnPoints.size;

        int itemCount = Random.Range(minItems, maxItems);

        for (int i = 0; i < itemCount; i++)
        {
            int spIndex = Random.Range(0, availableSpawnPoints.size - 1);
            int iIndex = Random.Range(0, itemObjects.Length - 1);

            //Spawn object at availableSpawnPoints[spIndex]
            Transform spawnPoint = availableSpawnPoints.GetAtIndex(spIndex).transform;
            GameObject go = Instantiate(itemObjects[iIndex], spawnPoint.position, spawnPoint.rotation);
            
            if (items.Length > 0)
            {
                ItemBox box = go.GetComponent<ItemBox>();

                int itemIndex = Random.Range(0, items.Length);
                box.item = items[itemIndex];

            }

            //Remove the spawnpoint from the available spawn points so it can't be used again
            availableSpawnPoints.RemoveAtIndex(spIndex);
        }
    }
}
