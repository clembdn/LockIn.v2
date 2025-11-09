using System.Collections;
using UnityEngine;

/// <summary>
/// Gère la transformation du monstre ensorcelé vers le robot désensorcelé.
/// Quand le bool isTransforming passe à true, le modèle monstre est progressivement
/// remplacé par le modèle robot avec des effets visuels/sonores optionnels.
/// 
/// Usage:
/// 1. Attache ce script au GameObject monstre
/// 2. Assigne le prefab/modèle robot dans robotModel
/// 3. Configure les effets (particules, son, durée)
/// 4. Depuis un autre script: GetComponent<MonsterToRobotTransition>().TriggerTransformation();
/// </summary>
public class MonsterToRobotTransition : MonoBehaviour
{
    [Header("Transformation Trigger")]
    [Tooltip("Le bool qui déclenche la transformation. Peut être set depuis l'Inspector pour tester ou par code.")]
    public bool isTransforming = false;

    [Header("Models")]
    [Tooltip("Le GameObject/Prefab du robot qui va remplacer le monstre.")]
    public GameObject robotModel;

    [Tooltip("Si true, instancie le robotModel. Si false, active un robotModel déjà en child (désactivé).")]
    public bool instantiateRobot = true;

    [Header("Transition Settings")]
    [Tooltip("Durée de la transition (fade out monstre / fade in robot).")]
    public float transitionDuration = 2f;

    [Tooltip("Type de transition visuelle.")]
    public TransitionType transitionType = TransitionType.CrossFade;

    [Tooltip("Si true, le monstre arrête de bouger pendant la transformation.")]
    public bool freezeMonsterDuringTransition = true;

    [Tooltip("Si true, désactive le MonsterAI après la transformation.")]
    public bool disableAIAfterTransition = true;

    [Header("Visual Effects")]
    [Tooltip("Particules optionnelles jouées pendant la transformation (lumière magique, etc).")]
    public ParticleSystem transformationParticles;

    [Tooltip("Couleur de flash/fade pendant la transition.")]
    public Color transitionColor = new Color(1f, 1f, 1f, 1f);

    [Header("Audio")]
    [Tooltip("Son joué au début de la transformation.")]
    public AudioClip transformationSound;

    [Tooltip("AudioSource pour jouer le son (créé automatiquement si null).")]
    public AudioSource audioSource;

    [Header("Post-Transformation")]
    [Tooltip("Si true, le robot reste à la position du monstre et hérite de sa rotation.")]
    public bool inheritTransform = true;

    [Tooltip("Offset Y pour élever le robot (utile si le robot est trop bas dans le sol).")]
    public float robotHeightOffset = 0.5f;

    [Tooltip("Si true, le GameObject monstre est détruit après la transformation (le robot le remplace).")]
    public bool destroyMonsterAfterTransition = true;

    [Header("Robot Setup (for Ending)")]
    [Tooltip("Si true, ajoute automatiquement RobotHealth au robot pour l'ending.")]
    public bool setupRobotForEnding = true;

    [Tooltip("Tag à assigner au robot (pour détection par le couteau).")]
    public string robotTag = "Robot";

    [Tooltip("Phrase que le robot dit avant de mourir.")]
    [TextArea(2, 4)]
    public string robotDyingWords = "Non... pas comme ça... Je ne faisais que... corriger vos TAs...";

    [Header("Debug")]
    [Tooltip("Afficher les logs de transformation.")]
    public bool debugMode = false;

    // État interne
    private bool hasTransformed = false;
    private GameObject spawnedRobot;
    private MonsterAI monsterAI;
    private Renderer[] monsterRenderers;
    private MonsterScreamerController screamerController;
    
    // Capture de la position au moment du trigger
    private Vector3 capturedPosition;
    private Quaternion capturedRotation;

    public enum TransitionType
    {
        CrossFade,      // Fade out monstre + fade in robot simultanément
        Instant,        // Swap immédiat
        FlashTransition // Flash blanc puis swap
    }

    void Start()
    {
        // Récupérer les components
        monsterAI = GetComponent<MonsterAI>();
        screamerController = GetComponent<MonsterScreamerController>();
        monsterRenderers = GetComponentsInChildren<Renderer>();

        // Setup audio
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f;
            }
        }

        if (debugMode)
        {
            Debug.Log($"MonsterToRobotTransition: Initialized on '{name}'. Waiting for transformation trigger.");
        }
    }

    void Update()
    {
        // Vérifier si la transformation doit être déclenchée
        if (isTransforming && !hasTransformed)
        {
            TriggerTransformation();
        }
    }

    /// <summary>
    /// Déclenche la transformation monstre → robot.
    /// Peut être appelé depuis un autre script ou en mettant isTransforming = true.
    /// </summary>
    public void TriggerTransformation()
    {
        if (hasTransformed)
        {
            if (debugMode)
            {
                Debug.LogWarning("MonsterToRobotTransition: Transformation already completed.");
            }
            return;
        }

        if (robotModel == null)
        {
            Debug.LogError("MonsterToRobotTransition: Aucun robotModel assigné! Assigne le prefab robot dans l'Inspector.");
            return;
        }

        hasTransformed = true;
        isTransforming = true;

        // CAPTURE la position ACTUELLE du monstre (en temps réel)
        capturedPosition = transform.position;
        capturedRotation = transform.rotation;

        if (debugMode)
        {
            Debug.Log($"MonsterToRobotTransition: Starting transformation at position {capturedPosition} (current monster position)");
        }

        // Freeze monster
        if (freezeMonsterDuringTransition)
        {
            FreezeMonster();
        }

        // Start transformation coroutine
        StartCoroutine(TransformationSequence());
    }

    IEnumerator TransformationSequence()
    {
        // Play sound
        if (transformationSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(transformationSound);
        }

        // Play particles at the captured transformation position
        if (transformationParticles != null)
        {
            // Move particles to transformation position if they're not already there
            if (transformationParticles.transform.parent != transform)
            {
                transformationParticles.transform.position = capturedPosition;
            }
            transformationParticles.Play();
            
            if (debugMode)
            {
                Debug.Log($"MonsterToRobotTransition: Playing particles at {capturedPosition}");
            }
        }

        // Execute transition based on type
        switch (transitionType)
        {
            case TransitionType.CrossFade:
                yield return StartCoroutine(CrossFadeTransition());
                break;

            case TransitionType.FlashTransition:
                yield return StartCoroutine(FlashTransition());
                break;

            case TransitionType.Instant:
                InstantTransition();
                break;
        }

        // Spawn/activate robot
        SpawnRobot();

        // Post-transformation cleanup
        PostTransformationCleanup();

        if (debugMode)
        {
            Debug.Log("MonsterToRobotTransition: Transformation complete!");
        }
    }

    IEnumerator CrossFadeTransition()
    {
        float elapsed = 0f;

        // Prepare robot (instantiate but keep invisible)
        GameObject robot = PrepareRobot();
        Renderer[] robotRenderers = robot.GetComponentsInChildren<Renderer>();
        SetRenderersAlpha(robotRenderers, 0f);

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            // Fade out monster
            SetRenderersAlpha(monsterRenderers, 1f - t);

            // Fade in robot
            SetRenderersAlpha(robotRenderers, t);

            yield return null;
        }

        // Ensure final state
        SetRenderersAlpha(monsterRenderers, 0f);
        SetRenderersAlpha(robotRenderers, 1f);
    }

    IEnumerator FlashTransition()
    {
        // Flash effect (implementation simplifiée, peut être amélioré avec post-processing)
        float flashDuration = transitionDuration * 0.3f;
        float elapsed = 0f;

        // Fade to white
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;
            SetRenderersColor(monsterRenderers, Color.Lerp(Color.white, transitionColor, t));
            yield return null;
        }

        // Swap
        SetRenderersAlpha(monsterRenderers, 0f);
        GameObject robot = PrepareRobot();

        // Fade from white
        elapsed = 0f;
        Renderer[] robotRenderers = robot.GetComponentsInChildren<Renderer>();
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;
            SetRenderersColor(robotRenderers, Color.Lerp(transitionColor, Color.white, t));
            yield return null;
        }

        SetRenderersColor(robotRenderers, Color.white);
    }

    void InstantTransition()
    {
        SetRenderersAlpha(monsterRenderers, 0f);
        PrepareRobot();
    }

    GameObject PrepareRobot()
    {
        if (spawnedRobot != null)
            return spawnedRobot;

        // Utilise la position CAPTURÉE au moment du trigger (pas la position de départ)
        Vector3 spawnPos = inheritTransform ? capturedPosition : robotModel.transform.position;
        Quaternion spawnRot = inheritTransform ? capturedRotation : robotModel.transform.rotation;

        // Ajoute l'offset en Y pour éviter que le robot soit dans le sol
        spawnPos.y += robotHeightOffset;

        if (instantiateRobot)
        {
            // Instantiate robot at captured monster position (avec offset Y)
            spawnedRobot = Instantiate(robotModel, spawnPos, spawnRot);
            spawnedRobot.name = "Robot_Desensorcelé";

            if (debugMode)
            {
                Debug.Log($"MonsterToRobotTransition: Robot instantiated at CAPTURED position {spawnPos} (with Y offset {robotHeightOffset}), rotation {spawnRot.eulerAngles}");
            }
        }
        else
        {
            // Activate existing robot child
            spawnedRobot = transform.Find(robotModel.name)?.gameObject;
            if (spawnedRobot == null)
            {
                Debug.LogError($"MonsterToRobotTransition: Robot child '{robotModel.name}' not found! Set instantiateRobot to true or place robot as child.");
                spawnedRobot = Instantiate(robotModel, spawnPos, spawnRot);
            }
            else
            {
                // Place at captured position (avec offset Y)
                spawnedRobot.transform.position = spawnPos;
                spawnedRobot.transform.rotation = spawnRot;
            }
            spawnedRobot.SetActive(true);
        }

        // Setup robot for ending (add RobotHealth, tag, collider)
        if (setupRobotForEnding)
        {
            SetupRobotComponents();
        }

        return spawnedRobot;
    }

    void SetupRobotComponents()
    {
        if (spawnedRobot == null)
            return;

        // Set tag (with safety check)
        if (!string.IsNullOrEmpty(robotTag))
        {
            try
            {
                spawnedRobot.tag = robotTag;
                if (debugMode)
                {
                    Debug.Log($"MonsterToRobotTransition: Robot tagged as '{robotTag}'");
                }
            }
            catch (UnityException e)
            {
                Debug.LogWarning($"MonsterToRobotTransition: Could not set tag '{robotTag}' (tag doesn't exist in Unity Tags). The RobotHealth component will still work. Error: {e.Message}");
            }
        }

        // Ensure collider exists
        Collider col = spawnedRobot.GetComponent<Collider>();
        if (col == null)
        {
            // Try to find in children
            col = spawnedRobot.GetComponentInChildren<Collider>();
            
            if (col == null)
            {
                // Add a capsule collider as fallback
                CapsuleCollider capsule = spawnedRobot.AddComponent<CapsuleCollider>();
                capsule.height = 2f;
                capsule.radius = 0.5f;
                capsule.center = new Vector3(0, 1f, 0);
                
                if (debugMode)
                {
                    Debug.Log("MonsterToRobotTransition: Added CapsuleCollider to robot.");
                }
            }
        }

        // Add RobotHealth component
        RobotHealth robotHealth = spawnedRobot.GetComponent<RobotHealth>();
        if (robotHealth == null)
        {
            robotHealth = spawnedRobot.AddComponent<RobotHealth>();
            robotHealth.maxHealth = 1;
            robotHealth.dyingWords = robotDyingWords;
            robotHealth.deathDelay = 1f;
            robotHealth.debugMode = debugMode;
            
            if (debugMode)
            {
                Debug.Log("MonsterToRobotTransition: Added RobotHealth component to robot.");
            }
        }
        else
        {
            // Update existing component
            robotHealth.dyingWords = robotDyingWords;
            
            if (debugMode)
            {
                Debug.Log("MonsterToRobotTransition: RobotHealth already exists, updated dying words.");
            }
        }
    }

    void SpawnRobot()
    {
        if (spawnedRobot == null)
        {
            PrepareRobot();
        }
    }

    void PostTransformationCleanup()
    {
        // Disable AI components
        if (disableAIAfterTransition)
        {
            if (monsterAI != null)
            {
                monsterAI.enabled = false;
            }
            if (screamerController != null)
            {
                screamerController.enabled = false;
            }
        }

        // Stop particles
        if (transformationParticles != null && transformationParticles.isPlaying)
        {
            transformationParticles.Stop();
        }

        // Hide or destroy monster
        if (destroyMonsterAfterTransition)
        {
            if (debugMode)
            {
                Debug.Log("MonsterToRobotTransition: Destroying monster GameObject in 1 second...");
            }
            // Hide immediately then destroy after delay
            foreach (var renderer in monsterRenderers)
            {
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }
            Destroy(gameObject, 1f); // Delay to ensure transition completes
        }
        else
        {
            // Just hide the monster
            foreach (var renderer in monsterRenderers)
            {
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }
            if (debugMode)
            {
                Debug.Log("MonsterToRobotTransition: Monster hidden (not destroyed).");
            }
        }
    }

    void FreezeMonster()
    {
        // Disable AI
        if (monsterAI != null)
        {
            monsterAI.enabled = false;
        }

        // Disable NavMeshAgent (only if it's properly initialized)
        var navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null && navAgent.enabled)
        {
            try
            {
                // Only stop if agent is on a NavMesh
                if (navAgent.isOnNavMesh)
                {
                    navAgent.isStopped = true;
                }
            }
            catch (System.Exception e)
            {
                if (debugMode)
                {
                    Debug.LogWarning($"MonsterToRobotTransition: Could not stop NavMeshAgent: {e.Message}");
                }
            }
            finally
            {
                // Always try to disable it
                navAgent.enabled = false;
            }
        }

        // Disable screamer controller
        if (screamerController != null)
        {
            screamerController.enabled = false;
        }

        if (debugMode)
        {
            Debug.Log("MonsterToRobotTransition: Monster frozen for transformation.");
        }
    }

    void SetRenderersAlpha(Renderer[] renderers, float alpha)
    {
        if (renderers == null) return;

        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;

            foreach (var mat in renderer.materials)
            {
                if (mat == null) continue;

                // Check if material supports transparency
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.color;
                    color.a = alpha;
                    mat.color = color;

                    // Enable transparency rendering if needed
                    if (alpha < 1f && mat.renderQueue < 3000)
                    {
                        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        mat.SetInt("_ZWrite", 0);
                        mat.DisableKeyword("_ALPHATEST_ON");
                        mat.EnableKeyword("_ALPHABLEND_ON");
                        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        mat.renderQueue = 3000;
                    }
                }
            }
        }
    }

    void SetRenderersColor(Renderer[] renderers, Color color)
    {
        if (renderers == null) return;

        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;

            foreach (var mat in renderer.materials)
            {
                if (mat != null && mat.HasProperty("_Color"))
                {
                    mat.color = color;
                }
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Test Transformation (Play Mode Only)")]
    private void TestTransformation()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("MonsterToRobotTransition: This test only works in Play mode.");
            return;
        }

        TriggerTransformation();
    }

    [ContextMenu("Reset Transformation (for testing)")]
    private void ResetTransformation()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("MonsterToRobotTransition: This reset only works in Play mode.");
            return;
        }

        hasTransformed = false;
        isTransforming = false;

        if (spawnedRobot != null)
        {
            Destroy(spawnedRobot);
        }

        SetRenderersAlpha(monsterRenderers, 1f);

        if (monsterAI != null)
        {
            monsterAI.enabled = true;
        }

        Debug.Log("MonsterToRobotTransition: Reset complete. You can test again.");
    }
#endif
}
