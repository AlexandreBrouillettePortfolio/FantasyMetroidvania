using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthInteractableHandler : InteractableHandler
{
    [SerializeField]
    GameObject Manager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate(bool state, int element)
    {
        if (element == 4)
        {
            if (Manager != null)
            {
                Manager.GetComponent<LevelManager>().ChildDestroyed(gameObject.name);
            }
            Destroy(gameObject);
        }
    }
}
