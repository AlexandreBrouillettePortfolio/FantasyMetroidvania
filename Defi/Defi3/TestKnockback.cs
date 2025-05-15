using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKnockback : MonoBehaviour
{
   // Reference to the PlayerMovement component; assign it via the Inspector.
    public PlayerMovement playerMovement;

    // The magnitude of the knockback force applied.
    public float knockbackMagnitude = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // H key: left
        {
            Vector2 knockback = new Vector2(-1f, 0f) * knockbackMagnitude;
            Debug.Log("Testing OnPlayerHit with left knockback: " + knockback);
            playerMovement.OnPlayerHit(knockback);
        }
        else if (Input.GetKeyDown(KeyCode.J)) // J key: down
        {
            Vector2 knockback = new Vector2(0f, -1f) * knockbackMagnitude;
            Debug.Log("Testing OnPlayerHit with down knockback: " + knockback);
            playerMovement.OnPlayerHit(knockback);
        }
        else if (Input.GetKeyDown(KeyCode.K)) // K key: right
        {
            Vector2 knockback = new Vector2(1f, 0f) * knockbackMagnitude;
            Debug.Log("Testing OnPlayerHit with right knockback: " + knockback);
            playerMovement.OnPlayerHit(knockback);
        }
        else if (Input.GetKeyDown(KeyCode.U)) // U key: up
        {
            Vector2 knockback = new Vector2(0f, 1f) * knockbackMagnitude;
            Debug.Log("Testing OnPlayerHit with up knockback: " + knockback);
            playerMovement.OnPlayerHit(knockback);
        }
    }
}
