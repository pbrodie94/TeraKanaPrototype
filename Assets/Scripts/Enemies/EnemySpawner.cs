using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool spawnComplete = false;
    public float spawnProgress = 0;
    public LinkedList<SpawnArea> areas = new LinkedList<SpawnArea>();

    public GameObject Monster;

    private int minEnemies = 10;
    private int maxEnemies = 20;

    private void Start()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("SpawnArea");

        if (go.Length != 0 || go != null)
        {
            foreach (GameObject g in go)
            {
                SpawnArea sa = g.GetComponent<SpawnArea>();

                if (sa != null)
                {
                    areas.AddBack(sa);
                }
            }
        }

        //SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        spawnComplete = false;
        spawnProgress = 0;

        int x = Random.Range(minEnemies, maxEnemies);

        for (int i = 0; i < x; i++)
        {
            int areaIndex = 0;

            if (areas.size > 1)
            {
                bool validArea = false;

                do
                {
                    areaIndex = Random.Range(0, areas.size - 1);

                    if (areas.size <= 0)
                    {
                        areaIndex = -1;
                        return;
                    }

                    if (areas.GetAtIndex(areaIndex).numEnemies >= areas.GetAtIndex(areaIndex).maxEnemies)
                    {
                        areas.RemoveAtIndex(areaIndex);

                        continue;
                    }

                    validArea = true;

                } while (!validArea);

            }

            if (areaIndex < 0)
                return;

            Vector3 point = areas.GetAtIndex(areaIndex).GetRandomPoint();

            if (point == Vector3.zero)
            {
                Debug.Log("Can't place enemy, too many obstructions");

                break;
            }

            float rot = Random.Range(0, 360);
            Quaternion rotation = Quaternion.Euler(0, rot, 0);

            Instantiate(Monster, point, rotation);

            areas.GetAtIndex(areaIndex).AddedEnemy();

            spawnProgress = i / x;
        }

        spawnComplete = true;
    }

    public void SetEnemySpawnBounds(int min, int max)
    {
        minEnemies = min;
        maxEnemies = max;
    }
}
