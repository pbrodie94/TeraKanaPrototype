using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionObjective : Objective
{

    public string objectName;

    protected override void Awake()
    {
        base.Awake();

        objectiveType = MissionType.Collection;

        if (objectName == null)
        {
            objectName = "Object";
        }

        message = "Press 'E' to collect " + objectName + ".";
    }

    protected override void Interaction()
    {
        //Collect specimen
        //Hide message, and destroy script
        hud.ShowMessage(null, false);
        messageShown = false;
        interactable = false;

        hud.AddNotification("Acquired " + objectName);

        missionManager.CompletedObjective();
        Destroy(gameObject);
    }
}
