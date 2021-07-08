using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Vector2 levelEnemyCountRange = Vector2.zero;

    private HUDManager hud;
    private EnemySpawner enemySpawner;
    private LevelMission mission;

    private void Start()
    {
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();
        enemySpawner = GetComponent<EnemySpawner>();
        mission = GetComponent<LevelMission>();

        if (levelEnemyCountRange.x <= 0 || levelEnemyCountRange.y <= 0 || levelEnemyCountRange.x > levelEnemyCountRange.y)
            levelEnemyCountRange = Vector2.zero;

        StartCoroutine(InitializeLevel());
    }

    private IEnumerator InitializeLevel()
    {
        //Setup Mission
        mission.InitializeMission(this, hud);
        yield return null;

        //Spawn items

        yield return null;

        //Spawn Enemies
        if (levelEnemyCountRange != Vector2.zero)
        {
            enemySpawner.SetEnemySpawnBounds(Mathf.RoundToInt(levelEnemyCountRange.x), Mathf.RoundToInt(levelEnemyCountRange.y));
        }

        enemySpawner.SpawnEnemies();
        yield return null;

        //Spawn player
    }
}
