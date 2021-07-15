using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHoldManager : MonoBehaviour
{
    [Header("Preferences")]
    public float smoothing = 10;
    //public float swayAmount = 0.01f;
    public float maxHipSwayAmount = 0.5f;
    public float maxAimSwayAmount = 0.02f;
    protected float maxSwayAmount;
    public float swaySmoothing = 5;

    protected Vector3 positionVelocity = Vector3.zero;
    //protected Quaternion rotationVelocity = Quaternion.identity;

    protected Vector3 holdPosition;
    protected Vector3 hipPosition;
    protected Vector3 holsterPosition;
    protected Vector3 sprintPosition;
    protected Quaternion sprintRotation;
    protected Quaternion wantedRotation = Quaternion.identity;

    protected GameObject player;

    protected PlayerMove playerControl;
    protected PlayerStats stats;
    protected Inventory inventory;
    protected HUDManager hud;
    protected Transform cam;

    protected virtual void Start()
    {
        if (!cam)
            cam = GameObject.Find("Main Camera").transform;

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        if (!playerControl)
            playerControl = GetComponent<PlayerMove>();

        if (!inventory)
            inventory = GetComponent<Inventory>();

        if (!hud)
            hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();

        holdPosition = hipPosition;
    }

    
}
