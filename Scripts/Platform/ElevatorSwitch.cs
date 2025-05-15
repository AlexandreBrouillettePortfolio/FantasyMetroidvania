using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ElevatorSwitch : MonoBehaviour
{
    // Reference to the MovingPlatform script on your elevator.
    public MovingPlatform movingPlatform;
    // Speed to activate the elevator.
    public float activationSpeed = 2f;
    public bool playerInsideZone = false;
    public GameObject interactionUI;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerInsideZone)
        {
            if (Input.GetButton("Interact"))                                  //les inputs du bouton F.
            {
                if (_animator != null)
                {
                    _animator.SetTrigger("Switch");
                }

                MovePlateforme();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideZone = true;
            interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideZone = false;
            interactionUI.SetActive(false);
        }
    }

    private void MovePlateforme()
    {
        if (movingPlatform.moveSpeed > 0f)
        {
            bool headingForA = Vector3.Distance(movingPlatform.nextPosition,
                                                movingPlatform.pointA.position) < 0.01f;

            movingPlatform.nextPosition = headingForA
                                        ? movingPlatform.pointB.position
                                        : movingPlatform.pointA.position;
        }
        else
        {
            float distToA = Vector3.Distance(movingPlatform.transform.position,
                                             movingPlatform.pointA.position);
            float distToB = Vector3.Distance(movingPlatform.transform.position,
                                             movingPlatform.pointB.position);

            movingPlatform.nextPosition = (distToA <= distToB)
                                        ? movingPlatform.pointB.position
                                        : movingPlatform.pointA.position;
        }

        movingPlatform.moveSpeed = activationSpeed;
    }
}
