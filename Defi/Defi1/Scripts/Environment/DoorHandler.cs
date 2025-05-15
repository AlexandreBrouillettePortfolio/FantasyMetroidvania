using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : ActivatableHandler
{
    public float raiseTimer;
    public int doorType; //0 = vertical, 1 = horizontal
    float raiseTimerTracker;
    float startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive && raiseTimerTracker > 0)
        {
            if (doorType == 0) transform.position = new Vector2(transform.position.x, transform.position.y + Time.deltaTime);
            else if (doorType == 1) transform.position = new Vector2(transform.position.x + Time.deltaTime, transform.position.y);
            raiseTimerTracker -= Time.deltaTime;
        }
        else if (!isActive && transform.position.y > startPos)
        {
            if (doorType == 0) transform.position = new Vector2 (transform.position.x, transform.position.y - Time.deltaTime);
            else if (doorType == 1) transform.position = new Vector2(transform.position.x - Time.deltaTime, transform.position.y);
        }
    }

    public override void ChildActivate(bool state)
    {
        if (state)
        {
            raiseTimerTracker = raiseTimer;
        }
    }
}
