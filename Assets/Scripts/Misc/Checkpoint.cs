using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isCurrentCheckpoint = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCurrentCheckpoint)
        {
            //Update checkpoint
            LevelController.instance.UpdateCheckpoint(this);
        }
    }

    public void SetCurrentCheckpoint(bool current)
    {
        isCurrentCheckpoint = current;
    }
}
