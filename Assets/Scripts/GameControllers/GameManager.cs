using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    private List<AsyncOperation> scenesLoading;
    private int currentSceneIndex = 0;

    [SerializeField] private Slider progressBar;
    private float loadProgress;
    private float initProgress;
    private static GameManager _instance = null;
    public static GameManager instance {
        get {
            return _instance;
        }
    }

    private static LoadSaveManager _gameData = null;
    public static LoadSaveManager gameData
    {
        get
        {
            return _gameData;
        }
    }

    private static bool shouldLoad = false;
    private static bool saving = false;

    private bool paused = false;
    public static int lives = 3;

    void Awake()
    {
        //Setup singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        if (!_gameData)
        {
            _gameData = GetComponent<LoadSaveManager>();
        }

        DontDestroyOnLoad(this.gameObject);

        //If we have a reference to the loading screen, we loaded in on the loading screen
        if (loadingScreen)
        {
            //Load in the MainMenu to start
            SceneManager.LoadSceneAsync(LevelManager.MainMenu, LoadSceneMode.Additive);
            currentSceneIndex = LevelManager.MainMenu;

            scenesLoading = new List<AsyncOperation>();

            loadingScreen.SetActive(false);
        }
    }

    private void Start()
    {
        if (shouldLoad)
        {
            _gameData.LoadGame(Application.persistentDataPath + "/GameData.xml");

            shouldLoad = false;
        }
        
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
            paused = true;
        } else
        {
            Time.timeScale = 1;
            paused = false;
        }
    }

    public void LoadLevel(int level)
    {
        //If we have a reference to our loading screen, 
        //Load into the loading screen
        if (loadingScreen)
        {
            loadingScreen.SetActive(true);
            scenesLoading.Add(SceneManager.UnloadSceneAsync(currentSceneIndex));
            scenesLoading.Add(SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive));

            loadProgress = 0;
            initProgress = 0;
            progressBar.value = 0;

            StartCoroutine(GetTotalProgress());
            
            currentSceneIndex = level;

            return;
        }

        //Otherwise directly load the level
        SceneManager.LoadScene(level);
    }

    private IEnumerator GetSceneLoadingProgress()
    {
        //Iterate through scenes loading
        for (int i = 0; i < scenesLoading.Count; ++i)
        {
            //If the scene is not done loading, return null and wait
            while(!scenesLoading[i].isDone)
            {
                loadProgress = 0;

                foreach(AsyncOperation operation in scenesLoading)
                {
                    loadProgress += operation.progress;
                }

                loadProgress = (loadProgress / scenesLoading.Count) * 100.0f;
                

                yield return null;
            }
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentSceneIndex));

        loadProgress = 100;
        
        //All scenes have loaded
        scenesLoading.Clear();
        
    }

    private IEnumerator GetTotalProgress()
    {
        StartCoroutine(GetSceneLoadingProgress());
        
        float totalProgress = 0;

        while(LevelController.instance == null || !LevelController.instance.isLoaded)
        {
            if (LevelController.instance == null)
            {
                initProgress = 0;
            } else {
                initProgress = LevelController.instance.progress * 100;
            }

            totalProgress = (loadProgress + initProgress) / 2.0f;

            progressBar.value = totalProgress;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        loadingScreen.SetActive(false);
        shouldLoad = false;
    }

    public void SaveGame()
    {
        if (!saving)
        {
            StartCoroutine(ProgressiveSaveGame());
        }
    }

    public void LoadGame()
    {
        shouldLoad = true;
        _gameData.LoadGame(Application.persistentDataPath + "/GameData.xml");
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator ProgressiveSaveGame()
    {
        saving = true;
        
        if (HUDManager.instance)
        {
            HUDManager.instance.AddNotification("Saving...", Color.yellow);
        }

        //Prepare player save
        LoadSaveManager.GameSaveData.PlayerData playerData = _gameData.gameSaveData.player;
        playerData = new LoadSaveManager.GameSaveData.PlayerData();
        FPSController player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPSController>();
        player.SavePlayerData();
        
        playerData = _gameData.gameSaveData.player;
        playerData.lives = lives;
        
        //Add player's current checkpoint spawn point
        if (LevelController.instance)
        {
            Transform spawnPoint = LevelController.instance.GetSpawnPoint();
            Debug.Log(spawnPoint.position);

            playerData.spawnPoint.position.x = spawnPoint.position.x;
            playerData.spawnPoint.position.y = spawnPoint.position.y;
            playerData.spawnPoint.position.z = spawnPoint.position.z;

            playerData.spawnPoint.rotation.x = spawnPoint.rotation.x;
            playerData.spawnPoint.rotation.y = spawnPoint.rotation.y;
            playerData.spawnPoint.rotation.z = spawnPoint.rotation.z;
        }

        _gameData.gameSaveData.player = playerData;
        
        yield return null;

        //Prepare enemies save
        _gameData.gameSaveData.enemies.Clear();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        for (int i = 0; i < enemies.Length; ++i)
        {
            Enemy enemy = enemies[i].GetComponent<Enemy>();
            if (enemy)
            {
                enemy.SaveEnemyData();
            }

            yield return null;
        }

        yield return null;
        
        //Save mission data
        if (LevelMission.instance)
        {
            LevelMission.instance.SaveMissionData();
        }

        yield return null;
        
        //Prepare doors save
        SaveDoorData();

        yield return null;
        
        //Save items
        _gameData.gameSaveData.itemBoxes.Clear();
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        for (int i = 0; i < items.Length; ++i)
        {
            LoadSaveManager.GameSaveData.ItemBoxData boxData = new LoadSaveManager.GameSaveData.ItemBoxData();
            ItemBox box = items[i].GetComponent<ItemBox>();
            
            //Save item data
            boxData.ID = items[i].GetInstanceID();
            boxData.itemIndex = box.itemIndex;

            if (box.itemIndex == 4)
            {
                boxData.keyName = box.item.itemName;
            }
            
            //Get transform data
            Transform boxTransform = items[i].transform;

            boxData.transformData.position.x = boxTransform.position.x;
            boxData.transformData.position.y = boxTransform.position.y;
            boxData.transformData.position.z = boxTransform.position.z;

            boxData.transformData.rotation.x = boxTransform.rotation.x;
            boxData.transformData.rotation.y = boxTransform.rotation.y;
            boxData.transformData.rotation.z = boxTransform.rotation.z;
            
            _gameData.gameSaveData.itemBoxes.Add(boxData);
            
            yield return null;
        }

        yield return null;
        
        _gameData.SaveGame(Application.persistentDataPath + "/GameData.xml");

        saving = false;
        
        if (HUDManager.instance)
        {
            HUDManager.instance.AddNotification("Saved Game.", Color.green);
        }
    }

    private void SaveDoorData()
    {
        //Create list of door data, and find all doors
        List<LoadSaveManager.GameSaveData.DoorData> doorData = new List<LoadSaveManager.GameSaveData.DoorData>();
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        //Iterate through all doors and save their data
        for (int i = 0; i < doors.Length; ++i)
        {
            LoadSaveManager.GameSaveData.DoorData doorInstance = new LoadSaveManager.GameSaveData.DoorData();
            Door door = doors[i].GetComponent<Door>();

            doorInstance.locked = door.isLocked;
            doorInstance.opened = door.isOpened;
            doorInstance.ID = doors[i].GetInstanceID();
            
            doorData.Add(doorInstance);
        }

        _gameData.gameSaveData.doors = doorData;
    }

    public bool GetIsLoading()
    {
        return shouldLoad;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public bool IsPaused()
    {
        return paused;
    }
}
