using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [HideInInspector] public bool spawnComplete = false;
    [HideInInspector] public float spawnProgress = 0;
    [HideInInspector] public LinkedList<SpawnArea> areas = new LinkedList<SpawnArea>();

    [Range(0, 99)]
    [SerializeField] private float spawnHeavyProbability = 15;
    [Range(0, 99)]
    [SerializeField] private float spawnRangedProbability = 25;
    
    [SerializeField] private GameObject LightMonsters;
    [SerializeField] private GameObject RangedMonsters;
    [SerializeField] private GameObject HeavyMonsters;

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
    }

    public void SpawnEnemies()
    {
        spawnComplete = false;
        spawnProgress = 0;

        int x = Random.Range(minEnemies, maxEnemies);

        for (int i = 0; i < x; i++)
        {
            int areaIndex = GetAreaIndex();
            
            Vector3 point = areas.GetAtIndex(areaIndex).GetRandomPoint();

            if (point == Vector3.zero)
            {
                Debug.Log("Can't place enemy, too many obstructions");

                break;
            }

            float rot = Random.Range(0, 360);
            Quaternion rotation = Quaternion.Euler(0, rot, 0);

            GameObject monsterType = LightMonsters;
            int monsterTypeIndex = GetMonsterToSpawn();

            switch (monsterTypeIndex)
            {
                case 1:
                    //Small
                    monsterType = LightMonsters;
                    break;
                case 2:
                    //Ranged
                    monsterType = RangedMonsters;
                    break;
                case 3:
                    //Heavy
                    monsterType = HeavyMonsters;
                    break;
            }

            GameObject monster = Instantiate(monsterType, point, rotation);
            Enemy enemy = monster.GetComponent<Enemy>();
            enemy.enemySpawnIndex = 0;
            enemy.enemyTypeIndex = monsterTypeIndex;

            areas.GetAtIndex(areaIndex).AddedEnemy();

            spawnProgress = i / x;
        }

        spawnComplete = true;
    }

    private int GetAreaIndex()
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
                    return -1;
                }

                if (areas.GetAtIndex(areaIndex).numEnemies >= areas.GetAtIndex(areaIndex).maxEnemies)
                {
                    areas.RemoveAtIndex(areaIndex);

                    continue;
                }

                validArea = true;

                return areaIndex;

            } while (!validArea);

        }
        
        return -1;
    }

    public void LoadEnemiesFromData(List<LoadSaveManager.GameSaveData.EnemyData> enemyData)
    {
        if (enemyData.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < enemyData.Count; ++i)
        {
            //Get enemy location and position
            Vector3 pos = Vector3.zero;
            pos.x = enemyData[i].transformData.position.x;
            pos.y = enemyData[i].transformData.position.y;
            pos.z = enemyData[i].transformData.position.z;
            
            Vector3 rot = Vector3.zero;
            rot.x = enemyData[i].transformData.rotation.x;
            rot.y = enemyData[i].transformData.rotation.y;
            rot.z = enemyData[i].transformData.rotation.z;

            int enemyType = enemyData[i].enemyTypeIndex;
            int enemyIndex = enemyData[i].enemyIndex;

            GameObject monsterType = LightMonsters;
            
            switch (enemyType)
            {
                case 1:
                    //Small
                    monsterType = LightMonsters;
                    break;
                case 2:
                    //Ranged
                    monsterType = RangedMonsters;
                    break;
                case 3:
                    //Heavy
                    monsterType = HeavyMonsters;
                    break;
            }
            
            //Instatiate enemy and apply the saved data
            GameObject go = Instantiate(monsterType, pos, Quaternion.Euler(rot));
            EnemyStats stats = go.GetComponent<EnemyStats>();
            stats.SetHealth(enemyData[i].health);
        }
    }

    private int GetMonsterToSpawn()
    {
        float monsterSpawnRand = Random.Range(0, 100);

        if (monsterSpawnRand <= spawnHeavyProbability)
        {
            return 3;
        }

        if (monsterSpawnRand <= spawnRangedProbability)
        {
            return 2;
        }
        
        return 1;
    }

    public void SetEnemySpawnBounds(int min, int max)
    {
        minEnemies = min;
        maxEnemies = max;
    }
}
