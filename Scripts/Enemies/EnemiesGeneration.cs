using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemiesGeneration : MonoBehaviour
{
    [Tooltip("The ratio for the number of enemies is 60% ground and 40% flying")]
    public int numberOfEnemies = 1;

    [Tooltip("The spawner Object")]
    public GameObject spawnerObject;

    public List<GameObject> groundPrefabEnemies;
    public List<GameObject> flyingPrefabEnemies;
    public GameObject bossPrefab;

    [Tooltip("List of the trigger for the fight")]
    public GameObject[] triggerCombat;

    [Tooltip("The final list of enemies")]
    public List<GameObject> finalEnemies = new List<GameObject>();

    [Tooltip("The player")]
    public GameObject player;

    private float xStart = 0;
    private float yStart = 0;
    private float xStartFlying = 0;

    private string uuidSpawn;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 spawnCollider = spawnerObject.GetComponent<Collider2D>().bounds.size;
        xStart = spawnerObject.transform.position.x - (spawnCollider.x / 3);
        xStartFlying = spawnerObject.transform.position.x - (spawnCollider.x / 3);
        yStart = spawnerObject.transform.position.y + spawnCollider.y;

        // Generate random uuid for the monster who had spwned with this spawner
        uuidSpawn = Guid.NewGuid().ToString();

        // Create and set the position of the enemies
        StartCoroutine(CreationOfEnemies());
    }

    IEnumerator InstantiateGroundEnemies(int nbGroundEnemiesToSpawn)
    {
        // Distance minimale entre les ennemis terrestres
        float minDistanceBetweenEnemies = 2.5f;
        int randomGroundEnemyIndex = 0;

        // Liste des positions déjà occupées
        List<Vector2> occupiedPositions = new List<Vector2>();

        for (int i = 0; i < nbGroundEnemiesToSpawn; i++)
        {

            randomGroundEnemyIndex = (randomGroundEnemyIndex < groundPrefabEnemies.Count - 1) ? randomGroundEnemyIndex + 1 : randomGroundEnemyIndex - 1;
            if (randomGroundEnemyIndex < 0)
                randomGroundEnemyIndex = 0;
            GameObject groundEnemyPrefab = groundPrefabEnemies[randomGroundEnemyIndex];

            // Calculer d'abord la position de spawn basée sur le collider du prefab
            Vector2 enemyColliderSize = groundEnemyPrefab.GetComponent<Collider2D>().bounds.size;
            Vector2 spawnPosition = new Vector2(xStart + enemyColliderSize.x, yStart + enemyColliderSize.y);

            // Vérifier si la position est trop proche d'une position déjà occupée
            bool positionValid = true;
            foreach (Vector2 occupiedPos in occupiedPositions)
            {
                if (Vector2.Distance(spawnPosition, occupiedPos) < minDistanceBetweenEnemies)
                {
                    positionValid = false;
                    // Ajuster la position en x
                    xStart += minDistanceBetweenEnemies;
                    spawnPosition = new Vector2(xStart, yStart + enemyColliderSize.y);
                    break;
                }
            }

            Debug.Log($"[EnemiesGeneration] BEFORE GROUND SPAWN - Position calculée: {spawnPosition}");

            // Mettre à jour la position de départ pour le prochain ennemi
            xStart += enemyColliderSize.x + (minDistanceBetweenEnemies * (float)(minDistanceBetweenEnemies * 0.2));

            // Ajouter cette position à la liste des positions occupées
            occupiedPositions.Add(spawnPosition);

            // Instancier l'ennemi directement à la bonne position
            GameObject enemy = Instantiate(groundEnemyPrefab, spawnPosition, Quaternion.identity);
            enemy.name = "Ground_Enemy_" + i;

            Debug.Log($"[EnemiesGeneration] AFTER GROUND SPAWN - Position de l'ennemi: {enemy.transform.position}");

            // Vérifier si la position est valide
            if (spawnPosition == Vector2.zero)
            {
                Debug.LogWarning($"[EnemiesGeneration] Position invalide (0,0) pour l'ennemi terrestre!");
                Destroy(enemy);
                continue;
            }

            // Forcer la bonne position
            enemy.transform.position = spawnPosition;

            // Vérifier et configurer le composant GroundMovement
            GroundEnemy groundEnemy = enemy.GetComponent<GroundEnemy>();
            if (groundEnemy != null)
            {
                GroundMovement groundMovement = enemy.GetComponent<GroundMovement>();
                if (groundMovement != null)
                {
                    groundMovement.SetStartPosition(spawnPosition);
                    Debug.Log($"[EnemiesGeneration] Ground enemy position set at: {spawnPosition}");
                }
            }

            yield return new WaitForEndOfFrame();

            // Vérifier une dernière fois après une frame
            Debug.Log($"[EnemiesGeneration] FINAL GROUND position: {enemy.transform.position}");
            if (enemy.transform.position.x == 0 && enemy.transform.position.y == 0)
            {
                Debug.LogError($"[EnemiesGeneration] ERREUR: L'ennemi terrestre est retourné à (0,0) après une frame!");
                // Repositionner à nouveau
                enemy.transform.position = spawnPosition;

                // Refaire le verrouillage de position si possible
                if (groundEnemy != null && enemy.GetComponent<GroundMovement>() != null)
                {
                    enemy.GetComponent<GroundMovement>().SetStartPosition(spawnPosition);
                }
            }

            finalEnemies.Add(enemy);
        }
    }

    IEnumerator InstantiateFlyingEnemies(int nbFlyingEnemiesToSpawn)
    {
        // Create a list of random flying enemies
        List<GameObject> randomFlyingEnemies = new List<GameObject>();

        for (int i = 0; i < nbFlyingEnemiesToSpawn; i++)
        {
            int randomFlyingEnemyIndex = UnityEngine.Random.Range(0, flyingPrefabEnemies.Count - 1);
            randomFlyingEnemies.Add(flyingPrefabEnemies[randomFlyingEnemyIndex]);
        }

        // Réinitialiser la position X pour les ennemis volants
        float flyingXStart = spawnerObject.transform.position.x - (spawnerObject.GetComponent<Collider2D>().bounds.size.x / 3);

        // Définir une hauteur significativement plus élevée pour les ennemis volants
        float flyingYStart = yStart + 5f; // hauteur plus élevée pour les ennemis volants

        // Distance minimale entre les ennemis volants
        float minDistanceBetweenEnemies = 3.0f;

        // Liste des positions déjà occupées
        List<Vector2> occupiedPositions = new List<Vector2>();

        foreach (GameObject flyingEnemyPrefab in randomFlyingEnemies)
        {
            // Calculer d'abord la position de spawn
            Vector2 enemyColliderSize = flyingEnemyPrefab.GetComponent<Collider2D>().bounds.size;
            Vector2 spawnPosition = new Vector2(flyingXStart, flyingYStart);

            // Vérifier si la position est trop proche d'une position déjà occupée
            bool positionValid = true;
            foreach (Vector2 occupiedPos in occupiedPositions)
            {
                if (Vector2.Distance(spawnPosition, occupiedPos) < minDistanceBetweenEnemies)
                {
                    positionValid = false;
                    // Ajuster la position en x
                    flyingXStart += minDistanceBetweenEnemies;
                    spawnPosition = new Vector2(flyingXStart, flyingYStart);
                    break;
                }
            }

            Debug.Log($"[EnemiesGeneration] BEFORE SPAWN - Position calculée: {spawnPosition}");

            // Mettre à jour la position de départ pour le prochain ennemi
            flyingXStart += enemyColliderSize.x + minDistanceBetweenEnemies;

            // Ajouter cette position à la liste des positions occupées
            occupiedPositions.Add(spawnPosition);

            // Instancier l'ennemi directement à la position correcte (éviter 0,0)
            GameObject enemy = Instantiate(flyingEnemyPrefab, spawnPosition, Quaternion.identity);
            enemy.name = "Flying_Enemy_" + finalEnemies.Count;

            Debug.Log($"[EnemiesGeneration] AFTER SPAWN - Position de l'ennemi: {enemy.transform.position}");

            // Vérifier et ajouter les composants nécessaires si manquants
            FlyingEnemy flyingEnemy = enemy.GetComponent<FlyingEnemy>();
            if (flyingEnemy == null)
            {
                flyingEnemy = enemy.AddComponent<FlyingEnemy>();
                Debug.Log($"[EnemiesGeneration] Added FlyingEnemy component to {enemy.name}");
            }

            ProjectileAttack projectileAttack = enemy.GetComponent<ProjectileAttack>();
            if (projectileAttack == null)
            {
                projectileAttack = enemy.AddComponent<ProjectileAttack>();
                Debug.Log($"[EnemiesGeneration] Added ProjectileAttack component to {enemy.name}");
            }

            FlyingMovement flyingMovement = enemy.GetComponent<FlyingMovement>();
            if (flyingMovement == null)
            {
                flyingMovement = enemy.AddComponent<FlyingMovement>();
                Debug.Log($"[EnemiesGeneration] Added FlyingMovement component to {enemy.name}");
            }

            // S'assurer que l'Animator est référencé pour FlyingMovement
            if (flyingMovement.animator == null)
            {
                Animator animator = enemy.GetComponent<Animator>();
                if (animator != null)
                {
                    flyingMovement.animator = animator;
                }
            }

            // Forcer l'ennemi à être à la bonne position
            enemy.transform.position = spawnPosition;

            // Verrouiller la position après tous les composants ajoutés
            if (flyingEnemy != null)
            {
                flyingEnemy.LockPosition(spawnPosition);
                Debug.Log($"[EnemiesGeneration] Position after lock: {enemy.transform.position}");
            }

            yield return new WaitForEndOfFrame();

            // Une dernière vérification après la frame
            Debug.Log($"[EnemiesGeneration] FINAL position: {enemy.transform.position}");
            if (enemy.transform.position.x == 0 && enemy.transform.position.y == 0)
            {
                Debug.LogError($"[EnemiesGeneration] ERREUR: L'ennemi est retourné à (0,0) après une frame!");
                // Essayer de repositionner à nouveau
                enemy.transform.position = spawnPosition;
            }

            finalEnemies.Add(enemy);
        }
    }

    /// <summary>
    /// Instantiate the boss
    /// </summary>
    /// <returns></returns>
    IEnumerator InstantiateBoss()
    {
        Vector2 spawnCollider = spawnerObject.GetComponent<Collider2D>().bounds.size;

        float bossSpawnX = spawnerObject.transform.position.x;
        float bossSpawnY = spawnerObject.transform.position.y + (spawnCollider.y * 2);

        GameObject boss = Instantiate(bossPrefab, new Vector2(bossSpawnX, bossSpawnY), Quaternion.identity);

        yield return new WaitForEndOfFrame();
    }


    IEnumerator CreationOfEnemies()
    {
        // Check if their is a boss
        if (bossPrefab != null)
        {
            yield return StartCoroutine(InstantiateBoss());
        }
        else
        {
            // No boss -> normal enemies
            // 60 % of enemies are ground
            int nbGroundEnemiesToSpawn = (int)Mathf.Ceil(numberOfEnemies * 0.60f);
            // 40 % of enemies are Flying
            int nbFlyingEnemiesToSpawn = (int)Mathf.Ceil(numberOfEnemies * 0.40f);

            // If there is no enemies, spawn at least one
            if (nbFlyingEnemiesToSpawn == 0 && nbGroundEnemiesToSpawn == 0)
            {
                nbGroundEnemiesToSpawn = 1;
                nbFlyingEnemiesToSpawn = 1;
            }

            if (groundPrefabEnemies.Count > 0)
            {
                yield return StartCoroutine(InstantiateGroundEnemies(nbGroundEnemiesToSpawn));
            }
            if (flyingPrefabEnemies.Count > 0)
            {
                yield return StartCoroutine(InstantiateFlyingEnemies(nbFlyingEnemiesToSpawn));
            }
        }
        SetupTriggerCombatSystem();
    }

    /// <summary>
    /// Function that check if the enemy position is valid -> no enemy in him
    /// </summary>
    /// <param name="enemy"> an enemy </param>
    /// <returns> bool if the position is valid else false </returns>
    bool CheckForCollisionAfterPositioning(GameObject enemy)
    {
        Collider2D boxCollider = enemy.GetComponent<Collider2D>();

        if (boxCollider != null)
        {

            Vector2 enemyPosition = enemy.gameObject.transform.position;

            // Radius -> 2 x the size of the enemy
            float radius = enemy.GetComponent<Collider2D>().bounds.size.y * 2f;

            // Set the center a little bit under the enemy
            Vector2 circlePosition = new Vector2(enemyPosition.x, enemyPosition.y - enemy.GetComponent<Collider2D>().bounds.size.y);

            // Collision check
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(circlePosition, radius);

            foreach (Collider2D hitCollider in hitColliders)
            {
                // Check if the hit collider is another enemy (ignoring the current one)
                GameObject otherEnemy = hitCollider.gameObject;

                if (otherEnemy != null && otherEnemy != enemy && finalEnemies.Contains(otherEnemy))
                {
                    // Collision detected with another enemy or there is an enemy below
                    return true;
                }
            }

            // No collision with other enemies
            return false;
        }

        // If the enemy doesn't have a Collider2D, return false (no collision)
        return false;
    }

    void SetupTriggerCombatSystem()
    {
        // Add uuid spawn for each monster
        foreach (GameObject enemy in finalEnemies)
        {
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().AddUuidSpawn(uuidSpawn);
            }
        }

        foreach (GameObject trigger in triggerCombat)
        {
            CombatTrigger combatTrigger = trigger.GetComponent<CombatTrigger>();

            if (finalEnemies.Count > 0 && combatTrigger != null)
            {
                combatTrigger.AddOpponents(finalEnemies);
            }
        }
    }
}
