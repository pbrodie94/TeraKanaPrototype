using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    [Header("Objective Properties")]
    public MissionType objectiveType;

    protected bool _completed = false;
    public bool completed
    {
        get
        {
            return _completed;
        }
    }

    [SerializeField] protected float interactRadius;
    protected bool messageShown = false;

    protected Transform player;
    protected Transform cam;
    protected LevelMission missionManager;
    protected HUDManager hud;

    protected virtual void Awake()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (!cam)
            cam = Camera.main.transform;
    }

    public virtual void InitializeObjective(LevelMission manager, HUDManager hudManager)
    {
        _completed = false;
        missionManager = manager;
        hud = hudManager;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
