using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIStateManager : MonoBehaviour
{
    private Transform target;

    public enum EnemyState
    {
        Sleep,
        Idle,
        Alert,
        Engaging
    }

    public EnemyState state = EnemyState.Idle;
    public EnemyState defaultState = EnemyState.Idle;

    [Tooltip("Alert timeout in seconds.")]
    public float alertTimeout = 60;
    private float timeLastDetected;

    protected Enemy enemy;
    protected EnemyStats stats;
    protected TargetDetection targetDetection;

    protected CombatManager cm;

    private bool alarmEnemy = false;

    protected void Start()
    {
        /*if (!target)
            target = GameObject.FindGameObjectWithTag("Player").transform;*/

        if (!enemy)
            enemy = GetComponent<Enemy>();

        if (!cm)
            cm = GameObject.FindGameObjectWithTag("GameController").GetComponent<CombatManager>();

        stats = GetComponent<EnemyStats>();
        targetDetection = GetComponent<TargetDetection>();

        LevelController.PlayerSpawned += GetPlayerReference;
    }

    public void GetPlayerReference()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        LevelController.PlayerSpawned -= GetPlayerReference;
    }

    private void LateUpdate()
    {
        if (alarmEnemy)
        {
            return;
        }
        
        if (targetDetection.DetectTarget(target))
        {
            //Target seen

            //Alert other enemies in the area
            if (state != EnemyState.Engaging)
                enemy.Yell(target);

            UpdateState(EnemyState.Engaging);
            timeLastDetected = Time.time;
        }
        else {

            //Target not seen
            if (targetDetection.SoundDetected())
            {
                UpdateState(EnemyState.Alert);
                timeLastDetected = Time.time;
            }
            else
            {

                if (state == EnemyState.Engaging)
                {
                    UpdateState(EnemyState.Alert);
                }
            }
        }

        if (alertTimeout > 0 && state == EnemyState.Alert)
        {

            if (Time.time >= timeLastDetected + alertTimeout)
            {
                UpdateState(defaultState);
            }
        }
    }

    void UpdateState(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Sleep:
                state = newState;
                break;

            case EnemyState.Idle:
                state = newState;
                break;

            case EnemyState.Alert:

                //if target initially detected, add to combat manager's list
                if (state == EnemyState.Idle || state == EnemyState.Sleep)
                {
                    cm.AddEnemy(enemy);
                }

                state = newState;

                break;

            case EnemyState.Engaging:
                
                //if target initially detected, add to combat manager's list
                if (state == EnemyState.Idle || state == EnemyState.Sleep)
                {
                    if (!cm)
                    {
                        cm = GameObject.FindGameObjectWithTag("GameController").GetComponent<CombatManager>();
                    }
                    cm.AddEnemy(enemy);
                }

                state = newState;

                break;
        }
    }

    public void Alerted()
    {
        //Alert other enemies in the area
        UpdateState(EnemyState.Alert);
        timeLastDetected = Time.time;
    }

    public void Aggro()
    {
        UpdateState(EnemyState.Engaging);
        alarmEnemy = true;
    }

    public void Die()
    {
        cm.RemoveEnemy(enemy);
    }

    public EnemyState GetCurrentState()
    {
        return state;
    }
}
