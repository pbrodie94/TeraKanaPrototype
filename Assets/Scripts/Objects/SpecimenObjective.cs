using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecimenObjective : Objective
{
    protected override void Start()
    {
        base.Start();

        objectiveType = MissionType.Specimen;
        interactMessage += "collect Specimen Sample.";
    }

    protected override void Interact()
    {
        //Collect specimen
        //Hide message, and destroy script
        hud.ShowMessage(null, false);
        messageShown = false;
        isInteractible = false;

        hud.AddNotification("Acquired specimen sample");

        missionManager.CompletedObjective(this);
    }
}
