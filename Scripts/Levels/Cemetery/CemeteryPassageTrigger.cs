using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemeteryPassageTrigger : MonoBehaviour
{
    GameObject Foreground;

    private void Start()
    {
        Foreground = transform.root.Find("Grid").transform.Find("Foreground").gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Foreground.SetActive(!Foreground.activeSelf);
        }
    }
}
