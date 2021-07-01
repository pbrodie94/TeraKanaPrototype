using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTargetingGun : Tool
{
    public float maxLaserDistance = 1000;

    public GameObject TargetLocator;
    private Transform targetLocatorDisplay;

    public GameObject missile;

    private Transform muzzle;
    private LineRenderer laser;
    private Transform cam;

    protected override void Start()
    {
        base.Start();

        muzzle = transform.Find("Muzzle");
        laser = muzzle.GetComponentInChildren<LineRenderer>();
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        //Only show laser when aim button pressed?
        if (equipped)
        {
            laser.enabled = true;

            RaycastHit hit;

            if (Physics.Raycast(muzzle.position, cam.forward, out hit, maxLaserDistance))
            {
                if (hit.collider.tag != "Enemy")
                {
                    if (!targetLocatorDisplay)
                        targetLocatorDisplay = Instantiate(TargetLocator, hit.point, Quaternion.identity).transform;

                    targetLocatorDisplay.position = hit.point;
                }
                else
                {
                    if (targetLocatorDisplay)
                        Destroy(targetLocatorDisplay.gameObject);
                }

                float dist = Vector3.Distance(muzzle.position, hit.point);

                if (dist <= maxLaserDistance)
                {
                    laser.SetPosition(1, new Vector3(0, 0, dist));
                } else
                {
                    laser.SetPosition(1, new Vector3(0, 0, maxLaserDistance));
                }
            }

            //Display laser

            if (Input.GetButtonDown(InputManager.Shoot))
            {
                fired = true;

                //Call air support
                MissileStrike(targetLocatorDisplay);

                //Dequip laser targeting gun and redraw weapon
                tm.DequipTool();
            }
        } else
        {
            laser.enabled = false;
        }
    }

    public override void DestroyDisplayElements()
    {
        if (targetLocatorDisplay && !fired)
            Destroy(targetLocatorDisplay.gameObject);
    }

    void MissileStrike(Transform target)
    {
        //Set position above target
        Vector3 pos = target.position;
        pos.y += 1000;

        Vector3 wantedLook = pos - target.position;
        Quaternion rot = Quaternion.LookRotation(wantedLook);

        //Spawn missile
        GameObject go = Instantiate(missile, pos, rot);
        Missile m = go.GetComponent<Missile>();

        m.target = target;
    }
}
