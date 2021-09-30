using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamLook : MonoBehaviour
{
    [Header("Preferences")]
    public float lookSensitivityX = 5;
    public float lookSensitivityY = 5;
    public float lookSmoothing = 1;
    float xRotation = 0;
    public float maxY = 90;
    public float minY = -90;

    private Vector2 targetDirection;

    Transform playerTrans;
    PlayerMove player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (GameManager.instance && GameManager.instance.IsPaused())
            return;

        targetDirection.x = -Input.GetAxisRaw(InputManager.MouseY) * lookSensitivityX;
        targetDirection.y = Input.GetAxisRaw(InputManager.MouseX) * lookSensitivityY;

        GetLook();
    }

    void GetLook()
    {
        xRotation += targetDirection.x;
        xRotation = Mathf.Clamp(xRotation, minY, maxY);

        playerTrans.Rotate(Vector3.up * targetDirection.y);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
