using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrap : MonoBehaviour
{
    public float knockbackMagnitude = 10f; // Adjust as needed
    public int damage = 10;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with an object tagged as "Player"
        if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.collider.GetComponent<Player>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            PlayerMovement playerMovement = collision.collider.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Calculate the normalized direction vector from the projectile to the player
                Vector2 direction = (collision.transform.position - transform.position).normalized;

                // Use the highest value between x and y, setting the other axis to 0
                Vector2 knockbackForce;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    knockbackForce = new Vector2(Mathf.Sign(direction.x) * knockbackMagnitude, 0f);
                }
                else
                {
                    knockbackForce = new Vector2(0f, Mathf.Sign(direction.y) * knockbackMagnitude);
                }
                // Call the player's hit method with the computed knockback force
                playerMovement.OnPlayerHit(knockbackForce);
            }
            //Debug.Log("CircleCollider collided with Player!");
        }
    }
}
