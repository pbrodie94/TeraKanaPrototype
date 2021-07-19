using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 20;

    [HideInInspector]
    public float damage;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //hit the player
            other.GetComponent<Stats>().TakeDamage(damage);
        }
    }
}
