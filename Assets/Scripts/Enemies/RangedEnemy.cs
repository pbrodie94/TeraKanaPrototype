using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Ranged Combat")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float firePower = 500;
    [SerializeField] private float fireRate = 3;
    private float timeLastFired;
    private bool shooting = false;
    private static readonly int Shoot1 = Animator.StringToHash("Shoot");

    protected override void Engage()
    {
        if (stats.died || !target)
            return;
        
        //Ranged enemy does not try to melee target, but will if target is close enough
        if (distToTarget <= meleeAttackDist)
        {
            //Melee the player
            if (Time.time >= timeLastAttacked + attackDelay && !attacking)
            {
                StartCoroutine(Melee());
            }

            return;
        }

        if (distToTarget > engageRadius)
        {
            agent.SetDestination(target.position);
            wantedSpeed = runSpeed;
        }
        else
        {
            speed = 0;
            wantedSpeed = 0;
            agent.velocity = Vector3.zero;
            
            RotateTowards();

            if (Time.time > timeLastFired)
            {
                if (!shooting)
                {
                    StartCoroutine(Shoot());
                }
            }
        }
    }

    protected virtual IEnumerator Shoot()
    {
        shooting = true;
        
        if (!projectile)
        {
            StopCoroutine(Shoot());
        }

        anim.SetTrigger(Shoot1);

        float wait = frameInterval * 22;
        yield return new WaitForSeconds(wait);
        int shots = 1;

        do
        {
            //Create projectile, set damage and add force
            GameObject proj = Instantiate(projectile, firePoint.position, firePoint.rotation);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * firePower);
            EnemyProjectile eProj = proj.GetComponent<EnemyProjectile>();
            eProj.SetDamage(stats.attack);

            wait = frameInterval * 8;
            yield return new WaitForSeconds(wait);

            ++shots;

        } while (shots < 4);

        timeLastFired = Time.time + fireRate;

        shooting = false;
    }

    protected override IEnumerator Melee()
    {
        attacking = true;
        
        int atkIndex = SelectAttack();
        float wait;

        anim.SetInteger(AttackIndex, atkIndex);
        anim.SetTrigger(Attack);

        wait = frameInterval * onOffFrames[atkIndex].x;
        yield return new WaitForSeconds(wait);
        
        SetMeleeColliders(0, true);

        wait = frameInterval * onOffFrames[atkIndex].y;
        yield return new WaitForSeconds(wait);
        
        SetMeleeColliders(0, false);
        
        //Time last attacked for delay
        timeLastAttacked = Time.time;

        attacking = false;
    }

    private int SelectAttack()
    {
        float randGen = Random.Range(0, 100);

        if (randGen <= 20)
        {
            return 1;
        }

        return 0;
    }
}
