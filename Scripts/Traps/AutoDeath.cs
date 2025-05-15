using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeath : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // HealthBar(Script) playerHealth= colision.GetComponent<Health>();
        if (collision.CompareTag("Player"))
        {

            Player player = collision.GetComponent<Player>();
            
            if (player != null)
            {
                player.TakeDamage(100000000);
            }

        }
    }
}
