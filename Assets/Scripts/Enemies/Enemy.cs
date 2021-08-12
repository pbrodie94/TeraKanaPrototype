using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public Transform target;

    public float walkSpeed = 6;
    public float runSpeed = 8;
    public float acceleration = 3;
    protected float wantedSpeed;
    protected float speed;

    public float alertRadius = 30;

    [Header("Combat")]

    [HideInInspector] public bool attackTurn = false;
    public float meleeAttackDist = 3;
    public float engageRadius = 10;
    public float attackDelay = 1.5f;
    protected bool attacking = false;

    public Collider[] meleeColliders;
    [Tooltip ("Set frames to wait before tuning the melee colliders on, and off.")]
    public Vector2[] onOffFrames;

    protected float distToTarget;
    protected float timeLastAttacked; 
    
    protected const float frameInterval = 0.042f;

    protected EnemyAIStateManager aiState;
    protected EnemyStats stats;
    protected NavMeshAgent agent;
    protected Animator anim;

    protected AudioSource audioSource;
    [SerializeField] protected AudioClip deathAudio;
    [SerializeField] protected AudioClip[] hitSoundEffects;
    
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    protected static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
    protected static readonly int Attack = Animator.StringToHash("Attack");

    protected virtual void Start()
    {
        aiState = GetComponent<EnemyAIStateManager>();
        stats = GetComponent<EnemyStats>();
        anim = transform.GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        //agent.baseOffset = 0;

        if (meleeAttackDist > 0)
            agent.stoppingDistance = meleeAttackDist;

        LevelController.PlayerSpawned += GetPlayerReference;
    }

    public void GetPlayerReference()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        LevelController.PlayerSpawned -= GetPlayerReference;
    }

    protected virtual void LateUpdate()
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
        anim.SetFloat(MoveSpeed, speed);
        

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
            agent.stoppingDistance = meleeAttackDist;

            //If too far, move towards target
            if (distToTarget > agent.stoppingDistance)
            {
                //Set destination to target
                agent.SetDestination(target.position);

                //Run
                wantedSpeed = runSpeed;
            }
            else if (distToTarget < meleeAttackDist)
            {
                speed = 0;
                agent.velocity = Vector3.zero;

                RotateTowards();

                //If last attacked is greater than the time elapsed + the delay
                if (Time.time > timeLastAttacked + attackDelay && !attacking)
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

                if (distToTarget < meleeAttackDist && Time.time >= timeLastAttacked + attackDelay)
                    StartCoroutine(Melee());
            }
        }
    }

    protected virtual IEnumerator Melee()
    {
        attacking = true;
        
        //Set attack animation
        anim.SetInteger(AttackIndex, 0);
        anim.SetTrigger(Attack);

        //Length of 1 anim frame (24fps 1sec / 24frames) x the number of frames needed to wait
        float wait = frameInterval * onOffFrames[0].x;

        yield return new WaitForSeconds(wait);

        SetMeleeColliders(0, true);

        wait = frameInterval * onOffFrames[0].y;
        yield return new WaitForSeconds(wait);

        SetMeleeColliders(0, false);
        
        //Time last attacked for delay
        timeLastAttacked = Time.time;

        attacking = false;
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

        if (audioSource && hitSoundEffects[0])
        {
            int soundIndex = 0;

            if (hitSoundEffects.Length > 1)
            {
                soundIndex = Random.Range(0, hitSoundEffects.Length - 1);
            }
            
            audioSource.PlayOneShot(hitSoundEffects[soundIndex]);
        }
    }

    public virtual void Yell(Transform target)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, alertRadius);

        if (cols.Length > 0)
        {
            foreach (Collider c in cols)
            {
                if (c.CompareTag("Enemy") && c.gameObject != gameObject)
                {
                    Enemy e = c.GetComponent<Enemy>();

                    if (e)
                    {
                        e.Alert();
                    }
                }
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

        if (audioSource && deathAudio)
        {
            audioSource.PlayOneShot(deathAudio);
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
