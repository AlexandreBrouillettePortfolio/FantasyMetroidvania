using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindInteractableHandler : InteractableHandler
{
    private int activatorCount = 0;
    private int direction = 0;
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            transform.position = new Vector2(transform.position.x + Time.deltaTime*direction, transform.position.y);
        }
        if (isRising)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + Time.deltaTime);
        }
        else if (graceRiseTracker > 0)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + Time.deltaTime);
            graceRiseTracker -= Time.deltaTime;
            if (graceRiseTracker >= 0)
            {
                GetComponent<Rigidbody2D>().gravityScale = 1.0f;
            }
        }
    }

    public override void Activate(bool state, int element)
    {
        if (element == 2)
        {
            if (state)
            {
                if (player.transform.position.x < transform.position.x)
                {
                    direction += 1; 
                }
                else if (player.transform.position.x > transform.position.x)
                {
                    direction -= 1;
                }
                activated = true;
                activatorCount++;
            }
            else
            {
                activatorCount--;
                if (activatorCount == 0)
                {
                    direction = 0;
                    activated = false;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnviroKill"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
