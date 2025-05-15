using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class OscillatingHitboxWithEasing : MonoBehaviour
{
    [Header("Oscillation Settings")]
    // The center point around which the hitbox oscillates.
    public Transform centerPoint;
    // Duration to move from 0° to 180°.
    public float moveDuration = 0.625f;
    // Duration to pause at each endpoint (0° and 180°).
    public float pauseDuration = 0.625f;
    // (For a complete cycle of 2.5 seconds, ensure moveDuration + pauseDuration = 1.25 sec)

    // Calculated values from the initial setup.
    private float radius;          // Distance between the hitbox and the center.
    private Vector2 initialOffset; // Initial offset from the center.
    private float initialAngle;    // Angle (in degrees) corresponding to the starting position (0°).

    // Oscillation states.
    private enum OscState { PausedAtStart, MovingForward, PausedAtEnd, MovingBackward }
    private OscState currentState;
    private float stateTimer = 0f;

    private void Start()
    {
        if (centerPoint == null)
        {
            Debug.LogError("CenterPoint not assigned!");
            enabled = false;
            return;
        }

        // Calculate the initial offset and radius based on the hitbox's starting position.
        initialOffset = transform.position - centerPoint.position;
        radius = initialOffset.magnitude;
        // Determine the initial angle in degrees (this will be our 0° reference).
        initialAngle = Mathf.Atan2(initialOffset.y, initialOffset.x) * Mathf.Rad2Deg;

        // Start with a pause at the starting position.
        currentState = OscState.PausedAtStart;
        stateTimer = 0f;
    }

    private void Update()
    {
        if (centerPoint == null)
            return;

        stateTimer += Time.deltaTime;
        float deltaAngle = 0f;

        switch (currentState)
        {
            case OscState.PausedAtStart:
                deltaAngle = 0f;
                if (stateTimer >= pauseDuration)
                {
                    stateTimer = 0f;
                    currentState = OscState.MovingForward;
                }
                break;

            case OscState.MovingForward:
                {
                    float t = Mathf.Clamp01(stateTimer / moveDuration);
                    // SmoothStep eases the value so movement is faster in the middle.
                    float easedT = Mathf.SmoothStep(0f, 1f, t);
                    // Interpolate from 0° to 180°.
                    deltaAngle = Mathf.Lerp(0f, 180f, easedT);
                    if (stateTimer >= moveDuration)
                    {
                        stateTimer = 0f;
                        currentState = OscState.PausedAtEnd;
                    }
                }
                break;

            case OscState.PausedAtEnd:
                deltaAngle = 180f;
                if (stateTimer >= pauseDuration)
                {
                    stateTimer = 0f;
                    currentState = OscState.MovingBackward;
                }
                break;

            case OscState.MovingBackward:
                {
                    float t = Mathf.Clamp01(stateTimer / moveDuration);
                    float easedT = Mathf.SmoothStep(0f, 1f, t);
                    // Interpolate from 180° back to 0°.
                    deltaAngle = Mathf.Lerp(180f, 0f, easedT);
                    if (stateTimer >= moveDuration)
                    {
                        stateTimer = 0f;
                        currentState = OscState.PausedAtStart;
                    }
                }
                break;
        }

        // The new angle is the initial angle plus the computed delta.
        float newAngle = initialAngle + deltaAngle;
        float newAngleRad = newAngle * Mathf.Deg2Rad;
        // Compute the new offset based on the radius and new angle.
        Vector2 newOffset = new Vector2(radius * Mathf.Cos(newAngleRad), radius * Mathf.Sin(newAngleRad));
        // Update the hitbox position relative to the center.
        transform.position = centerPoint.position + new Vector3(newOffset.x, newOffset.y, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            /* Récupère le composant PlayerHealth sur l'objet en collision
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            */
        }
    }

    // Affiche des Gizmos dans l'éditeur pour visualiser l'arc et la position actuelle.
    private void OnDrawGizmos()
    {
        if (centerPoint != null)
        {
            Gizmos.color = Color.green;
            // Dessine un cercle représentant le rayon (calculé dynamiquement en mode édition).
            Gizmos.DrawWireSphere(centerPoint.position, (transform.position - centerPoint.position).magnitude);
            // Dessine une ligne entre le centre et la position actuelle de la hitbox.
            Gizmos.DrawLine(centerPoint.position, transform.position);
        }
    }
}