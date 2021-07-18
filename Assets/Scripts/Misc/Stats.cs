using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    protected float health;
    public float startingHealth = 100;
    public float maxHealth;

    public float attack;
    public float defence;

    protected bool dead = false;
    public bool died
    {
        get { return dead; }
    }

    protected virtual void Start()
    {
        if (startingHealth <= 0)
            health = maxHealth;
        else
            health = startingHealth;
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
