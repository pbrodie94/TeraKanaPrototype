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

    private bool reloading = false;
    private float timeBeganReloading;
    private static readonly int Reload = Animator.StringToHash("Reload");

    public delegate void ShotFired(Transform pos);
    public static event ShotFired OnShotFired;
    
    

    void Update()
    {
        if (GameManager.instance.IsPaused())
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

                if (activeWeapon.ranged && Input.GetButtonDown(InputManager.Aim) && !reloading)
                {
                    HUDManager.instance.FadeRetical(true);
                    playerController.SetAiming(true);
                }

                if (activeWeapon.ranged && Input.GetButtonUp(InputManager.Aim))
                {
                    HUDManager.instance.FadeRetical(false);
                    playerController.SetAiming(false);
                }

                if (activeWeapon.ranged && Time.time > (timeDeployed + deployShootDelay) && !reloading)
                {

                    Firearm fa = activeWeapon.gameObject.GetComponent<Firearm>();

                    if (Input.GetButtonDown(InputManager.Reload) && fa.CanReload())
                    {
                        playerController.SetSprinting(false);
                        HUDManager.instance.FadeRetical(false);
                        playerController.SetAiming(false);
                        
                        anim.SetTrigger(Reload);
                        reloading = true;

                        timeBeganReloading = Time.time;
                    }

                    if (fa.auto == true ? Input.GetButton(InputManager.Shoot) : Input.GetButtonDown(InputManager.Shoot) && !reloading)
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

            if (Input.GetAxis(InputManager.WeaponScroll) > 0)
            {
                if (!secondaryWeapon)
                {
                    return;
                }

                //Next Weapon
                SwapWeapon();
            }

            if (Input.GetAxis(InputManager.WeaponScroll) < 0)
            {
                if (!secondaryWeapon)
                {
                    return;
                }
                
                //Previous weapon
                SwapWeapon();
            }
            
        }

        if (reloading && Time.time >= timeBeganReloading + (3 * 0.0167f))
        {
            reloading = false;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.instance.IsPaused())
            return;

        if (activeWeapon)
        {
            Transform weapon = activeWeapon.transform;
            
            weapon.localPosition = Vector3.Lerp(weapon.localPosition, holdPosition, smoothing * Time.fixedDeltaTime);
            weapon.localRotation = Quaternion.Slerp(weapon.localRotation, wantedRotation, smoothing * Time.fixedDeltaTime);
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
        if (!activeWeapon)
        {
            return;
        }

        GameObject weapon = activeWeapon.gameObject;
        
        if (!weapon.activeSelf)
        {
            weapon.SetActive(true);
        }

        weapon.layer = 8;

        foreach (Transform child in weapon.transform)
        {
            child.gameObject.layer = 8;
        }

        weapon.transform.SetParent(weaponParent);

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
        weapon.transform.localPosition = holdPosition;

        holstered = false;
        holdPosition = hipPosition;

        timeDeployed = Time.time;
        
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
            } 
            
            if (!secondaryWeapon)
            {

                //has no secondary weapon, equip to secondary weapon
                secondaryWeapon = newWeapon;
                secondaryWeapon.Equip(false);

                Transform weapon = secondaryWeapon.gameObject.transform;
                
                weapon.SetParent(transform);
                weapon.localPosition = secondaryWeapon.holsterPosition;
                weapon.gameObject.SetActive(false);

                return true;
            }
            
            return false;
            
        } else {
            
            //Replace active weapon and send active weapon to inventory

            //inventory.AddWeapons(activeWeapon);
            activeWeapon = newWeapon;

            DeployWeapon();

            return true;
        }

    }

    private void SwapWeapon()
    {
        //Do a safety check
        if (!activeWeapon || !secondaryWeapon)
        {
            return;
        }
        
        //Swap variables
        (activeWeapon, secondaryWeapon) = (secondaryWeapon, activeWeapon);

        //Activate and deploy the swapped weapon
        activeWeapon.Equip(true);
        DeployWeapon();
        
        //Set the previous weapon in the secondary slot then deactivate it
        Transform weapon = secondaryWeapon.gameObject.transform;
        weapon.SetParent(transform);
        weapon.localPosition = secondaryWeapon.holsterPosition;
        weapon.gameObject.SetActive(false);
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

    public void ReloadUpdateAmmo()
    {
        Firearm fa = activeWeapon.gameObject.GetComponent<Firearm>();

        fa.Reload();
    }

    public void FinishedReload()
    {
        reloading = false;
    }

    public bool IsReloading()
    {
        return reloading;
    }

    public Weapon GetPrimaryWeapon()
    {
        return activeWeapon;
    }

    public Weapon GetSecondaryWeapon()
    {
        return secondaryWeapon;
    }
}
