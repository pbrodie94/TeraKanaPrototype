using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float health;

    public float attack;
    public float defence;

    protected bool dead = false;
    public bool died
    {
        get { return dead; }
    }

    public virtual void TakeDamage(float damage)
    {
        if (dead)
            return;

        health -= damage;

        if (health <= 0)
        {
            dead = true;
        }
    }
}
