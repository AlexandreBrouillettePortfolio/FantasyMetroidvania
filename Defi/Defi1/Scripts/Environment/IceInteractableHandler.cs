using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceInteractableHandler : InteractableHandler
{
    public Sprite frozenSprite;
    bool isFrozen;
    bool isBoiling;
    private int activatorCount = 0;
    List<GameObject> submergedObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        isFrozen = false;
        isBoiling = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            transform.position = new Vector2(transform.position.x + Time.deltaTime, transform.position.y);
        }
    }

    public override void Activate(bool state, int element)
    {
        if (!isFrozen && element == 3)
        {
            isFrozen = true;
            isBoiling = false;
            if (GetComponent<Animator>() != null)
            {
                Destroy(GetComponent<Animator>());
            }
            if (frozenSprite != null) gameObject.GetComponent<SpriteRenderer>().sprite = frozenSprite; else gameObject.GetComponent<SpriteRenderer>().color = new Color(0.242f, 0.937f, 1f);
            if (submergedObjects.Count == 0)
            {
                gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            }
            foreach (GameObject obj in submergedObjects)
            {
                obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                obj.GetComponent<InteractableHandler>().MakeRise(false);
            }
        }
        if (isFrozen && element == 2 && !gameObject.GetComponent<BoxCollider2D>().isTrigger)
        {
            if (state)
            {
                activated = true;
                activatorCount++;
            }
            else
            {
                activatorCount--;
                if (activatorCount == 0)
                {
                    activated = false;
                }
            }
        }
        if (element == 1)
        {
            isBoiling = true;
            isFrozen = false;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.433f, 0.243f, 1f);
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            foreach (GameObject obj in submergedObjects)
            {
                obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                obj.GetComponent<InteractableHandler>().MakeRise(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFrozen && collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            submergedObjects.Add(collision.gameObject);
            if (isBoiling)
            {
                collision.gameObject.GetComponent<InteractableHandler>().MakeRise(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isFrozen && collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            collision.gameObject.GetComponent<InteractableHandler>().MakeRise(false);
            submergedObjects.Remove(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactables"))
        {
            collision.gameObject.GetComponent<InteractableHandler>().MakeRise(false);
            submergedObjects.Remove(collision.gameObject);
        }
    }
}
