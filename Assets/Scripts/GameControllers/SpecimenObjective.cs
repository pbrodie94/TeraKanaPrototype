using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecimenObjective : Objective
{
    protected override void Awake()
    {
        base.Awake();

        objectiveType = MissionType.Specimen;
        message = "Press 'E' to collect Specimen Sample.";
    }

    protected override void Interaction()
    {
        //Collect specimen
        //Hide message, and destroy script
        hud.ShowMessage(null, false);
        messageShown = false;
        interactable = false;
        missionManager.CompletedObjective(this);
    }
}
