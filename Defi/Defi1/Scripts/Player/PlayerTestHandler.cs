using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestHandler : MonoBehaviour
{
    public int speed;
    float activeTimer;
    float attackTimeTracker;
    bool windActive;
    int activeElement;

    // Start is called before the first frame update
    void Start()
    {
        activeTimer = 0.3f;
        attackTimeTracker = 0.0f;
        windActive = false;
        activeElement = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector2(transform.position.x + (speed *Time.deltaTime), transform.position.y);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector2(transform.position.x - (speed * Time.deltaTime), transform.position.y);
        }
        if (Input.GetKey(KeyCode.F))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + (speed * Time.deltaTime));
            GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && activeElement == 0)
        {
            AttackFire();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && activeElement == 0)
        {
            ActivateWind(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && activeElement == 2)
        {
            ActivateWind(false);
        }
        else if (Input.GetKeyDown(KeyCode.Q) && activeElement == 0)
        {
            ActivateIce();
        }
        else if (Input.GetKeyDown(KeyCode.E) && activeElement == 0)
        {
            AttackEarth();
        }
        if (attackTimeTracker > 0)
        {
            attackTimeTracker -= Time.deltaTime;
            if (attackTimeTracker <= 0)
            {
                if (activeElement == 1) transform.Find("FireHitBox").gameObject.SetActive(false);
                else if (activeElement == 3) transform.Find("IceHitBox").gameObject.SetActive(false);
                else if (activeElement == 4) transform.Find("EarthHitBox").gameObject.SetActive(false);
                attackTimeTracker = 0.0f;
                activeElement = 0;
            }
        }
    }

    void AttackFire()
    {
        if (attackTimeTracker == 0)
        {
            transform.Find("FireHitBox").gameObject.SetActive(true);
            attackTimeTracker = activeTimer;
            activeElement = 1;
        }
    }

    void AttackEarth()
    {
        if (attackTimeTracker == 0)
        {
            transform.Find("EarthHitBox").gameObject.SetActive(true);
            attackTimeTracker = activeTimer;
            activeElement = 4;
        }
    }

    void ActivateWind(bool state)
    {
        windActive = state;
        transform.Find("WindHitBox").gameObject.SetActive(state);
        if (state) activeElement = 2; else activeElement = 0;
    }

    void ActivateIce()
    {
        if (attackTimeTracker == 0)
        {
            transform.Find("IceHitBox").gameObject.SetActive(true);
            attackTimeTracker = activeTimer;
            activeElement = 3;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            collision.gameObject.GetComponent<InteractableHandler>().Activate(true, activeElement);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            collision.gameObject.GetComponent<InteractableHandler>().Activate(false, activeElement);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
