using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Système d'arme couteau : détecte les attaques (clic gauche ou trigger Attack depuis HandAnimationController)
/// et inflige des dégâts au robot via raycast ou collision.
/// Attaché à l'objet couteau dans les mains du joueur.
/// </summary>
public class KnifeWeapon : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Dégâts infligés par coup de couteau.")]
    public int damage = 1;

    [Tooltip("Portée du raycast pour détecter le robot (mètres).")]
    public float attackRange = 2f;

    [Tooltip("Layer du robot pour filtrer le raycast (optionnel).")]
    public LayerMask robotLayer = ~0; // Default: all layers

    [Tooltip("Tag du robot pour identifier la cible.")]
    public string robotTag = "Robot";

    [Header("Attack Input")]
    [Tooltip("Si true, attaque sur clic gauche. Si false, utilise l'animation Attack du HandAnimationController.")]
    public bool useMouseClick = true;

    [Tooltip("Cooldown entre chaque attaque (secondes).")]
    public float attackCooldown = 0.5f;

    [Header("Visual Feedback")]
    [Tooltip("Particules jouées lors d'une attaque réussie (sang, étincelles, etc).")]
    public ParticleSystem hitEffect;

    [Tooltip("Son joué lors d'une attaque.")]
    public AudioClip attackSound;

    [Tooltip("Son joué quand l'attaque touche le robot.")]
    public AudioClip hitSound;

    [Header("Camera Reference")]
    [Tooltip("Caméra du joueur pour le raycast. Auto-trouvée si null.")]
    public Camera playerCamera;

    [Header("Debug")]
    [Tooltip("Dessiner le raycast d'attaque dans la Scene view.")]
    public bool drawDebugRay = true;

    [Tooltip("Afficher les logs d'attaque.")]
    public bool debugMode = false;

    private AudioSource audioSource;
    private float lastAttackTime = 0f;
    private bool canAttack = true;

    void Start()
    {
        // Find camera
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogWarning("KnifeWeapon: No camera found. Assign playerCamera manually.");
            }
        }

        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D sound for weapon
        }

        if (debugMode)
        {
            Debug.Log($"KnifeWeapon: Initialized. Attack range: {attackRange}m, Damage: {damage}");
        }
    }

    void Update()
    {
        // Check cooldown
        if (Time.time - lastAttackTime < attackCooldown)
        {
            canAttack = false;
        }
        else
        {
            canAttack = true;
        }

        // Attack input
        if (useMouseClick && canAttack)
        {
            bool attackPressed = false;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            var mouse = Mouse.current;
            if (mouse != null && mouse.rightButton.wasPressedThisFrame)
                attackPressed = true;
#else
            if (Input.GetMouseButtonDown(1)) // Right click
                attackPressed = true;
#endif

            if (attackPressed)
            {
                Attack();
            }
        }
    }

    /// <summary>
    /// Exécute une attaque au couteau (peut être appelé depuis HandAnimationController ou autre script).
    /// </summary>
    public void Attack()
    {
        if (!canAttack)
        {
            if (debugMode)
            {
                Debug.Log("KnifeWeapon: Attack on cooldown.");
            }
            return;
        }

        lastAttackTime = Time.time;

        if (debugMode)
        {
            Debug.Log("KnifeWeapon: Attack executed!");
        }

        // Play attack sound
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        // Raycast from camera center
        if (playerCamera != null)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of screen
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, attackRange, robotLayer))
            {
                if (debugMode)
                {
                    Debug.Log($"KnifeWeapon: Raycast hit '{hit.collider.name}' (tag: '{hit.collider.tag}') at distance {hit.distance:F2}m");
                }

                // Check if we hit the robot (prioritize RobotHealth component over tag)
                RobotHealth robotHealth = hit.collider.GetComponent<RobotHealth>();
                if (robotHealth == null)
                {
                    robotHealth = hit.collider.GetComponentInParent<RobotHealth>();
                }

                bool hasRobotTag = false;
                try
                {
                    hasRobotTag = hit.collider.CompareTag(robotTag);
                }
                catch
                {
                    // Tag doesn't exist in Unity, ignore
                }

                if (robotHealth != null || hasRobotTag)
                {
                    if (debugMode)
                    {
                        Debug.Log($"KnifeWeapon: Detected robot! (RobotHealth: {robotHealth != null}, Tag: {hasRobotTag})");
                    }
                    OnHitRobot(hit);
                }
                else if (debugMode)
                {
                    Debug.Log($"KnifeWeapon: Hit '{hit.collider.name}' but it's not a robot (no RobotHealth and tag is '{hit.collider.tag}')");
                }
            }
            else if (debugMode)
            {
                Debug.Log("KnifeWeapon: Raycast missed (no hit in range).");
            }
        }
    }

    void OnHitRobot(RaycastHit hit)
    {
        if (debugMode)
        {
            Debug.Log($"KnifeWeapon: HIT ROBOT! Dealing {damage} damage.");
        }

        // Deal damage
        RobotHealth robotHealth = hit.collider.GetComponent<RobotHealth>();
        if (robotHealth == null)
        {
            // Try to find on parent
            robotHealth = hit.collider.GetComponentInParent<RobotHealth>();
        }

        if (robotHealth != null)
        {
            robotHealth.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning($"KnifeWeapon: Hit robot '{hit.collider.name}' but no RobotHealth component found!");
        }

        // Play hit sound
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Spawn hit effect
        if (hitEffect != null)
        {
            ParticleSystem effect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            effect.Play();
            Destroy(effect.gameObject, 2f);
        }
    }

    void OnDrawGizmos()
    {
        if (!drawDebugRay || playerCamera == null)
            return;

        // Draw attack range from camera
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Gizmos.color = canAttack ? Color.green : Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction * attackRange);
    }

#if UNITY_EDITOR
    [ContextMenu("Test Attack (Play Mode)")]
    private void TestAttack()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("KnifeWeapon: This test only works in Play mode.");
            return;
        }

        Attack();
    }
#endif
}
