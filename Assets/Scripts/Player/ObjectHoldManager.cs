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

    protected Vector3 holdPosition;
    protected Vector3 hipPosition;
    protected Vector3 holsterPosition;
    protected Vector3 sprintPosition;
    protected Quaternion sprintRotation;
    protected Quaternion wantedRotation = Quaternion.identity;

    //protected GameObject player;

    //protected PlayerMove playerControl;
    protected FPSController playerController;
    protected PlayerStats stats;
    protected Inventory inventory;
    protected Transform cam;
    protected Animator anim;
    [SerializeField] protected Transform weaponParent;

    //[SerializeField] protected Transform fpsArms;
    //[SerializeField] protected Animator weaponAnim;
    //[SerializeField] private RuntimeAnimatorController defaultController;

    protected virtual void Start()
    {
        if (!cam)
            cam = Camera.main.transform;

        if (!playerController)
            playerController = GetComponent<FPSController>();

        if (!inventory)
            inventory = GetComponent<Inventory>();

        anim = GetComponent<Animator>();

        holdPosition = hipPosition;
    }

    
}
