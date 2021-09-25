using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmScan : BioScan
{
    public delegate void ScanCompletedAction(AlarmScan scan);
    public event ScanCompletedAction OnScanCompleted;

    protected override void Start()
    {
        base.Start();

        scanMessage = "Scan to unlock: ";
    }

    protected override void ScanComplete()
    {
        _scanComplete = true;
        HUDManager.instance.HideProgressBar();
        
        if (OnScanCompleted != null)
        {
            OnScanCompleted(this);
        }

        Destroy(gameObject);
    }
}
