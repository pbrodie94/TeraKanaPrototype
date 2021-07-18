using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float collisionCheckDist = 1;

    public float payload = 9999;
    public float damageRadius = 50;
    public float distBuffer = 3;
    public float speed = 1000;

    public Transform target;
    public GameObject Explosion;

    public float distFarExplosion = 50;
    public AudioClip explosionSFXClose;
    public AudioClip explosionSFXFar;

    private Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();

        //Incase wee're not, look at target
        transform.LookAt(target.position);
    }

    private void LateUpdate()
    {
        float dist = Vector3.Distance(transform.position, target.position);

        //Move missile forward
        body.MovePosition(transform.position + Vector3.down * speed * Time.deltaTime);


        if (transform.position.y <= target.position.y + distBuffer /*|| transform.position.y < target.position.y*/)
        {
            Explode();
        }
    }

    void Explode()
    {
        //Spawn explosion
        GameObject go = Instantiate(Explosion, target.position, Quaternion.identity);

        //Destroy the target indicator
        Destroy(target.gameObject);

        //Get distance to player
        Transform p = GameObject.FindGameObjectWithTag("Player").transform;
        float dist = Vector3.Distance(p.position, transform.position);

        //Play audio based on distance
        AudioSource aSource = go.GetComponent<AudioSource>();
        aSource.clip = dist >= distFarExplosion ? explosionSFXFar : explosionSFXClose;
        aSource.Play();

        //Deal damage to surrounding enemies and player
        DealDamage();

        //Destroy explosion object after a few seconds
        Destroy(go, 10);
        //Destroy game object
        Destroy(gameObject);
    }

    void DealDamage()
    {
        //30% max damage
        //30% - 35% severe damage
        //35% - 60% moderate damage

        Collider[] cols = Physics.OverlapSphere(target.position, damageRadius);
        
        for (int i = 0; i < cols.Length; i++)
        {
            Stats s = null;
            float dist = 0;

            if (cols[i].tag == "Enemy")
            {
                s = cols[i].gameObject.GetComponent<Stats>();
                dist = Vector3.Distance(transform.position, cols[i].transform.position);

            } else if (cols[i].tag == "Player")
            {
                s = cols[i].gameObject.GetComponent<Stats>();
                dist = Vector3.Distance(transform.position, cols[i].transform.position);

            }

            if (s != null)
                s.TakeDamage(GetDamage(dist));
        }
    }

    private float GetDamage(float dist)
    {
        //if victim is within 30% of epicenter, deal max damage
        if (dist <= damageRadius * 0.3f)
        {
            //deal max damage
            return payload;
        } else
        {

            //Linear falloff of damage to the outside of the blast radius
            float damagePercentage = dist / damageRadius;
            float dam = payload - (payload * damagePercentage);
            return dam;
        }
    }
}
