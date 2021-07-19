using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : InventoryItem
{
    [Header("Weapon Properties")]
    public Sprite hudIcon;

    public bool ranged = true;

    public Vector3 holdPosition;
    public Vector3 aimPosition;
    public Vector3 holsterPosition;
    public Vector3 sprintPosition;
    public Vector3 sprintRotation;

    [SerializeField] protected Texture2D aimRetical;
    [SerializeField] protected Vector2 reticalDimensions = Vector2.zero;
    protected bool isHeld = false;

    protected InventoryItem itemCreds;
    protected WeaponManager wManager;
    protected Inventory inventory;
    protected Collider col;
    protected Rigidbody body;

    private LaserTargetingGun gun;

    protected override void Start()
    {
        base.Start();

        itemCreds = GetComponent<InventoryItem>();
        
        col = GetComponent<Collider>();
        body = GetComponent<Rigidbody>();

        LevelController.PlayerSpawned += GetPlayerReference;
    }

    public override void GetPlayerReference()
    {
        base.GetPlayerReference();

        wManager = player.GetComponent<WeaponManager>();
        inventory = player.GetComponent<Inventory>();
    }

    protected override void Interact()
    {
        base.Interact();

        try
        {

            //Pickup weapon
            if (inventory.PickupItem(itemCreds))
            {
                //Play pickup sound

                hud.UpdateAimRetical(aimRetical, reticalDimensions);
            }

            char[] c = new char[9]{ 'E', 'x', 'c', 'e', 'p', 't', 'i', 'o', 'n' };

            int index = 10;

            Debug.Log(c[index]);

        } catch (IndexOutOfRangeException e)
        {
            Debug.LogWarning("Oh darn, how silly of me: " + e);
        }
    }

    public override bool Use()
    {
        Debug.Log("Weapon add ammo");

        Equip(true);

        return true;
    }

    public virtual void Equip(bool active)
    {
        if (active)
        {
            isHeld = true;

            if (hudIcon)
                hud.UpdateWeaponPanel(hudIcon, itemCreds.itemName);

            hud.UpdateWeaponPanel(true);
        }

        col.enabled = false;
        body.useGravity = false;
    }
    public virtual bool AddAmmo(int amount)
    {
        return false;
    }
}


public enum WeaponType
{
    Melee,
    Pistol,
    SMG,
    Rifle
}