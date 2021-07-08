using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLimb : MonoBehaviour
{
    public int percentDamageTaken = 100;
    private float damageModifier;
    private EnemyStats stats;

    private void Start()
    {
        stats = gameObject.GetComponentInParent<EnemyStats>();

        //Debug.Log(stats);

        if (percentDamageTaken != 0)
        {
            damageModifier = percentDamageTaken / 100;
        } else
        {
            damageModifier = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        float modDam = damage * damageModifier;
        stats.TakeDamage(modDam);

        //Debug.Log("Shot " + gameObject.name);
    }
}
