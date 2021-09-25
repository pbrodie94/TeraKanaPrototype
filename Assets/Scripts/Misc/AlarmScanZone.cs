using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmScanZone : MonoBehaviour
{
    private List<Collider> zones = new List<Collider>();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            Collider col = child.GetComponent<Collider>();
            zones.Add(col);
        }
    }

    public Vector3 GetScanLocation()
    {
        if (zones.Count <= 0)
        {
            return Vector3.zero;
        }
        
        Vector3 position = Vector3.zero;
        int index = Random.Range(0, zones.Count - 1);
        Bounds bounds = zones[index].bounds;

        position = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));
        
        //Raycast to find ground
        float maxDist = bounds.min.y + bounds.max.y;
        RaycastHit[] hit = Physics.RaycastAll(position, Vector3.down, maxDist);
        for (int i = 0; i < hit.Length; ++i)
        {
            if (hit[i].collider.CompareTag("Ground"))
            {
                position.y = hit[i].point.y;
                break;
            }
        }

        return position;
    }

    public Vector3 GetScanLocationAvoidingOverlap(List<Vector3> existingPositions, float buffer)
    {
        //If there are no positions passed in, or buffer is 0, just return a point
        if (existingPositions.Count <= 0 || buffer <= 0)
        {
            return GetScanLocation();
        }

        //Get a point, and iterate through existing points checking if they are far enough away
        //If far enough, return the location
        bool overlapped = false;
        Vector3 position = Vector3.zero;
        do
        {
            position = GetScanLocation();

            foreach (Vector3 otherPosition in existingPositions)
            {
                float dist = Vector3.Distance(position, otherPosition);

                if (dist < buffer)
                {
                    overlapped = true;
                }
                else
                {
                    overlapped = false;
                }
            }

        } while (overlapped);

        return position;
    }
}
