using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcherManager : MonoBehaviour
{
    public int switcherNum;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (switcherNum == 0)
            {
                Camera.main.gameObject.transform.position = new Vector3(16f, 0.0f, -10.0f);
                //Destroy(gameObject);
            }
            else if (switcherNum == 1)
            {
                Camera.main.gameObject.transform.position = new Vector3(31.03f, -0.3f, -10.0f);
            }
        }
    }
}
