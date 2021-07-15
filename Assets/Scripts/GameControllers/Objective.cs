using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : Interactible
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

    protected LevelMission missionManager;

    public virtual void InitializeObjective(LevelMission manager, HUDManager hudManager)
    {
        _completed = false;
        missionManager = manager;
        hud = hudManager;

        if (interactionRange <= 0)
            interactionRange = 4;
    }

}
