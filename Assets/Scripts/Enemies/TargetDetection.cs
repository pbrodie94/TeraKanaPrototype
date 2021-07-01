using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetection : MonoBehaviour
{
    [Header("Attributes")]
    public float detectionRadius = 20;
    public float audioDetectionRadius = 50;
    public float fov = 80;

    private bool soundDetected = false;
    private float targetDistance;

    public Transform detectionSensor;

    private void Start()
    {
        if (!detectionSensor)
            detectionSensor = transform;
    }

    private void OnEnable()
    {
        WeaponManager.OnShotFired += DetectSound;
    }

    private void OnDisable()
    {
        WeaponManager.OnShotFired -= DetectSound;
    }

    public bool DetectTarget(Transform target)
    {
        targetDistance = Vector3.Distance(transform.position, target.position);

        if (targetDistance <= detectionRadius)
        {
            //Target is in detection range
            Vector3 targetDirection = target.position - detectionSensor.position;
            float targetAngle = Vector3.Angle(transform.forward, targetDirection);

            if (targetAngle <= fov / 2)
            {
                //Target is in field of view
                RaycastHit hit;

                if (Physics.Raycast(detectionSensor.position, targetDirection, out hit, detectionRadius))
                {
                    if (hit.collider.gameObject == target.gameObject)
                    {
                        //Player is unobstructed, and in view
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool SoundDetected()
    {
        if (soundDetected)
        {
            soundDetected = false;
            return true;
        }

        return false;
    }

    public void DetectSound(Transform pos)
    {
        float dist = Vector3.Distance(transform.position, pos.position);

        if (dist <= audioDetectionRadius)
        {
            //Heard sound
            soundDetected = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Draw detection radius sphere
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        //Draw audio detection radius sphere
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, audioDetectionRadius);

        //Draw field of view lines
        Quaternion fovLine1 = Quaternion.AngleAxis(-fov / 2, Vector3.up);
        Quaternion fovLine2 = Quaternion.AngleAxis(fov / 2, Vector3.up);
        Vector3 rayDir1 = fovLine1 * transform.forward;
        Vector3 rayDir2 = fovLine2 * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(detectionSensor.position, rayDir1 * detectionRadius);
        Gizmos.DrawRay(detectionSensor.position, rayDir2 * detectionRadius);

        Vector3 dir = GameObject.FindGameObjectWithTag("Player").transform.position - detectionSensor.position;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(detectionSensor.position, dir);
    }
}
