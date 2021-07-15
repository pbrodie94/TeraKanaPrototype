using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvas : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        LevelController.PlayerSpawned += GetPlayerReference;
    }

    public void GetPlayerReference()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (cam == null)
            return;

        Vector3 lookDir = transform.position - cam.position;
        Quaternion dir = Quaternion.LookRotation(lookDir);
        dir.x = 0;
        dir.z = 0;
        transform.rotation = dir;
    }
}
