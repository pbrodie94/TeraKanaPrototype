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

    [SerializeField] protected float pickupDistance = 10;
    [SerializeField] protected Texture2D aimRetical;
    [SerializeField] protected Vector2 reticalDimensions = Vector2.zero;
    protected bool isHeld = false;

    protected InventoryItem itemCreds;
    protected GameObject player;
    protected WeaponManager wManager;
    protected Inventory inventory;
    protected HUDManager hud;
    protected Transform cam;
    protected Collider col;
    protected Rigidbody body;

    protected virtual void Start()
    {
        itemCreds = GetComponent<InventoryItem>();
        player = GameObject.FindGameObjectWithTag("Player");
        wManager = player.GetComponent<WeaponManager>();
        inventory = player.GetComponent<Inventory>();
        cam = Camera.main.transform;
        col = GetComponent<Collider>();
        body = GetComponent<Rigidbody>();
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();
    }

    private void LateUpdate()
    {
        if (!isHeld)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position); 

            if (dist <= pickupDistance)
            {
                RaycastHit hit;

                if (Physics.Raycast(cam.position, cam.forward, out hit))
                {

                    if (hit.collider.gameObject == gameObject)
                    {
                        //Player is within range to pickup weapon, and looking at it

                        //Display pickup message

                        string msg = "Press 'F' to pick up " + itemCreds.Name;
                        hud.ShowMessage(msg, true);

                        if (Input.GetButtonDown(InputManager.Action))
                        {
                            //Pickup weapon
                            if (inventory.PickupItem(itemCreds))
                            {
                                hud.ShowMessage(null, false);

                                //Play pickup sound

                                hud.UpdateAimRetical(aimRetical, reticalDimensions);
                            }
                        }
                    }
                    else
                    {
                        hud.ShowMessage(null, false);
                    }
                }

            } else
            {
                hud.ShowMessage(null, false);
            }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, pickupDistance);
    }
}

public enum WeaponType
{
    Melee,
    Pistol,
    SMG,
    Rifle
}