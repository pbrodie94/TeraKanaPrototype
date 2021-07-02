using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecimenObjective : Objective
{
    protected override void Awake()
    {
        base.Awake();

        objectiveType = MissionType.Specimen;
    }

    private void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        //Is the player close enough to interact
        if (distToPlayer <= interactRadius)
        {
            RaycastHit hit;
            
            //Is the player looking at me
            if (Physics.Raycast(cam.position, cam.forward, out hit, interactRadius + 5))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    //Display interact message on screen
                    hud.ShowMessage("Press 'E' to collect Specimen sample.", true);
                    messageShown = true;

                    if (Input.GetButtonDown(InputManager.Action))
                    {
                        //Collect specimen
                        //Hide message, and destroy script
                        hud.ShowMessage(null, false);
                        messageShown = false;
                        missionManager.CompletedObjective(this);
                    }

                } else if (messageShown)
                {
                    hud.ShowMessage(null, false);
                    messageShown = false;
                }
            }
        } else if (messageShown)
        {
            hud.ShowMessage(null, false);
            messageShown = false;
        }
    }
}
