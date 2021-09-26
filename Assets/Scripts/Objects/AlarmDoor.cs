using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmDoor : Door
{
    [Header("Alarm Settings")]
    private bool isAlarmDectivated = false;
    [Range(2, 8)]
    [SerializeField] private int alarmClass = 2;
    [SerializeField] private AlarmScanZone scanZone;
    private int spawnedScans = 0;
    private int scansCompleted = 0;
    private int scanClassCompleted = 0;
    private bool alarmSequenceInitiated = false;

    [Header("Bioscan References")] 
    [SerializeField] private GameObject smallBioScan;
    [SerializeField] private GameObject largeBioScan;
    [SerializeField] private GameObject startScanLocation;
    [SerializeField] private GameObject lineRendererObject;
    private List<ScanLineRenderer> scanLines = new List<ScanLineRenderer>();
    private Vector3 lineOrigin = Vector3.zero;

    protected override void Start()
    {
        base.Start();

        if (!scanZone)
        {
            isAlarmDectivated = true;
            SetInteractionMessage();
        }
    }

    //Takes in an object to set the scan zone reference
    public void SetScanZone(GameObject scanZoneObject)
    {
        if (opened || scanClassCompleted >= alarmClass)
        {
            return;
        }
        
        //Try and get a reference to the Alarm scan zone object and set if not null
        AlarmScanZone zone = scanZoneObject.GetComponent<AlarmScanZone>();
        if (zone != null)
        {
            scanZone = zone;
        }

        //If scan zone is null, deactivate alarm door status
        if (!scanZone)
        {
            isAlarmDectivated = true;
            SetInteractionMessage();
            return;
        }

        isAlarmDectivated = false;
        SetInteractionMessage();
    }

    protected override void DoorInteraction()
    {
        if (locked || isAlarmDectivated)
        {
            base.DoorInteraction();
            return;
        }

        //If alarm sequence is initiated, do nothing
        if (alarmSequenceInitiated)
        {
            return;
        }

        //Begin alarm sequence
        HUDManager.instance.AddNotification("Warning! Alarm Detected!", Color.red);
        alarmSequenceInitiated = true;
        SetInteractionMessage();
        
        //Play alarm sound
        
        //Begin spawning alarm waves every 1 - 2 minutes until alarm deactivated
        LevelController.instance.GetEnemySpawner().BeginAlarmSequence();
        
        //Spawn scan at door
        GameObject scanObject = Instantiate(largeBioScan, startScanLocation.transform.position,
            startScanLocation.transform.rotation);

        //Set to layer 8 (Weapons) so it can be seen through walls by the gun camera
        scanObject.layer = 8;

        //Subscribe OnScanCompleted to CompletedScan event
        AlarmScan scan = scanObject.GetComponent<AlarmScan>();
        scan.OnScanCompleted += OnScanCompleted;

        scansCompleted = 0;
        spawnedScans = 1;

        lineOrigin = scanObject.transform.position;
    }

    private void OnScanCompleted(AlarmScan alarmBioScan)
    {
        //Unsubscribe from event
        alarmBioScan.OnScanCompleted -= OnScanCompleted;

        //Increment number of completed scans and check if set of scans is completed
        ++scansCompleted;
        if (scansCompleted < spawnedScans)
        {
            return;
        }
        
        ++scanClassCompleted;
        
        //Delete old scan lines
        for (int i = scanLines.Count - 1; i > -1; --i)
        {
            Destroy(scanLines[i].gameObject);
        }
        scanLines.Clear();
        
        //Check if all scans have been completed
        if (scanClassCompleted >= alarmClass)
        {
            //Stop alarm sound

            //Deactivate alarm door status
            LevelController.instance.GetEnemySpawner().EndAlarmSequence();
            isAlarmDectivated = true;
            SetInteractionMessage();
            
            HUDManager.instance.AddNotification("Scan Complete! Door unlocked!", Color.green);
            
            return;
        }

        //Spawn another set of scans
        SpawnScans();
    }

    private void SpawnScans()
    {
        List<Vector3> existingPositions = new List<Vector3>();
        for (int i = 0; i < 4; ++i)
        {
            Vector3 scanPosition = Vector3.zero;
            if (existingPositions.Count <= 0)
            {
                scanPosition = scanZone.GetScanLocation();
            }
            else
            {
                scanPosition = scanZone.GetScanLocationAvoidingOverlap(existingPositions, 2);
            }
            
            existingPositions.Add(scanPosition);

            GameObject scanObject = Instantiate(smallBioScan, scanPosition, Quaternion.identity);
            scanObject.layer = 8;
            
            //Subscribe scan completed to OnScanCompleted
            AlarmScan scan = scanObject.GetComponent<AlarmScan>();
            scan.OnScanCompleted += OnScanCompleted;

            ScanLineRenderer scanLine = Instantiate(lineRendererObject, lineOrigin, Quaternion.identity)
                .GetComponent<ScanLineRenderer>();
            
            scanLine.SetupLine(lineOrigin, scanObject);
            scanLines.Add(scanLine);
            
            scanObject.SetActive(false);
        }

        //Select new starting point
        int existingPointIndex = Random.Range(0, existingPositions.Count - 1);
        lineOrigin = existingPositions[existingPointIndex];

        //Line renderers begin moving
        foreach (ScanLineRenderer line in scanLines)
        {
            line.MoveToDestination();
        }

        scansCompleted = 0;
        spawnedScans = 4;
    }
    
    protected override void SetInteractionMessage()
    {
        if (isAlarmDectivated || locked)
        {
            base.SetInteractionMessage();
            return;
        }

        if (alarmSequenceInitiated)
        {
            message = "Complete the scans to unlock the door";
            return;
        }
        
        string classNumerals = "II";
        switch (alarmClass)
        {
            case 2:
                classNumerals = "II";
                break;
            case 3:
                classNumerals = "III";
                break;
            case 4:
                classNumerals = "IV";
                break;
            case 5:
                classNumerals = "V";
                break;
            case 6:
                classNumerals = "VI";
                break;
            case 7:
                classNumerals = "VII";
                break;
            case 8:
                classNumerals = "VIII";
                break;
        }

        message = "Class " + classNumerals + " Alarm Detected!";
    }
}
