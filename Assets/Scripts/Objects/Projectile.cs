using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float lifeTime = 20; 
    private float damage;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Init(float damage, float lifeTime)
    {
        this.damage = damage;
        this.lifeTime = lifeTime;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == gameObject || other.CompareTag("SpawnArea") || other.gameObject.CompareTag("Gas") || other.CompareTag("RockSlide"))
        {
            return;
        }
        
        foreach (Transform child in transform)
        {
            if (child.gameObject == other.gameObject)
            {
                return;
            }
        }
        
        if (other.CompareTag("Enemy"))
        {
            //hit the enemy
            other.GetComponent<Stats>().TakeDamage(damage);
        }

        if (other.CompareTag("EnemyTarget"))
        {
            EnemyLimb e = other.GetComponent<EnemyLimb>();
            e.TakeDamage(damage);

        }
        
        Destroy(gameObject);
    }
}
