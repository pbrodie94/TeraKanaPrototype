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

    [SerializeField] protected Texture2D aimRetical;
    [SerializeField] protected Vector2 reticalDimensions = Vector2.zero;
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
        inventory = player.GetComponent<Inventory>();
    }

    protected override void Interact()
    {
        base.Interact();

        //Pickup weapon
        if (inventory.PickupItem(itemCreds))
        {
            //Play pickup sound

            hud.UpdateAimRetical(aimRetical, reticalDimensions);
        }
    }

    public virtual void Equip(bool active)
    {
        if (active)
        {
            isHeld = true;

            if (hudIcon)
                hud.UpdateWeaponPanel(hudIcon, itemCreds.Name);

            hud.UpdateWeaponPanel(true);
        }

        col.enabled = false;
        body.useGravity = false;
    }
}

public enum WeaponType
{
    Melee,
    Pistol,
    SMG,
    Rifle
}