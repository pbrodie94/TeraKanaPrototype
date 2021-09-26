using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanLineRenderer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float bufferDistance = 3;
    private LineRenderer lr;
    private GameObject destination;
    private Vector3 currentEndPosition = Vector3.zero;
    private bool arrivedAtDestination = false;
    private bool beginMoving = false;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();

        if (bufferDistance <= 0)
        {
            bufferDistance = 3;
        }
    }

    public void SetupLine(Vector3 origin, GameObject goal)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, origin);
        lr.SetPosition(1, origin);

        currentEndPosition = origin;

        destination = goal;
    }
    
    public void SetupLine(Vector3 origin, GameObject goal, float buffer)
    {
        bufferDistance = buffer;
        SetupLine(origin, goal);
    }

    public void MoveToDestination()
    {
        beginMoving = true;
    }

    private void LateUpdate()
    {
        if (arrivedAtDestination || !beginMoving)
        {
            return;
        }
        
        float distanceToDestination = Vector3.Distance(currentEndPosition, destination.transform.position);
        if (distanceToDestination <= bufferDistance)
        {
            //Gotten close enough, activate the scan
            destination.SetActive(true);
            arrivedAtDestination = true;
            return;
        }

        Vector3 direction = destination.transform.position - currentEndPosition;
        direction.Normalize();

        currentEndPosition += direction * moveSpeed * Time.fixedDeltaTime;
        lr.SetPosition(1, currentEndPosition);
    }
}
