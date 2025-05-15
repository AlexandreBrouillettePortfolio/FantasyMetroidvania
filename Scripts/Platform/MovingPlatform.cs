using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum MovementMode
    {
        PingPong,
        RotateAround
    }

    [Header("General Settings")]
    public MovementMode movementMode = MovementMode.PingPong;

    [Header("Elevator Settings")]
    public bool isElevator = false;  // Set to true for elevator mode

    [Header("PingPong Settings")]
    public Transform pointA;
    public Transform pointB;
    // For elevator mode, start with no movement
    public float moveSpeed = 0f;
    // This holds the current target destination.
    public Vector3 nextPosition;

    [Header("RotateAround Settings")]
    public Transform pivotPoint;
    public float rotationSpeed = 30f;
    public Vector3 rotationAxis = Vector3.up;
    public bool rotateSprite = false;

    private void Start()
    {
        if (movementMode == MovementMode.PingPong && pointA != null && pointB != null)
        {
            // For continuous ping-pong, you might set nextPosition here.
            // For elevator mode, the platform should be at an endpoint (pointA or pointB)
            if (!isElevator)
            {
                nextPosition = pointB.position;
            }
            else
            {
                // Ensure the platform starts exactly at an endpoint.
                // For example, if it's at pointA:
                nextPosition = pointA.position;
                transform.position = pointA.position;
            }
        }
    }

    private void Update()
    {
        switch (movementMode)
        {
            case MovementMode.PingPong:
                DoPingPongMovement();
                break;
            case MovementMode.RotateAround:
                DoRotateAround();
                break;
        }
    }

    /// <summary>
    /// Moves the platform between pointA and pointB.
    /// In elevator mode, the platform moves only when moveSpeed > 0.
    /// </summary>
    private void DoPingPongMovement()
    {
        if (pointA == null || pointB == null) return;

        if (moveSpeed != 0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

            // When the platform reaches the target, stop movement.
            if (Vector3.Distance(transform.position, nextPosition) < 0.001f)
            {
                if (isElevator)
                {
                    moveSpeed = 0f;
                }
                else
                {
                    nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
                }
            }
        }
    }

    /// <summary>
    /// Rotates the platform around a pivot.
    /// </summary>
    private void DoRotateAround()
    {
        if (pivotPoint == null) return;

        float spriteRotationSpeed = -rotationSpeed;
        transform.RotateAround(pivotPoint.position, rotationAxis, rotationSpeed * Time.deltaTime);

        if (!rotateSprite)
        {
            transform.Rotate(Vector3.forward, spriteRotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!gameObject.activeInHierarchy)
                return;

            StartCoroutine(DetachAfterFrame(collision.transform));
        }
    }

    private IEnumerator DetachAfterFrame(Transform player)
    {
        yield return new WaitForEndOfFrame();
        if (player != null)
        {
            player.SetParent(null);
        }
    }
}
