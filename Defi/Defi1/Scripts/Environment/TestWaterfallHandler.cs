using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWaterfallHandler : MonoBehaviour
{
    public GameObject nextSegment;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            nextSegment.SetActive(true);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            collision.gameObject.GetComponent<InteractableHandler>().Activate(false, 2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            collision.gameObject.GetComponent<InteractableHandler>().Activate(true, 2);
        }
    }
}
