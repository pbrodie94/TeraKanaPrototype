using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<SpawnArea> areas = new List<SpawnArea>();

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
                    areas.Add(sa);
                }
            }
        }

        //SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        int x = Random.Range(minEnemies, maxEnemies);

        for (int i = 0; i < x; i++)
        {
            int areaIndex = 0;

            if (areas.Count > 1)
            {
                bool validArea = false;

                do
                {
                    areaIndex = Random.Range(0, areas.Count - 1);

                    if (areas.Count <= 0)
                    {
                        areaIndex = -1;
                        return;
                    }

                    if (areas[areaIndex].numEnemies >= areas[areaIndex].maxEnemies)
                    {
                        areas.RemoveAt(areaIndex);

                        continue;
                    }

                    validArea = true;

                } while (!validArea);

            }

            if (areaIndex < 0)
                return;

            Vector3 point = areas[areaIndex].GetRandomPoint();

            if (point == Vector3.zero)
            {
                Debug.Log("Can't place enemy, too many obstructions");

                break;
            }

            float rot = Random.Range(0, 360);
            Quaternion rotation = Quaternion.Euler(0, rot, 0);

            Instantiate(Monster, point, rotation);

            areas[areaIndex].AddedEnemy();
        }
    }

    public void SetEnemySpawnBounds(int min, int max)
    {
        minEnemies = min;
        maxEnemies = max;
    }
}
