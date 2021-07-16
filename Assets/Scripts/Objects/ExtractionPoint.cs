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

    protected override IEnumerator OnScanComplete()
    {
        yield return new WaitForSeconds(1);

        ScanComplete();
    }

    protected virtual void ScanComplete()
    {

    }
}
