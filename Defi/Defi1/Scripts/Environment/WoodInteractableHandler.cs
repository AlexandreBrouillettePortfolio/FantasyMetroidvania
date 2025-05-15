using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodInteractableHandler : InteractableHandler
{
    public GameObject[] connectedRoots;
    public Sprite[] woodSprite;

    float ttl;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = woodSprite[0];
        ttl = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            ttl -= Time.deltaTime;
            if (ttl <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public override void Activate(bool state, int element)
    {
        if (element == 1)
        {
            activated = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = woodSprite[1];
        } 
    }

    private void OnDestroy()
    {
        if (connectedRoots.Length != 0)
        {
            foreach (GameObject child in connectedRoots)
            {
                if (child != null)
                {
                    child.GetComponent<WoodInteractableHandler>().Activate(true, 1);
                }
            }
        }
    }
}
