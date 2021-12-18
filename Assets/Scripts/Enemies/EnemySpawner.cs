using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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

    [Header("Alarm Sequence")] 
    [SerializeField] private float timeBetweenAlarmWaves = 60;
    [SerializeField] private Vector2 alarmWaveSpawnRange = new Vector2(10, 15);
    [SerializeField] private SpawnArea[] testAlarmSpawnAreas;
    private float timeLastSpawnedWave;
    private bool alarmSequenceActivated = false;

    private void Awake()
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

    private void Start()
    {
        if (alarmWaveSpawnRange.x <= 0)
        {
            alarmWaveSpawnRange.x = 10;
        }

        if (alarmWaveSpawnRange.y <= 0)
        {
            alarmWaveSpawnRange.y = 15;
        }

        if (alarmWaveSpawnRange.y < alarmWaveSpawnRange.x)
        {
            alarmWaveSpawnRange.y = alarmWaveSpawnRange.x;
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
            int areaIndex = GetAreaIndex();
            
            Vector3 point = areas.GetAtIndex(areaIndex).GetRandomPoint();

            if (point == Vector3.zero)
            {
                Debug.Log("Can't place enemy, too many obstructions");

                break;
            }

            float rot = Random.Range(0, 360);
            Quaternion rotation = Quaternion.Euler(0, rot, 0);

            GameObject monster = GetMonsterToSpawn();
            
            Instantiate(monster, point, rotation);

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

    private GameObject GetMonsterToSpawn()
    {
        float monsterSpawnRand = Random.Range(0, 100);

        if (monsterSpawnRand <= spawnHeavyProbability)
        {
            return HeavyMonsters;
        }

        if (monsterSpawnRand <= spawnRangedProbability)
        {
            return RangedMonsters;
        }
        
        return LightMonsters;
    }

    public void SetEnemySpawnBounds(int min, int max)
    {
        minEnemies = min;
        maxEnemies = max;
    }

    public void BeginAlarmSequence()
    {
        alarmSequenceActivated = true;
        SpawnEnemyWave();
    }

    public void EndAlarmSequence()
    {
        alarmSequenceActivated = false;
    }

    private void Update()
    {
        if (alarmSequenceActivated)
        {
            if (Time.time >= timeLastSpawnedWave)
            {
                SpawnEnemyWave();
            }
        }
    }

    private void SpawnEnemyWave()
    {
        if (testAlarmSpawnAreas.Length <= 0)
        {
            alarmSequenceActivated = false;
            return;
        }
        
        HUDManager.instance.AddNotification("ENEMY WAVE SPAWNING!", Color.red);
        
        int enemiesToSpawn = Random.Range((int)alarmWaveSpawnRange.x, (int)alarmWaveSpawnRange.y);
        for (int i = 0; i < enemiesToSpawn; ++i)
        {
            SpawnArea area = testAlarmSpawnAreas[0];
            if (testAlarmSpawnAreas.Length > 1)
            {
                int index = Random.Range(0, testAlarmSpawnAreas.Length - 1);
                area = testAlarmSpawnAreas[index];
            }

            GameObject monster = GetMonsterToSpawn();
            Vector3 position = area.GetRandomPoint();
            float rot = Random.Range(0, 360);
            Quaternion rotation = Quaternion.Euler(0, rot, 0);

            GameObject monsterInstance = Instantiate(monster, position, rotation);
            Enemy enemy = monsterInstance.GetComponent<Enemy>();
            GameObject player = GameObject.FindWithTag("Player");
            enemy.InitializeWaveSpawn(player);
        }
        
        timeLastSpawnedWave = Time.time + timeBetweenAlarmWaves;
    }
}
