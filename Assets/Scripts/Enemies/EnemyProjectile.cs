using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float damage = 10;

    private void Start()
    {
        //Destroy after 60 seconds of being created
        Destroy(gameObject, 60);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == gameObject || other.CompareTag("EnemyTarget") || other.CompareTag("SpawnArea") || other.gameObject.CompareTag("Gas") || other.CompareTag("RockSlide"))
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
        
        //Deal damage to player if hit
        if (other.CompareTag("Player"))
        {
            PlayerStats ps = other.gameObject.GetComponent<PlayerStats>();
            ps.TakeDamage(damage);
        }

        //Destroy projectile regardless of what is hit
        Destroy(gameObject);
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}
