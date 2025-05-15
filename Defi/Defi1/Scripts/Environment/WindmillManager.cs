using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillManager : InteractableHandler
{
    public List<ActivatableHandler> activatableList = new List<ActivatableHandler>();

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            print("Rotating");
            transform.Rotate(0, 0, -48*Time.deltaTime);
        }
    }

    public override void Activate(bool state, int element)
    {
        if (element == 2)
        {
            activated = state;
            if (activatableList.Count > 0)
            {
                foreach (ActivatableHandler obj in activatableList)
                {
                    obj.Activate(state);
                }
            }
        }
    }
}
