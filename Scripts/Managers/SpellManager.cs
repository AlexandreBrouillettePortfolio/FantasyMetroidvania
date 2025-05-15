using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [SerializeField]
    private GameObject parentObject;

    [SerializeField]
    private int element;

    [SerializeField]
    private int timeToLive;

    public Animator animator;

    private void Start()
    {
        animator.SetInteger(Animators.Spell.Element, element);
    }

    public void Activate()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void CheckIfRestartOrKill()
    {
        if (timeToLive == 0) return;
        timeToLive--;
        if (timeToLive == 0)
        {
            Destroy(parentObject.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            print("Trigger Collided");
            collision.gameObject.GetComponent<InteractableHandler>().Activate(true, element);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            collision.gameObject.GetComponent<InteractableHandler>().Activate(false, element);
        }
    }
}
