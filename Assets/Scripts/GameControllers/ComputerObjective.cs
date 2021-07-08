using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerObjective : Objective
{

    [SerializeField] private float downloadSpeed = 1;
    private float progress = 0;
    private string downloadMessage = "Downloading files: ";
    private bool downloading = false;
    private bool downloadComplete = false;

    protected override void Awake()
    {
        base.Awake();

        objectiveType = MissionType.Computer;

        message = "Press 'E' to begin download.";
    }

    protected override void Interaction()
    {
        //Start Upload progress bar
        hud.InitializeProgressBar(downloadMessage, progress);
        downloading = true;
        interactable = false;

        hud.AddNotification("Downloading computer data.", HUDManager.NotificationType.Warning);
        hud.AddNotification("ALARM DETECTED!", HUDManager.NotificationType.Alert);
    }

    private void FixedUpdate()
    {
        if (downloading)
        {
            if (progress < 100)
            {
                progress += downloadSpeed * Time.deltaTime;
            } else
            {
                progress = 100;
                downloadComplete = true;

                missionManager.CompletedObjective(this);
            }

            hud.UpdateProgressBar(downloadMessage, progress);
        }
    }
}
