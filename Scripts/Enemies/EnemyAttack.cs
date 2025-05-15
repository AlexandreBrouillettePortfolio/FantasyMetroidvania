using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float stunDuration = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BaseEnemy enemy = GetComponentInParent<BaseEnemy>();
            if (enemy == null || enemy.IsInCombat()) return;

            // Mettre à jour l'état global
            if (EnemyGlobalState.Instance != null)
            {
                EnemyGlobalState.Instance.SetPlayerHit(true);
            }

            // Récupérer le TurnManager
            TurnManager turnManager = FindObjectOfType<TurnManager>();
            if (turnManager != null)
            {
                // Démarrer le combat au tour par tour
                turnManager.StartCombat(enemy, other.GetComponent<Player>());
            }

            // Appliquer le knockback
            var playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            // Appliquer le stun
            var playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.CombatTrigger(true);
            }
        }
    }
} 