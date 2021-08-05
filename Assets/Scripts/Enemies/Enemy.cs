using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform target;

    public float walkSpeed = 6;
    public float runSpeed = 8;
    public float acceleration = 3;
    protected float wantedSpeed;
    protected float speed;

    public float alertRadius = 30;

    [Header("Combat")]

    public bool attackTurn = false;
    public float attackDist;
    public float engageRadius = 10;
    public float attackDelay = 1.5f;

    public Collider[] meleeColliders;
    [Tooltip ("Set frames to wait before tuning the melee colliders on, and off.")]
    public Vector2[] onOffFrames;

    protected float distToTarget;
    protected float timeLastAttacked; 

    protected EnemyAIStateManager aiState;
    protected EnemyStats stats;
    protected NavMeshAgent agent;
    protected Animator anim;

    protected virtual void Start()
    {
        aiState = GetComponent<EnemyAIStateManager>();
        stats = GetComponent<EnemyStats>();
        anim = transform.GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        //agent.baseOffset = 0;

        if (attackDist > 0)
            agent.stoppingDistance = attackDist;

        LevelController.PlayerSpawned += GetPlayerReference;
    }

    public void GetPlayerReference()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        LevelController.PlayerSpawned -= GetPlayerReference;
    }

    protected void LateUpdate()
    {
        if (stats.died)
            return;

        switch(aiState.state)
        {
            case EnemyAIStateManager.EnemyState.Sleep:

                break;

            case EnemyAIStateManager.EnemyState.Idle:

                break;

            case EnemyAIStateManager.EnemyState.Alert:

                Engage();

                break;

            case EnemyAIStateManager.EnemyState.Engaging:

                Engage();

                break;
        }

        speed = Mathf.Lerp(speed, wantedSpeed, acceleration * Time.deltaTime);

        agent.speed = speed;
        anim.SetFloat("MoveSpeed", speed);
        anim.SetBool("Alert", (aiState.state == EnemyAIStateManager.EnemyState.Alert || aiState.state == EnemyAIStateManager.EnemyState.Engaging));

        if (target)
        {
            distToTarget = Vector3.Distance(transform.position, target.position);
        }
    }

    protected virtual void RotateTowards()
    {
        if (!target)
        {
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = rot;
    }

    protected virtual void Engage()
    {
        if (stats.died || !target)
            return;

        //Combat manager has given enemy a turn to attack
        if (attackTurn)
        {
            //Set to attacking distance
            agent.stoppingDistance = attackDist;

            //If too far, move towards target
            if (distToTarget > agent.stoppingDistance)
            {
                //Set destination to target
                agent.SetDestination(target.position);

                //Run
                wantedSpeed = runSpeed;
            }
            else if (distToTarget < agent.stoppingDistance)
            {
                speed = 0;
                agent.velocity = Vector3.zero;

                RotateTowards();

                //If last attacked is greater than the time elapsed + the delay
                if (Time.time > timeLastAttacked + attackDelay)
                {
                    StartCoroutine(Melee());
                }
            }
        }
        else
        {
            //Not our turn to attack
            //Set distance to a medium close distance
            agent.stoppingDistance = engageRadius;

            //We can get closer if we want
            if (distToTarget > agent.stoppingDistance)
            {
                //Set destination to target
                agent.SetDestination(target.position);

                //Run
                wantedSpeed = runSpeed;
            }
            else
            {
                speed = 0;
                agent.velocity = Vector3.zero;

                //We're close enough, just look at target for now
                RotateTowards();

                if (distToTarget < attackDist)
                    StartCoroutine(Melee());
            }
        }
    }

    protected virtual IEnumerator Melee()
    {
        //Time last attacked for delay
        timeLastAttacked = Time.time;
        
        //Set attack animation
        anim.SetInteger("AttackIndex", 0);
        anim.SetTrigger("Attack");

        //Length of 1 anim frame (24fps 1sec / 24frames) x the number of frames needed to wait
        float wait = 0.042f * onOffFrames[0].x;

        yield return new WaitForSeconds(wait);

        SetMeleeColliders(0, true);

        wait = 0.042f * onOffFrames[0].y;
        yield return new WaitForSeconds(wait);

        SetMeleeColliders(0, false);
    }

    public void SetMeleeColliders(int colliderIndex, bool TurnedOn)
    {
        //If no melee colliders, exit
        if (meleeColliders.Length == 0 || meleeColliders == null)
            return;

        //Turning on colliders
        if (TurnedOn)
        {
            if (meleeColliders.Length >= colliderIndex + 1)
            {
                meleeColliders[colliderIndex].enabled = true;
            } else
            {
                meleeColliders[0].enabled = true;
            }
        } else
        //Turning off colliders
        {
            for (int i = 0; i < meleeColliders.Length; i++)
            {
                meleeColliders[i].enabled = false;
            }
        }
    }

    public void HitTarget(GameObject go)
    {
        PlayerStats ps = go.GetComponent<PlayerStats>();

        ps.TakeDamage(stats.attack);
    }

    public virtual void Yell(Transform target)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, alertRadius);

        foreach (Collider c in cols)
        {
            if (c.tag == "Enemy")
            {
                Enemy e = c.GetComponent<Enemy>();
                e.Alert();
            }
        }
    }

    public virtual void Alert()
    {
        aiState.Alerted();
    }

    public virtual void Die()
    {
        agent.enabled = false;
        Collider col = GetComponent<Collider>();
        Collider[] cols = GetComponentsInChildren<Collider>();

        aiState.Die();

        if (col)
            col.enabled = false;

        if (cols.Length > 0)
        {
            foreach (Collider c in cols)
            {
                c.enabled = false;
            }
        }

        Destroy(gameObject, 30);

        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
}
