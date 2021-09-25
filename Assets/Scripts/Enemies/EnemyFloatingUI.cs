using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFloatingUI : MonoBehaviour
{
    [SerializeField] private GameObject enemyFloatingUI;
    [SerializeField] private Text enemyNameText;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private float showDistance = 10;

    private Transform player;

    private EnemyStats enemyStats;
    private EnemyAIStateManager aiStateManager;

    private void Start()
    {
        LevelController.PlayerSpawned += GetPlayerReference;

        //Get reference to necessary scripts
        enemyStats = GetComponentInParent<EnemyStats>();
        aiStateManager = GetComponentInParent<EnemyAIStateManager>();

        //Set enemy's name
        enemyNameText.text = enemyStats.GetEnemyName();
        
        //Set the health bar to defaults
        enemyHealthSlider.maxValue = enemyStats.GetMaxHealth();
        enemyHealthSlider.value = enemyStats.GetCurrentHealth();
        
        enemyFloatingUI.SetActive(false);
    }

    public void GetPlayerReference()
    {
        //Get player reference
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        if (!enemyFloatingUI || !player)
            return;
        
        //Check for distance to show floating ui, and also if the enemy is alerted
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= showDistance || aiStateManager.GetCurrentState() != EnemyAIStateManager.EnemyState.Sleep &&
            aiStateManager.GetCurrentState() != EnemyAIStateManager.EnemyState.Idle)
        {
            enemyFloatingUI.SetActive(true);
            
            //Always look at player 
            transform.LookAt(player.position);
        }
        else
        {
            enemyFloatingUI.SetActive(false);
        }

        enemyHealthSlider.value = enemyStats.GetCurrentHealth();
    }
}
