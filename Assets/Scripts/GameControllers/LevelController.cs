using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;
    public float progress;
    private int numberOfTasks;
    private int tasksCompleted;
    private float taskProgress;
    public bool isLoaded = false;
    public delegate void OnPlayerSpawned();
    public static OnPlayerSpawned PlayerSpawned;

    [SerializeField] private Vector2 levelEnemyCountRange = Vector2.zero;
    [SerializeField] private Vector2 levelItemCount = Vector2.zero;

    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPoint;

    private ItemSpawner itemSpawner;
    private EnemySpawner enemySpawner;
    private LevelMission mission;

    private void Start()
    {
        instance = this;
        isLoaded = false;

        progress = 0;
        numberOfTasks = 5;
        tasksCompleted = 0;
        taskProgress = 0;

        itemSpawner = GetComponent<ItemSpawner>();
        enemySpawner = GetComponent<EnemySpawner>();
        mission = GetComponent<LevelMission>();

        if (levelEnemyCountRange.x <= 0 || levelEnemyCountRange.y <= 0 || levelEnemyCountRange.x > levelEnemyCountRange.y)
            levelEnemyCountRange = Vector2.zero;

        if (levelItemCount == Vector2.zero || levelItemCount.x <= 0 || levelItemCount.y <= 0)
        {
            levelItemCount = new Vector2(10, 20);
        }

        if (!spawnPoint)
            spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;

        Cursor.lockState = CursorLockMode.Locked;

        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;

        StartCoroutine(InitializeLevel());
    }

    private IEnumerator InitializeLevel()
    {
        //Handle locked doors, and spawn keys if necessary
        HandleLockedDoors();

        yield return null;

        //Setup Mission
        mission.initComplete = false;
        StartCoroutine(mission.InitializeMission());

        taskProgress = 0;
        
        while(!mission.initComplete)
        {
            taskProgress = mission.totalInitProgress / 100;
            taskProgress = taskProgress * (1.0f / (float)numberOfTasks);
            progress = (((float)tasksCompleted / (float)numberOfTasks) + taskProgress);

            yield return null;
        }

        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;

        yield return null;

        //Spawn items
        if (levelItemCount != Vector2.zero)
        {
            itemSpawner.spawnComplete = false;

            itemSpawner.SpawnItems((int)levelItemCount.x, (int)levelItemCount.y);

            taskProgress = 0;
            while (!itemSpawner.spawnComplete)
            {
                taskProgress = itemSpawner.progress;
                
                taskProgress = taskProgress * (1.0f / (float)numberOfTasks);
                progress = (((float)tasksCompleted / (float)numberOfTasks) + taskProgress);

                yield return null;
            }
        }

        //Destroy(itemSpawner);

        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;

        yield return null;

        //Spawn Enemies

        if (levelEnemyCountRange != Vector2.zero)
        {
            enemySpawner.spawnComplete = false;
            taskProgress = 0;

            enemySpawner.SetEnemySpawnBounds(Mathf.RoundToInt(levelEnemyCountRange.x), Mathf.RoundToInt(levelEnemyCountRange.y));
            enemySpawner.SpawnEnemies();

            while(!enemySpawner.spawnComplete)
            {
                taskProgress = enemySpawner.spawnProgress;

                taskProgress = taskProgress * (1.0f / (float)numberOfTasks);
                progress = (((float)tasksCompleted / (float)numberOfTasks) + taskProgress);

                yield return null;
            }
        }

        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;

        yield return null;

        //Spawn player
        Instantiate(player, spawnPoint.position, spawnPoint.rotation);

        //Call delegate function so objects can find reference to player
        if (PlayerSpawned != null)
        {
            PlayerSpawned();
        }

        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;

        yield return new WaitForSeconds(0.5f);

        isLoaded = true;
    }

    private int GetNumLockedDoors()
    {
        int lockedDoors = 0;

        return lockedDoors;
    }

    private void HandleLockedDoors()
    {
        taskProgress = 0;

        int numLocks = 0;
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        LinkedList<Door> lockedDoors = new LinkedList<Door>();

        foreach (GameObject go in doors)
        {
            Door door = go.GetComponent<Door>();

            if (door.isLocked)
            {
                lockedDoors.AddBack(door);
                numLocks++;
            }
        }

        //If there are no locked doors, don't run the rest
        if (numLocks <= 0)
            return;

        for (int i = 0; i < numLocks; i++)
        {
            //Spawn keys


            //Set the doors to the key needed


        }
    }
}
