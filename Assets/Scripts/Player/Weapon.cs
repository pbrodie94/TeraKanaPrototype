using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] protected Texture2D aimReticle;
    [SerializeField] protected Vector2 reticleDimensions = Vector2.zero;
    protected bool isHeld = false;

    protected InventoryItem itemCreds;
    protected WeaponManager wManager;
    protected Inventory inventory;
    protected Collider col;
    protected Rigidbody body;

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
        if (!inventory)
        {
            inventory = player.GetComponent<Inventory>();
        }
    }

    protected override void Interact()
    {
        base.Interact();

        //Pickup weapon
        if (inventory.PickupItem(itemCreds))
        {
            //Play pickup sound
            if (audioSource && interactSound)
                audioSource.PlayOneShot(interactSound);

            hud.UpdateAimReticle(aimReticle, reticleDimensions);
        }
    }

    public void PickupWeapon(GameObject player)
    {
        //Pickup weapon
        inventory = player.GetComponent<Inventory>();
        if (!itemCreds)
            itemCreds = GetComponent<InventoryItem>();
        if (inventory.PickupItem(itemCreds))
        {
            HUDManager.instance.UpdateAimReticle(aimReticle, reticleDimensions);
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
                HUDManager.instance.UpdateWeaponPanel(hudIcon, itemCreds.itemName);

            HUDManager.instance.UpdateWeaponPanel(true);
        }

        if (!col)
        {
            col = GetComponent<Collider>();
        }
        col.enabled = false;
        if (!body)
        {
            body = GetComponent<Rigidbody>();
        }
        body.useGravity = false;
        body.isKinematic = true;
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