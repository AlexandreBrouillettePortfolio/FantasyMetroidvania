using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteShapeController))]
public class WaterSpriteShapeManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public int springCount = 20;         // Nombre de segments
    public float spacing = 1.0f;         // Distance horizontale entre les segments
    public float waterDepth = 2.0f;      // Profondeur de l'eau
    public float stiffness = 0.001f;     // Rigidité du ressort
    public float decay = 0.99f;          // Damping
    public float spread = 0.02f;         // Propagation latérale de l'onde
    public float mass = 1.0f;            // Masse de chaque ressort
    public float impactForce = 1.0f;     // Multiplicateur pour la force d’impact

    [Header("Base Wave (Houle)")]
    public float baseWaveAmplitude = 0.02f;   // Amplitude de base réduite (au lieu de 0.05)
    public float baseWaveFrequency = 1.0f;
    public float baseWavePhaseOffset = 0.3f;

    private List<Water> springs = new List<Water>();
    private SpriteShapeController spriteShapeController;
    private List<GameObject> colliders = new List<GameObject>();

    private void Start()
    {
        // Récupère le SpriteShapeController attaché à ce GameObject
        spriteShapeController = GetComponent<SpriteShapeController>();

        // Vérifiez que l'asset SpriteShape a bien été assigné dans l'inspecteur du SpriteShapeController.
        if (spriteShapeController.spriteShape == null)
        {
            Debug.LogError("[WaterSpriteShapeManager] Aucun asset SpriteShape assigné dans le SpriteShapeController. Veuillez assigner un asset valide dans l'inspecteur.");
        }

        InitializeWater();
        CreateColliders();
        UpdateSpriteShape();
    }

    private void Update()
    {
        SimulateSprings();
        UpdateSpriteShape();
        UpdateColliders();
    }

    /// <summary>
    /// Crée chaque segment de l'eau (ressort)
    /// </summary>
    private void InitializeWater()
    {
        springs.Clear();
        for (int i = 0; i < springCount; i++)
        {
            Water spring = new Water(i * spacing)
            {
                stiffness = stiffness,
                decay = decay,
                spread = spread,
                mass = mass
            };
            springs.Add(spring);
        }
    }

    /// <summary>
    /// Simulation physique des ressorts et propagation des ondes
    /// </summary>
    private void SimulateSprings()
    {
        float dt = Time.deltaTime;
        float extraSpeedFactor = 1000f;  // Facteur réduit pour un mouvement plus doux

        // Mise à jour de chaque ressort
        for (int i = 0; i < springs.Count; i++)
        {
            float x = springs[i].height;
            float kOverM = springs[i].stiffness / springs[i].mass;
            float accel = -kOverM * x;

            // Ajoute une oscillation sinusoïdale de base
            float wave = baseWaveAmplitude * Mathf.Sin(Time.time * baseWaveFrequency + i * baseWavePhaseOffset);

            springs[i].velocity += (accel + wave) * dt * extraSpeedFactor;
            springs[i].velocity *= Mathf.Pow(springs[i].decay, dt);
            springs[i].height += springs[i].velocity * dt;
        }

        // Propagation de l'onde aux voisins
        int iterations = 8;
        float[] leftDeltas = new float[springCount];
        float[] rightDeltas = new float[springCount];

        for (int iter = 0; iter < iterations; iter++)
        {
            for (int i = 0; i < springCount; i++)
            {
                if (i > 0)
                {
                    float diffLeft = springs[i].height - springs[i - 1].height;
                    leftDeltas[i] = diffLeft * springs[i].spread * dt;
                    springs[i - 1].velocity += leftDeltas[i];
                }
                if (i < springCount - 1)
                {
                    float diffRight = springs[i].height - springs[i + 1].height;
                    rightDeltas[i] = diffRight * springs[i].spread * dt;
                    springs[i + 1].velocity += rightDeltas[i];
                }
            }
            for (int i = 0; i < springCount; i++)
            {
                if (i > 0)
                    springs[i - 1].height += leftDeltas[i];
                if (i < springCount - 1)
                    springs[i + 1].height += rightDeltas[i];
            }
        }
    }

    /// <summary>
    /// Met à jour la spline du SpriteShape pour faire correspondre le haut de l'eau à la vague.
    /// </summary>
    private void UpdateSpriteShape()
    {
        var spline = spriteShapeController.spline;
        spline.Clear();

        // Ajoute le point bas gauche
        spline.InsertPointAt(0, new Vector3(0f, -waterDepth, 0f));
        spline.SetTangentMode(0, ShapeTangentMode.Linear);

        // Ajoute les points correspondant aux crêtes de la vague
        for (int i = 0; i < springCount; i++)
        {
            Vector3 wavePos = new Vector3(i * spacing, springs[i].height, 0f);
            int insertIndex = spline.GetPointCount();
            spline.InsertPointAt(insertIndex, wavePos);
            spline.SetTangentMode(insertIndex, ShapeTangentMode.Continuous);
        }

        // Ajoute le point bas droit
        int lastIndex = spline.GetPointCount();
        float rightX = (springCount - 1) * spacing;
        spline.InsertPointAt(lastIndex, new Vector3(rightX, -waterDepth, 0f));
        spline.SetTangentMode(lastIndex, ShapeTangentMode.Linear);

        // Met à jour le collider du SpriteShape
        spriteShapeController.BakeCollider();
    }

    #region Gestion des Colliders

    /// <summary>
    /// Crée un GameObject enfant avec BoxCollider2D (isTrigger) pour chaque segment.
    /// </summary>
    private void CreateColliders()
    {
        foreach (GameObject colObj in colliders)
        {
            if (colObj != null) Destroy(colObj);
        }
        colliders.Clear();

        for (int i = 0; i < springCount; i++)
        {
            GameObject colliderObj = new GameObject($"WaterCollider_{i}");
            colliderObj.transform.SetParent(transform, false);
            colliderObj.transform.localPosition = new Vector3(i * spacing, 0, 0);

            BoxCollider2D boxCollider = colliderObj.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector2(spacing, waterDepth);

            WaterColliderTrigger trigger = colliderObj.AddComponent<WaterColliderTrigger>();
            trigger.index = i;
            trigger.waterManager = this;

            colliders.Add(colliderObj);
        }
    }

    /// <summary>
    /// Met à jour la position et la taille de chaque collider pour suivre la vague.
    /// </summary>
    private void UpdateColliders()
    {
        for (int i = 0; i < springCount; i++)
        {
            float topY = springs[i].height;
            float bottomY = topY - waterDepth;
            float midY = (topY + bottomY) * 0.5f;
            float totalHeight = Mathf.Abs(topY - bottomY);

            GameObject colliderObj = colliders[i];
            if (!colliderObj) continue;

            BoxCollider2D boxCollider = colliderObj.GetComponent<BoxCollider2D>();

            colliderObj.transform.localPosition = new Vector3(i * spacing, midY, 0);
            boxCollider.size = new Vector2(spacing, totalHeight);
            boxCollider.offset = Vector2.zero;
        }
    }

    /// <summary>
    /// Applique une force verticale sur le segment de vague indiqué.
    /// </summary>
    public void ApplyForce(int index, float force)
    {
        if (index >= 0 && index < springs.Count)
        {
            springs[index].velocity += force;
        }
    }

    #endregion
}