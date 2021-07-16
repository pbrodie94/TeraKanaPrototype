using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] protected float interactRange = 3;

    protected string message;
    private bool messageShown = false;
    private bool opened = false;
    protected bool canClose = true;

    [SerializeField] private bool locked = false;
    public bool isLocked
    {
        get
        {
            return locked;
        }
    }

    [SerializeField] protected GameObject[] controlBox;

    protected Transform player;
    protected Transform cam;

    protected HUDManager hud;
    protected Animation animation;

    protected virtual void Start()
    {
        
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();

        animation = GetComponentInChildren<Animation>();

        message = "Press 'E' to open door.";
        messageShown = false;

        LevelController.PlayerSpawned += GetPlayerReference;
    }

    public void GetPlayerReference()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = Camera.main.transform;
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
                    hud.ShowMessage(message, true);
                    messageShown = true;

                    if (Input.GetButtonDown(InputManager.Action))
                    {
                        DoorInteraction();
                    }
                }
                else
                {
                    if (messageShown)
                    {
                        hud.ShowMessage(null, false);
                        messageShown = false;
                    }
                }
            }
            
        }
        else
        {
            if (messageShown)
            {
                hud.ShowMessage(null, false);
                messageShown = false;
            }
        }
    }

    protected virtual void DoorInteraction()
    {
        //Open door
        if (!opened)
        {
            animation.Play("open");
            message = "Press 'E' to close door";
            opened = true;

        } else if (opened && canClose)
        {
            animation.Play("close");
            message = "Press 'E' to open door.";
            opened = false;
        }
    }
}
