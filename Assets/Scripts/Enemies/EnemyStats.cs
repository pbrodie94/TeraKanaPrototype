using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{ 
    public float damageAnimRatio = 35;

    int takeDamageAnim = 0;
    float damageAnimHealth;
    float nextDamageAnim;
    public Animator anim;

    EnemyAIStateManager aiState;

    protected override void Start()
    {
        base.Start();

        if (!anim)
            anim = GetComponentInChildren<Animator>();

        if (!aiState)
            aiState = GetComponent<EnemyAIStateManager>();

        damageAnimHealth = health / damageAnimRatio;
        damageAnimHealth = health / damageAnimHealth;
        nextDamageAnim = health - damageAnimHealth;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (aiState.state != EnemyAIStateManager.EnemyState.Alert)
            aiState.Alerted();

        if (health <= 0)
        {
            HUDManager.instance.SetHitMarker(true);
        } else
        {
            HUDManager.instance.SetHitMarker(false);
        }

        //take damage animation
        if (!dead && health <= nextDamageAnim)
        {
            nextDamageAnim -= damageAnimHealth;

            takeDamageAnim = Random.Range(0, 2);
            anim.SetInteger("DamageIndex", takeDamageAnim);
            anim.SetTrigger("TakeDamage");
        } 
        
        if (dead) 
        {
            dead = true;

            anim.SetTrigger("Die");

            Enemy e = GetComponent<Enemy>();
            e.Die();

        }

    }
}
