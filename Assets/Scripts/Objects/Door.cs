using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : MonoBehaviour
{
    [SerializeField] protected float interactRange = 3;

    protected string message;
    private bool messageShown = false;
    private bool opened = false;
    public bool isOpened
    {
        get { return opened; }
    }
    protected bool canClose = true;

    [SerializeField] private bool locked = false;
    private InteractionItem requiredKey = null;
    public bool isLocked
    {
        get
        {
            return locked;
        }
    }

    [SerializeField] protected GameObject[] controlBox;

    protected Transform player;
    protected Inventory playerInventory;
    protected Transform cam;
    
    protected Animation doorAnimation;

    protected virtual void Start()
    {
        doorAnimation = GetComponentInChildren<Animation>();

        message = locked ? message :  "Press 'E' to open door.";
        messageShown = false;

        LevelController.PlayerSpawned += GetPlayerReference;
    }

    public void GetPlayerReference()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = Camera.main.transform;
        playerInventory = player.gameObject.GetComponent<Inventory>();

        if (locked && requiredKey == null)
        {
            locked = false;
            message = "Press 'E' to open door.";
        }
    }

    private void Update()
    {
        if (!player || !cam)
            return;

        float dist = Vector3.Distance(controlBox[0].transform.position, player.position);
        float dist2 = interactRange + 1;

        if (controlBox.Length > 1)
        {
            dist2 = Vector3.Distance(controlBox[1].transform.position, player.position);
        }

        if (dist <= interactRange || dist2 <= interactRange)
        {
            RaycastHit hit;

            if (Physics.Raycast(cam.position, cam.forward, out hit, interactRange + 1))
            {

                if (hit.collider.gameObject == controlBox[0] || hit.collider.gameObject == controlBox[1])
                {
                    HUDManager.instance.ShowMessage(message, true);
                    messageShown = true;

                    if (!playerInventory.IsHoldingItem())
                    {
                        if (Input.GetButtonDown(InputManager.Action))
                        {
                            DoorInteraction();
                        }
                    }
                    else
                    {
                        if (Input.GetButtonDown(InputManager.Action) || Input.GetButtonDown(InputManager.Shoot))
                        {
                            playerInventory.UseInterractItem(UnlockDoor(playerInventory.GetHeldItem()));
                        }
                    }
                }
                else
                {
                    if (messageShown)
                    {
                        HUDManager.instance.ShowMessage(null, false);
                        messageShown = false;
                    }
                }
            }
        }
        else
        {
            if (messageShown)
            {
                HUDManager.instance.ShowMessage(null, false);
                messageShown = false;
            }
        }
    }

    protected virtual void DoorInteraction()
    {
        if (locked)
        {
            HUDManager.instance.AddNotification("Door locked, go find" + requiredKey.itemName, Color.red);

            return;
        }
        
        //Open door
        if (!opened)
        {
            doorAnimation.Play("open");
            message = "Press 'E' to close door";
            opened = true;

        } else if (opened && canClose)
        {
            doorAnimation.Play("close");
            message = "Press 'E' to open door.";
            opened = false;
        }
    }

    public bool UnlockDoor(InteractionItem key)
    {
        if (key != requiredKey)
        {
            HUDManager.instance.AddNotification("Incorrect key", Color.red);
            return false;
        }
        
        locked = false;
        HUDManager.instance.AddNotification("Unlocked door", Color.cyan);

        message = "Press 'E' to open door.";

        return true;
    }

    public void SetUnlockKey(InteractionItem key)
    {
        requiredKey = key;

        message = "Door locked, go find " + key.itemName + ".";
        
        Debug.Log(message);
    }

    public void SetDoorData(bool opened, bool locked)
    {
        //If locked, set door being locked, then return since if locked it won't be open
        if (locked)
        {
            locked = true;

            if (opened)
            {
                doorAnimation.Play("close");
                opened = false;
            }

            return;
        }

        locked = false;
        
        //Set door being opened 
        if (opened)
        {
            doorAnimation.Play("open");
            opened = true;
        }
        else
        {
            doorAnimation.Play("close");
            opened = false;
        }
    }
}
