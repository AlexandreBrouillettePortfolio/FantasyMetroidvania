using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public List<ActivatableHandler> activatableList = new List<ActivatableHandler>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            foreach (ActivatableHandler obj in activatableList)
            {
                obj.Activate(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            foreach (ActivatableHandler obj in activatableList)
            {
                obj.Activate(false);
            }
        }
    }
}
