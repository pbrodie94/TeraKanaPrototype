using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;
    [HideInInspector] public float progress;
    private int numberOfTasks;
    private int tasksCompleted;
    private float taskProgress;
    [HideInInspector] public bool isLoaded = false;
    public delegate void OnPlayerSpawned();
    public static OnPlayerSpawned PlayerSpawned;

    [SerializeField] private Vector2 levelEnemyCountRange = Vector2.zero;
    [SerializeField] private Vector2 levelItemCount = Vector2.zero;

    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPoint;
    private Checkpoint currentCheckpoint;

    private ItemSpawner itemSpawner;
    private EnemySpawner enemySpawner;
    private LevelMission mission;

    private void Start()
    {
        instance = this;
        isLoaded = false;

        progress = 0;
        numberOfTasks = 6;
        tasksCompleted = 0;
        taskProgress = 0;

        itemSpawner = GetComponent<ItemSpawner>();
        enemySpawner = GetComponent<EnemySpawner>();
        mission = GetComponent<LevelMission>();

        if (!GameManager.instance.GetIsLoading())
        {
            if (levelEnemyCountRange.x <= 0 || levelEnemyCountRange.y <= 0 ||
                levelEnemyCountRange.x > levelEnemyCountRange.y)
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
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            StartCoroutine(LoadLevelFromData());
        }
    }

    private IEnumerator InitializeLevel()
    {
        //Handle locked doors, and spawn keys if necessary
        HandleLockedDoors();

        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;
        
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

    private IEnumerator LoadLevelFromData()
    {
        //Get saved data
        LoadSaveManager.GameSaveData gameData = GameManager.gameData.gameSaveData;
        
        //Find player's spawn point
        LoadPlayerSpawnLocation(gameData);

        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;

        yield return null;
        
        //Handle all doors
        
        LoadDoorData(gameData);
        
        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;
        
        yield return null;
        
        //Handle mission type and progress
        
        mission.LoadMissionData(gameData.missionData);
        
        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;
        
        yield return null;
        
        //Handle items
        
        itemSpawner.LoadItemData(gameData.itemBoxes);
        
        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;
        
        yield return null;
        
        //Place enemies at their positions
        enemySpawner.LoadEnemiesFromData(gameData.enemies);
        
        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;
        
        //Spawn player
        Instantiate(player, spawnPoint.position, spawnPoint.rotation);

        //Call delegate function so objects can find reference to player
        if (PlayerSpawned != null)
        {
            PlayerSpawned();
        }
        
        //Load player data
        LoadPlayerData(gameData.player);
        
        ++tasksCompleted;
        progress = (float)tasksCompleted / (float)numberOfTasks;
        
        yield return new WaitForSeconds(0.5f);

        isLoaded = true;
    }
    
    private void HandleLockedDoors()
    {
        taskProgress = 0;
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        LinkedList<Door> lockedDoors = new LinkedList<Door>();

        foreach (GameObject go in doors)
        {
            Door door = go.GetComponent<Door>();

            if (door.isLocked)
            {
                lockedDoors.AddBack(door);
            }
        }

        //If there are no locked doors, don't run the rest
        if (lockedDoors.size <= 0)
            return;

        itemSpawner.SpawnKeys(lockedDoors);
    }

    private void LoadPlayerSpawnLocation(LoadSaveManager.GameSaveData gameData)
    {
        //Retrieve the position and rotation information
        Vector3 pos = Vector3.zero;
        pos.x = gameData.player.spawnPoint.position.x;
        pos.y = gameData.player.spawnPoint.position.y;
        pos.z = gameData.player.spawnPoint.position.z;

        Vector3 rot = Vector3.zero;
        rot.x = gameData.player.spawnPoint.rotation.x;
        rot.y = gameData.player.spawnPoint.rotation.y;
        rot.z = gameData.player.spawnPoint.rotation.z;

        //Set spawn point to the saved spawn point
        spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
        spawnPoint.position = pos;
        spawnPoint.rotation = Quaternion.Euler(rot);
    }

    private void LoadDoorData(LoadSaveManager.GameSaveData gameData)
    {
        //Check if there is any data
        if (gameData.doors.Count <= 0)
        {
            //No door data, set doors like normal
            HandleLockedDoors();
            return;
        }
        
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach (GameObject go in doors)
        {
            //iterate through the saved door data 
            for (int i = 0; i < gameData.doors.Count; ++i)
            {
                if (go.GetInstanceID() == gameData.doors[i].ID)
                {
                    //Apply the door's saved data
                    Door door = go.GetComponent<Door>();
                    door.SetDoorData(gameData.doors[i].opened, gameData.doors[i].locked);
                    
                    break;
                }
            }
            
            Debug.LogError("Could not find door ID");
        }
    }

    private void LoadPlayerData(LoadSaveManager.GameSaveData.PlayerData playerData)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        //Load health
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.SetHealth(playerData.health);

        //Load weapons
        /*if (playerData.primaryWeapon)
        {
            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            Weapon primary = Instantiate(playerData.primaryWeapon.gameObject).GetComponent<Weapon>();
            weaponManager.EquipWeapon(primary, false);
            
            //Delete the weapons in the world

            if (playerData.secondaryWeapon)
            {
                Weapon secondary = Instantiate(playerData.secondaryWeapon).GetComponent<Weapon>();
                weaponManager.EquipWeapon(secondary, false);
            }
        }*/

        //Load inventory
        /*Inventory inventory = player.GetComponent<Inventory>();
        
        for (int i = 0; i < playerData.aidItems.Count; ++i)
        {
            inventory.LoadItems(playerData.aidItems[i]);
        }

        for (int i = 0; i < playerData.items.Count; ++i)
        {
            inventory.LoadItems(playerData.items[i]);
        }*/
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }

    public void UpdateCheckpoint(Checkpoint checkpoint)
    {
        //if there is currently a checkpoint, set it as not the active checkpoint
        if (currentCheckpoint)
        {
            currentCheckpoint.SetCurrentCheckpoint(false);
        }

        //Update the new checkpoint, and change the spawnpoint
        currentCheckpoint = checkpoint;
        currentCheckpoint.SetCurrentCheckpoint(true);
        spawnPoint = currentCheckpoint.gameObject.transform;

        HUDManager.instance.AddNotification("Checkpoint", Color.cyan);
        
        GameManager.instance.SaveGame();
    }
}
