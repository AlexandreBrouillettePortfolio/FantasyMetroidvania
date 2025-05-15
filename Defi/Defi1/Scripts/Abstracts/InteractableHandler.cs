using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableHandler : MonoBehaviour
{
    protected bool activated;
    protected bool isRising;
    protected float graceRisePeriod = 0.2f;
    protected float graceRiseTracker = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        activated = false;
    }

    public abstract void Activate(bool state, int element);

    public void MakeRise(bool state)
    {
        isRising = state;
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0.0f; 
        }
        if (!isRising)
        {
            graceRiseTracker = graceRisePeriod;
        }
    }
}
