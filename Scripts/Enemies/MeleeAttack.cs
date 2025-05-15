using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float frontAttackRange = 1.5f;
    [SerializeField] private float circularAttackRange = 2f;
    [SerializeField] private int attackDamage = 10;

    private Transform target;

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;
    }

    public void PerformFrontAttack()
    {
        if (target == null) return;

        // Get direction to target
        float direction = target.position.x > transform.position.x ? 1 : -1;
        Vector2 attackPosition = (Vector2)transform.position + Vector2.right * direction * frontAttackRange;
        
        // Visual effect for debugging
        CreateAttackVisual(attackPosition, frontAttackRange, new Color(1, 0, 0, 0.5f), 0.2f);
        
        // Line debug for frontal attack
        Debug.DrawLine(transform.position, attackPosition, Color.red, 0.5f);
        
        // Hit detection
        DetectHits(attackPosition, frontAttackRange);
    }

    public void PerformCircularAttack()
    {
        // Visual effect for debugging
        CreateAttackVisual(transform.position, circularAttackRange * 2, new Color(1f, 0.5f, 0f, 0.7f), 0.3f);
        
        // Circle debug for circular attack
        DrawDebugCircle(transform.position, circularAttackRange);
        
        // Hit detection
        DetectHits(transform.position, circularAttackRange);
    }

    private void DetectHits(Vector2 center, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // In a real game, we would call a damage method on the player here
                Debug.Log($"Attack hit player for {attackDamage} damage");
            }
        }
    }

    private void CreateAttackVisual(Vector2 position, float size, Color color, float duration)
    {
        GameObject attackVisual = new GameObject("AttackVisual");
        attackVisual.transform.position = position;
        SpriteRenderer visualRenderer = attackVisual.AddComponent<SpriteRenderer>();
        
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        visualRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        attackVisual.transform.localScale = new Vector3(size, size, 1);
        visualRenderer.sortingOrder = 1;
        
        Destroy(attackVisual, duration);
    }

    private void DrawDebugCircle(Vector3 center, float radius)
    {
        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10 * Mathf.Deg2Rad;
            float nextAngle = (i + 1) * 10 * Mathf.Deg2Rad;
            Vector3 start = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Vector3 end = center + new Vector3(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * radius;
            Debug.DrawLine(start, end, Color.red, 0.5f);
        }
    }
} 