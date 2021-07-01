using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : ObjectHoldManager
{
    private Tool activeTool;

    public GameObject laserTarget;

    private WeaponManager wManager;

    protected override void Start()
    {
        base.Start();

        wManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        if (Input.GetButtonDown(InputManager.Interact))
        {
            if (activeTool)
            {
                DequipTool();
            } else
            {
                EquipTool(laserTarget);
            }
        }

        if (activeTool)
        {
            if (!activeTool.holstered)
            {
                HoldTool();

                if (playerControl.sprinting)
                {
                    holdPosition = sprintPosition;
                    wantedRotation = sprintRotation;
                }
                else
                {
                    holdPosition = hipPosition;
                    maxSwayAmount = maxHipSwayAmount;
                    wantedRotation = Quaternion.identity;
                }
            }

            activeTool.transform.localPosition = Vector3.Lerp(activeTool.transform.localPosition, holdPosition, smoothing * Time.deltaTime);
            activeTool.transform.localRotation = Quaternion.Lerp(activeTool.transform.localRotation, wantedRotation, smoothing * Time.deltaTime);
        }
    }

    void EquipTool(GameObject tool)
    {
        wManager.HolsterWeapon(true);

        GameObject go = Instantiate(tool.gameObject, transform.position, Quaternion.Euler(90, 0, 0));
        activeTool = go.GetComponent<Tool>();
        activeTool.transform.parent = cam.transform;
        activeTool.holstered = false;

        hipPosition = activeTool.hipPosition;
        holsterPosition = activeTool.holsterPosition;
        sprintPosition = activeTool.sprintPosition;

        activeTool.transform.localPosition = holsterPosition;

        holdPosition = hipPosition;

        activeTool.equipped = true;
    }

    public void DequipTool()
    {
        holdPosition = holsterPosition;
        activeTool.holstered = true;
        activeTool.equipped = false;

        activeTool.DestroyDisplayElements();

        Destroy(activeTool.gameObject, 1);

        wManager.HolsterWeapon(false);
    }

    void HoldTool()
    {
        //Position Sway
        float lookX = -Input.GetAxis(InputManager.MouseX);
        float lookY = -Input.GetAxis(InputManager.MouseY);
        lookX = Mathf.Clamp(lookX, -maxSwayAmount, maxSwayAmount);
        lookY = Mathf.Clamp(lookY, -maxSwayAmount, maxSwayAmount);

        Vector3 swayPosition = new Vector3(lookX, lookY, 0);

        activeTool.transform.localPosition = Vector3.Lerp(activeTool.transform.localPosition, swayPosition + holdPosition, swaySmoothing * Time.deltaTime);
    }
}
