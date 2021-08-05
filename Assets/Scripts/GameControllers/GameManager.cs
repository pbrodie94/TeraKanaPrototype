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
    private bool paused = false;

    void Awake()
    {
        //Setup singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
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
