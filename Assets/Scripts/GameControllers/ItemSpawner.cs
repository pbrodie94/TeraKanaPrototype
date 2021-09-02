using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public bool spawnComplete = false;
    public float progress;
    private GameObject[] itemSpawnPoints;
    private LinkedList<GameObject> availableSpawnPoints = new LinkedList<GameObject>();

    [SerializeField] private GameObject[] itemObjects;
    [SerializeField] private InventoryItem key;
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

    public void SpawnKeys(LinkedList<Door> lockedDoors)
    {
        if (lockedDoors.size <= 0)
        {
            return;
        }

        List<int> usedKeyNumbers = new List<int>();

        for (int i = 0; i < lockedDoors.size; ++i)
        {
            //Get the spawn point that key will be spawned at
            int spawnPointIndex = Random.Range(0, availableSpawnPoints.size - 1);
            int iIndex = Random.Range(0, itemObjects.Length - 1);
            Transform spawnPoint = itemSpawnPoints[spawnPointIndex].transform;
            
            //Spawn the box
            GameObject box = Instantiate(itemObjects[iIndex], spawnPoint.position, spawnPoint.rotation);
            ItemBox itemBox = box.GetComponent<ItemBox>();
            
            //Create the key and add it to the box
            InteractionItem keyInstance = Instantiate(key.gameObject).GetComponent<InteractionItem>();
            
            //Get unique key number
            int keyNumber;

            do
            {
                keyNumber = Random.Range(100, 999);

                if (usedKeyNumbers.Count > 0)
                {
                    break;
                }

            } while (usedKeyNumbers.Contains(keyNumber));
            
            //Add the key number to a list so it's not used again
            usedKeyNumbers.Add(keyNumber);

            keyInstance.itemName = "Key " + keyNumber.ToString();
            itemBox.item = keyInstance;

            //Pass the reference of the key to the particular door
            lockedDoors.GetAtIndex(i).SetUnlockKey(keyInstance);
            
            //Remove item spawn point from index
            availableSpawnPoints.RemoveAtIndex(spawnPointIndex);
        }
    }

    public void SpawnItems(int min, int max)
    {
        progress = 0;
        spawnComplete = false;

        if (availableSpawnPoints.size <= 0 || itemObjects.Length <= 0)
        {
            spawnComplete = true;
            return;
        }

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

            progress = i / itemCount;
        }

        spawnComplete = true;
    }

    public void LoadItemData(List<LoadSaveManager.GameSaveData.ItemBoxData> itemBoxData)
    {
        //If no saved items, return
        if (itemBoxData.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < itemBoxData.Count; ++i)
        {
            //Get position and rotation the box
            Vector3 pos = Vector3.zero;
            pos.x = itemBoxData[i].transformData.position.x;
            pos.y = itemBoxData[i].transformData.position.y;
            pos.z = itemBoxData[i].transformData.position.z;
            
            Vector3 rot = Vector3.zero;
            rot.x = itemBoxData[i].transformData.rotation.x;
            rot.y = itemBoxData[i].transformData.rotation.y;
            rot.z = itemBoxData[i].transformData.rotation.z;
            
            //Spawn the item, and set necessary values
            int iIndex = Random.Range(0, itemObjects.Length - 1);
            GameObject go = Instantiate(itemObjects[iIndex], pos, Quaternion.Euler(rot));
            ItemBox box = go.GetComponent<ItemBox>();
            //box.item = itemBoxData[i].item;
        }
    }
}
