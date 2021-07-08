using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public int maxEngagingEnemies = 3;
    public Vector2 enemyRotateIntervalRange = new Vector2(5, 10);
    private float enemyRotateIterval;


    public List<EnemyObject> enemies = new List<EnemyObject>();
    public int numberAttackers = 0;

    private float timeLastRotate = 0;

    private void Start()
    {
        enemyRotateIterval = Random.Range(enemyRotateIntervalRange.x, enemyRotateIntervalRange.y);
    }

    private void LateUpdate()
    {
        if (enemies.Count > maxEngagingEnemies)
        {
            if (Time.time >= timeLastRotate + enemyRotateIterval)
            {
                RotateEnemies();
            }
        }

        //Debug.Log("Number of attackers: " + numberAttackers);
    }

    public void RotateEnemies()
    {
        //If more than max engaging enemies, rotate attacking enemies
        if (enemies.Count > maxEngagingEnemies)
        {
            float atkTime = 0;
            float notAttackTime = 0;
            int atkIndex = 0;
            int nAtkIndex = 0;

            //Loop thorugh active enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                //if attacking
                if (enemies[i].attacking)
                {
                    //Get tme started attacking
                    float x = Time.time - enemies[i].timeStartAttack;
                    //if not attacked longer, save index
                    if (x > atkTime)
                    {
                        atkTime = x;
                        atkIndex = i;
                    }
                } else
                {
                    //Get time not attacked
                    float y = Time.time - enemies[i].timeLastAttacked;
                    //if not attacked longer, save index
                    if (y > notAttackTime)
                    {
                        notAttackTime = y;
                        nAtkIndex = i;
                    }
                }
            }

            //Swap the longest attacker with the longest not attacker
            if (numberAttackers > maxEngagingEnemies)
                enemies[atkIndex].EndAttack();

            enemies[nAtkIndex].StartAttack();

            timeLastRotate = Time.time;
        } else
        {
            //If there are less than the maximum engaging enemies, all enemies can attack
            //Don't 're-start' attack so start of attack time is accurate
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].attacking)
                    enemies[i].StartAttack();
            }
        }

        //If we have enough alerted enemies, and not full on attackers, find another enemy to attack
        if (enemies.Count > maxEngagingEnemies && numberAttackers < maxEngagingEnemies)
            RotateEnemies();

        enemyRotateIterval = Random.Range(enemyRotateIntervalRange.x, enemyRotateIntervalRange.y);
    }

    public bool AddEnemy(Enemy e)
    {
        Debug.Log("Adding " + e);

        if (enemies.Count > 1)
        {

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].en == e)
                {
                    return false;
                }
            }

            EnemyObject en = new EnemyObject(e, this);

            enemies.Add(en);

            if (enemies.Count < maxEngagingEnemies || numberAttackers < maxEngagingEnemies)
                en.StartAttack();

            Debug.Log(enemies.Count);

            return true;

        } else
        {
            EnemyObject en = new EnemyObject(e, this);

            en.StartAttack();

            enemies.Add(en);

            Debug.Log(enemies.Count);

            return true;
        }
        
    }

    public bool RemoveEnemy(Enemy e)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].en == e)
            {
                EnemyObject eo = enemies[i];

                if (eo.attacking)
                    eo.EndAttack();

                enemies.Remove(eo);

                //Debug.Log(enemies.Count);

                RotateEnemies();

                return true;
            }
        }

        return false;
    }
}

public class EnemyObject
{
    public EnemyObject(Enemy e, CombatManager c)
    {
        en = e;
        cm = c;
    }

    public Enemy en;
    public CombatManager cm;
    public bool attacking = false;
    public float timeStartAttack;
    public float timeLastAttacked;

    public void StartAttack()
    {
        //Switch enemy into attack mode
        attacking = true;
        en.attackTurn = true;
        cm.numberAttackers++;
        timeStartAttack = Time.time;
    }

    public void EndAttack()
    {
        //Switch enemy out of attack mode
        attacking = false;
        en.attackTurn = false;
        cm.numberAttackers--;
        timeLastAttacked = Time.time;
    }
}
