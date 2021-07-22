using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : ObjectHoldManager
{ 
    public float aimRecoilDecrease = 0.5f;
    private bool holstered = false;
    public float deployShootDelay = 1;
    private float timeDeployed;

    private Vector3 aimPosition;
    private float recoil;
    private float recRot;

    private Weapon activeWeapon = null;
    private Weapon secondaryWeapon = null;
    
    public delegate void ShotFired(Transform pos);
    public static event ShotFired OnShotFired;

    void Update()
    {
        if (hud.isPaused || hud.isMenu)
            return;

        if (activeWeapon)
        {
            if (!holstered)
            {
                HoldWeapon();

                if (activeWeapon.ranged && Input.GetButton(InputManager.Aim))
                {
                    playerController.SetSprinting(false);

                    holdPosition = aimPosition;
                    wantedRotation = Quaternion.identity;
                    maxSwayAmount = maxAimSwayAmount;
                }
                else
                {
                    if (playerController.IsSprinting())
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

                if (activeWeapon.ranged && Input.GetButtonDown(InputManager.Aim))
                {
                    hud.FadeRetical(true);
                    playerController.SetAiming(true);
                }

                if (activeWeapon.ranged && Input.GetButtonUp(InputManager.Aim))
                {
                    hud.FadeRetical(false);
                    playerController.SetAiming(false);
                }

                if (activeWeapon.ranged && Time.time > (timeDeployed + deployShootDelay))
                {

                    Firearm fa = activeWeapon.gameObject.GetComponent<Firearm>();

                    if (Input.GetButtonDown(InputManager.Reload))
                    {
                        playerController.SetSprinting(false);

                        //Relaod weapon
                        fa.Reload();
                    }

                    if (fa.auto == true ? Input.GetButton(InputManager.Shoot) : Input.GetButtonDown(InputManager.Shoot))
                    {
                        playerController.SetSprinting(false);

                        if (fa.Shoot())
                        {
                            ApplyRecoil(recoil, -recRot);

                            if (OnShotFired != null)
                                OnShotFired(transform);
                        }
                    }
                }                
            }
            
        }
    }

    void FixedUpdate()
    {
        if (hud.isPaused || hud.isMenu)
            return;

        if (activeWeapon)
        {
            activeWeapon.transform.localPosition = Vector3.Lerp(activeWeapon.transform.localPosition, holdPosition, smoothing * Time.fixedDeltaTime);
            activeWeapon.transform.localRotation = Quaternion.Slerp(activeWeapon.transform.localRotation, wantedRotation, smoothing * Time.fixedDeltaTime);
        }
    }

    void HoldWeapon()
    {
        //Position Sway
        float lookX = -Input.GetAxis(InputManager.MouseX);
        float lookY = -Input.GetAxis(InputManager.MouseY);
        lookX = Mathf.Clamp(lookX, -maxSwayAmount, maxSwayAmount);
        lookY = Mathf.Clamp(lookY, -maxSwayAmount, maxSwayAmount);

        Vector3 swayPosition = new Vector3(lookX, lookY, 0);

        activeWeapon.transform.localPosition = Vector3.Lerp(activeWeapon.transform.localPosition, swayPosition + holdPosition, swaySmoothing * Time.deltaTime);
    }

    public void ApplyRecoil(float recoilAmount, float rotationAmount)
    {

        float rec = recoilAmount;
        float rot = rotationAmount;

        if (Input.GetButton(InputManager.Aim))
        {
            rec *= aimRecoilDecrease;
            rot *= aimRecoilDecrease;
        }

        //Kickback
        Vector3 recoil = holdPosition;
        recoil.z -= rec;

        activeWeapon.transform.localPosition = recoil;

        //Rotation
        activeWeapon.transform.localRotation = Quaternion.Euler(rot, 0, 0);
    }

    void DeployWeapon()
    {
        if (activeWeapon)
        {
            activeWeapon.gameObject.layer = 8;

            foreach (Transform child in activeWeapon.gameObject.transform)
            {
                child.gameObject.layer = 8;
            }

            activeWeapon.transform.SetParent(weaponParent);

            hipPosition = activeWeapon.holdPosition;
            aimPosition = activeWeapon.aimPosition;
            holsterPosition = activeWeapon.holsterPosition;
            sprintPosition = activeWeapon.sprintPosition;
            sprintRotation = Quaternion.Euler(activeWeapon.sprintRotation);
            

            if (activeWeapon.ranged)
            {
                Firearm fa = activeWeapon.gameObject.GetComponent<Firearm>();
                recoil = fa.recoil;
                recRot = fa.recRot;
            }

            //Deploying animations
            activeWeapon.transform.localPosition = holdPosition;

            holstered = false;
            holdPosition = hipPosition;

            timeDeployed = Time.time;
        }
    }

    public bool EquipWeapon(Weapon newWeapon, bool activate)
    {
        if (!activate)
        {
            if (!activeWeapon)
            {
                //has no active weapon, pick it up and equip

                activeWeapon = newWeapon;
                activeWeapon.Equip(true);
                
                DeployWeapon();

                return true;
            } else if (!secondaryWeapon)
            {
                //has no secondary weapon, equip to secondary weapon
                secondaryWeapon = newWeapon;
                Destroy(newWeapon);

                return true;
            } else
            {
                return false;
            }
        } else
        {
            //Replace active weapon and send active weapon to inventory

            //inventory.AddWeapons(activeWeapon);
            activeWeapon = newWeapon;

            DeployWeapon();

            return true;
        }

    }

    public void HolsterWeapon(bool holst)
    {
        if (activeWeapon)
        {
            if (holst)
            {
                holstered = true;

                //This is a placeholder, create holstering animations later
                holdPosition = holsterPosition;
            }
            else
            {
                holstered = false;

                //This is a placeholder, create holstering animations later
                holdPosition = hipPosition;

                timeDeployed = Time.time;
            }
        }
    }

    public bool AddAmmo(int amount)
    {
        if (!HasWeapon())
            return false;

        return activeWeapon.AddAmmo(amount);
    }

    public bool HasWeapon()
    {
        if (activeWeapon != null || secondaryWeapon != null)
        {
            return true;
        } else
        {
            return false;
        }
    }

}
