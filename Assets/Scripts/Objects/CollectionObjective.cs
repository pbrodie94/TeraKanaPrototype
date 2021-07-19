using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionObjective : Objective
{

    public string objectName;

    protected override void Start()
    {
        base.Start();

        objectiveType = MissionType.Collection;

        if (objectName == null)
        {
            objectName = "Object";
        }

        interactMessage += "collect " + objectName + ".";
    }


    protected override void Interact()
    {
        //Collect specimen
        //Hide message, and destroy script
        hud.ShowMessage(null, false);
        messageShown = false;
        isInteractible = false;

        hud.AddNotification("Acquired " + objectName);

        missionManager.CompletedObjective();
        Destroy(gameObject);
    }
}
