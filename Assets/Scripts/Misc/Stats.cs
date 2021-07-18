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
        if (maxHealth <= 0)
            maxHealth = 100;

        if (startingHealth <= 0 || startingHealth > maxHealth)
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
            health = 0;
            dead = true;
        }
    }
}
