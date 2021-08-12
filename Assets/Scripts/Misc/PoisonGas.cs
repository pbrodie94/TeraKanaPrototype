using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    [SerializeField] private float damage = 5;
    [SerializeField] private float damageInterval = 1.5f;

    private float timeLastDamaged;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time >= timeLastDamaged)
            {
                PlayerStats ps = other.GetComponent<PlayerStats>();
                ps.TakeDamage(damage);

                timeLastDamaged = Time.time + damageInterval;
            }
        }
    }
}
