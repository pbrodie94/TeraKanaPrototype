using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : Interactable
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

    public virtual void InitializeObjective()
    {
        _completed = false;

        if (interactionRange <= 0)
            interactionRange = 4;
    }

}
