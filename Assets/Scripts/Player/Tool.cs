using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public string toolName;

    [HideInInspector]
    public bool equipped = false;
    public bool holstered = false;
    protected bool fired = false;

    public Vector3 hipPosition;
    public Vector3 holsterPosition;
    public Vector3 sprintPosition;
    public Vector3 sprintRotation;

    protected ToolManager tm;

    protected virtual void Start()
    {
        tm = GameObject.FindGameObjectWithTag("Player").GetComponent<ToolManager>();
    }

    public virtual void DestroyDisplayElements()
    {

    }
}
