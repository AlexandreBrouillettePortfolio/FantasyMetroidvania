using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScript : MonoBehaviour
{
    public GameObject RespwnPoint;
    public float RespawnDelay = 1f;
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            if (other.GetComponent<PlayerMovement>().isDead) return;
            
            Debug.Log("Player hit lava, starting respawn timer.");
            StartCoroutine(RespawnAfterDelay(other));
        }
    }

    private IEnumerator RespawnAfterDelay(Collider2D playerCollider)
    {
        // Wait for 1 second before respawning
        yield return new WaitForSeconds(RespawnDelay);

        // Move the player to the respawn point.
        playerCollider.transform.position = RespwnPoint.transform.position;

        // Reset player's velocity if it has a Rigidbody2D component.
        Rigidbody2D rb = playerCollider.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

    }
}
