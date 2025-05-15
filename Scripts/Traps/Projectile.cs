using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 moveSpeed = new Vector2(3f,0);
    public int damage = 10;
    public float knockbackMagnitude = 10f; // Adjust as needed

    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (transform.parent.rotation.y == 0)
        {
            moveSpeed.x = -Mathf.Abs(moveSpeed.x);
        }
    }

    // Update is called once per frame
    void Start()
    {
        rb.velocity = new Vector2(moveSpeed.x, moveSpeed.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // HealthBar(Script) playerHealth= colision.GetComponent<Health>();
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
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
        }

        Destroy(gameObject);
        Debug.Log("Arrow has hit: " + collision.name);
    }
}
