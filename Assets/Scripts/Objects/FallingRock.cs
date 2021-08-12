using System;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [SerializeField] private float damage = 5;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (rb.velocity.magnitude > 1)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //Damage player
                PlayerStats ps = other.gameObject.GetComponent<PlayerStats>();
                ps.TakeDamage(damage);
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                //Damage enemy
                EnemyStats es = other.gameObject.GetComponent<EnemyStats>();
                es.TakeDamage(damage);
            }
        }
    }
}
