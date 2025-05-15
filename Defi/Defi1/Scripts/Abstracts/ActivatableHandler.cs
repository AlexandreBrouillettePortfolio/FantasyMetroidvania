using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatableHandler : MonoBehaviour
{
    protected bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(bool state)
    {
        isActive = state;
        ChildActivate(state);
    }

    public abstract void ChildActivate(bool state);
}
