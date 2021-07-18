using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public delegate void OnPlayerSpawned();
    public static OnPlayerSpawned PlayerSpawned;

    [SerializeField] private Vector2 levelEnemyCountRange = Vector2.zero;
    [SerializeField] private Vector2 levelItemCount = Vector2.zero;

    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPoint;

    private HUDManager hud;
    private ItemSpawner itemSpawner;
    private EnemySpawner enemySpawner;
    private LevelMission mission;

    private void Start()
    {
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();
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

        StartCoroutine(InitializeLevel());
    }

    private IEnumerator InitializeLevel()
    {
        //Handle locked doors, and spawn keys if necessary
        HandleLockedDoors();

        yield return null;

        //Setup Mission
        Debug.Log("Initializing mission");
        mission.InitializeMission(this, hud);
        yield return null;

        //Spawn items
        

        Debug.Log("Spawning items.");
        if (levelItemCount != Vector2.zero)
        {
            itemSpawner.SpawnItems((int)levelItemCount.x, (int)levelItemCount.y);
        }

        //Destroy(itemSpawner);

        yield return null;

        //Spawn Enemies
        Debug.Log("Spawning enemies");
        if (levelEnemyCountRange != Vector2.zero)
        {
            enemySpawner.SetEnemySpawnBounds(Mathf.RoundToInt(levelEnemyCountRange.x), Mathf.RoundToInt(levelEnemyCountRange.y));
            enemySpawner.SpawnEnemies();
        }

        yield return null;

        //Spawn player
        Debug.Log("Spawning player.");
        Instantiate(player, spawnPoint.position, spawnPoint.rotation);

        //Call delegate function so objects can find reference to player
        if (PlayerSpawned != null)
        {
            PlayerSpawned();
        }
    }

    private int GetNumLockedDoors()
    {
        int lockedDoors = 0;

        return lockedDoors;
    }

    private void HandleLockedDoors()
    {
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
