using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvas : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Vector3 lookDir = transform.position - cam.position;
        Quaternion dir = Quaternion.LookRotation(lookDir);
        dir.x = 0;
        dir.z = 0;
        transform.rotation = dir;
    }
}
