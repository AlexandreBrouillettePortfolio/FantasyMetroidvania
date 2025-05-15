using UnityEngine;

public class Water
{
    public float height = 0f;
    public float velocity = 0f;
    public float mass = 1.0f;
    public float stiffness = 0.001f;
    public float decay = 0.99f;
    public float spread = 0.02f;

    public Water(float xPosition)
    {
        height = 0f;
        velocity = 0f;
    }
}

public class WaterColliderTrigger : MonoBehaviour
{
    [HideInInspector] public int index;
    [HideInInspector] public WaterSpriteShapeManager waterManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[WaterColliderTrigger] {gameObject.name} OnTriggerEnter2D with {other.name}");

        if (other.attachedRigidbody != null)
        {
            float impact = other.attachedRigidbody.velocity.y * -waterManager.impactForce;
            waterManager.ApplyForce(index, impact);
        }
    }
}