using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
   public float playerReach = 3.0f;
   public Camera playerCamera;

   Interactable currentInteractable;

    // Update is called once per frame
    void Update()
    {
        CheckInteraction();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void CheckInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        //if colliders with anything within player reach
        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.tag == "Interactable") //if looking at an interactable objects
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                //if there is a currentInteractable and it is not the newInteractable
                if (currentInteractable && newInteractable != currentInteractable)
                {
                    currentInteractable.DisableOutline();
                }

                if (newInteractable.enabled)
                {
                    SetNewCurrentInteractable(newInteractable);
                }
                else // if new interactable is not enabled
                {
                    DisableCurrentInteractable();
                }
            }
            else //if not an interactable
            {
                DisableCurrentInteractable();
            }
        }
        else
        {
            DisableCurrentInteractable();
        }
    }

    void SetNewCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();
        HUDController.instance.EnableInteractionText(currentInteractable.message);
    }

    void DisableCurrentInteractable()
    {
        HUDController.instance.DisableInteractionText();
        if (currentInteractable)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }
}
