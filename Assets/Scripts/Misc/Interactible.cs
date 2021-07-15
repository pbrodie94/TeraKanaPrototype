using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour
{
    [Header("Interact Preferences")]
    protected string interactMessage;
    [SerializeField] protected float interactionRange = 4;
    [SerializeField] protected bool holdInteract = false;

    protected float holdProgress = 0;
    [SerializeField] protected float progressSpeed = 150;

    protected bool isInteractible = true;
    protected bool messageShown = false;

    protected Transform player;
    protected Transform cam;

    protected HUDManager hud;

    protected virtual void Start()
    {
        hud = GameObject.FindGameObjectWithTag("UI").GetComponent<HUDManager>();

        LevelController.PlayerSpawned += GetPlayerReference;

        interactMessage = holdInteract ? "Hold 'E' to " : "Press 'E' to ";
    }

    public virtual void GetPlayerReference()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = Camera.main.transform;
    }

    protected virtual void Update()
    {
        if (!player || !isInteractible)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= interactionRange)
        {
            RaycastHit hit;

            if (Physics.Raycast(cam.position, cam.forward, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    //Does the player need to hold the action button to interact
                    if (holdInteract)
                    {
                        //Update the interaction bar
                        hud.UpdateInteractBar(interactMessage, holdProgress);
                        messageShown = true;

                        //If player is holding the button
                        if (Input.GetButton(InputManager.Action))
                        {
                            //Check if progress is greater than 100
                            if (holdProgress < 100)
                            {
                                //Increase the progress
                                holdProgress += progressSpeed * Time.fixedDeltaTime;

                            } else
                            {
                                //Hide the interaction bar, and call the interact function
                                Interact();
                                hud.HideInteractBar();
                                messageShown = false;
                                holdProgress = 0;

                            }
                        } 

                        //If player releases the button, reset the progress to 0
                        if (Input.GetButtonUp(InputManager.Action))
                        {
                            holdProgress = 0;
                        }
                    }
                    else
                    {
                        //Show interaction prompt
                        if (!messageShown)
                        {
                            hud.ShowMessage(interactMessage, true);
                            messageShown = true;
                        }

                        //Player only needs to press the button once to interact
                        if (Input.GetButtonDown(InputManager.Action))
                        {

                            Interact();
                            hud.ShowMessage(null, false);
                            messageShown = false;
                        }
                    }
                }
                //If player stops looking at the object, hide the message
                else if (messageShown)
                {
                    if (holdInteract)
                    {
                        hud.HideInteractBar();
                        messageShown = false;
                    }
                    else
                    {
                        hud.ShowMessage(null, false);
                        messageShown = false;
                    }

                }

            }
            
        }
        //If player moves out of range, hide the message
        else if (messageShown)
        {
            if (holdInteract)
            {
                hud.HideInteractBar();
                messageShown = false;
            }
            else
            {
                hud.ShowMessage(null, false);
                messageShown = false;
            }

        }
    }

    //Virtual function to allow individual objects to apply their unique interact functionality
    protected virtual void Interact()
    {

        if (holdInteract)
        {
            hud.HideInteractBar();
        }
        else
        {
            hud.ShowMessage(null, false);
        }

        isInteractible = false;
    }

    //Show the interaction range when the object is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
