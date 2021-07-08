using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    [Header("Objective Properties")]
    public MissionType objectiveType;

    protected string message;
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
    protected bool interactable = true;

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

        if (interactRadius <= 0)
            interactRadius = 4;
    }

    protected virtual void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        //Is the player close enough to interact
        if (distToPlayer <= interactRadius && interactable)
        {
            RaycastHit hit;

            //Is the player looking at me
            if (Physics.Raycast(cam.position, cam.forward, out hit, interactRadius + 5))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    //Display interact message on screen
                    hud.ShowMessage(message, true);
                    messageShown = true;

                    if (Input.GetButtonDown(InputManager.Action))
                    {
                        Interaction();
                    }

                }
                else if (messageShown)
                {
                    hud.ShowMessage(null, false);
                    messageShown = false;
                }
            }
        }
        else if (messageShown)
        {
            hud.ShowMessage(null, false);
            messageShown = false;
        }
    }

    protected virtual void Interaction()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
