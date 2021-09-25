using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionPoint : BioScan
{
    protected override void Start()
    {
        base.Start();

        scanMessage = "Stand by for Extraction: ";
    }

    protected override void ScanComplete()
    {
        //Show the drop complete screen
        if (!_scanComplete)
        {
            StartCoroutine(OnScanComplete());
        }
    }

    protected IEnumerator OnScanComplete()
    {
        yield return new WaitForSeconds(1);

        HUDManager.instance.GameOver(true);
    }
}
