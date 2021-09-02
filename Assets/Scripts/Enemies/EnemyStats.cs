using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    [SerializeField] private string enemyName;

    [SerializeField] private int numberOfDamageAnims = 0;
    
    public float damageAnimRatio = 35;

    int takeDamageAnim = 0;
    float damageAnimHealth;
    float nextDamageAnim;
    private Animator anim;

    EnemyAIStateManager aiState;
    private static readonly int DamageIndex = Animator.StringToHash("DamageIndex");
    private static readonly int Damage = Animator.StringToHash("TakeDamage");
    private static readonly int Die1 = Animator.StringToHash("Die");

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
        if (!dead && health <= nextDamageAnim && numberOfDamageAnims > 0)
        {
            nextDamageAnim -= damageAnimHealth;

            if (numberOfDamageAnims > 1)
            {
                takeDamageAnim = Random.Range(0, numberOfDamageAnims - 1);
                anim.SetInteger(DamageIndex, takeDamageAnim);
            }

            anim.SetTrigger(Damage);
        } 

    }

    protected override void Die()
    {
        base.Die();

        dead = true;

        anim.SetTrigger(Die1);

        Enemy e = GetComponent<Enemy>();
        e.Die();
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public string GetEnemyName()
    {
        return enemyName;
    }
}
