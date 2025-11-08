using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Contrôleur IA pour le MonsterMutant7
/// Le monstre spawn à côté du joueur et court vers lui
/// </summary>
public class MonsterAI : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Le Transform du joueur (sera trouvé automatiquement si non assigné)")]
    public Transform player;
    
    [Tooltip("L'Animator du monstre")]
    public Animator animator;
    
    [Header("Paramètres de mouvement")]
    [Tooltip("Vitesse de marche du monstre")]
    public float walkSpeed = 2f;
    
    [Tooltip("Vitesse de course du monstre")]
    public float runSpeed = 3.5f;
    
    [Tooltip("Distance à laquelle le monstre s'arrête du joueur")]
    public float stoppingDistance = 2f;
    
    [Tooltip("Distance à partir de laquelle le monstre court (sinon il marche)")]
    public float runDistance = 10f;
    
    [Header("Paramètres d'animation")]
    [Tooltip("Nom du paramètre de vitesse dans l'Animator")]
    public string speedParameterName = "Speed";
    
    [Tooltip("Nom du paramètre booléen pour la course dans l'Animator")]
    public string isRunningParameterName = "IsRunning";
    
    [Tooltip("Nom du paramètre booléen pour la marche dans l'Animator")]
    public string isWalkingParameterName = "IsWalking";
    
    [Header("Debug")]
    [Tooltip("Afficher les informations de debug dans la console")]
    public bool showDebugInfo = false;
    
    [Tooltip("Afficher les informations dans la scène (3D Text)")]
    public bool showDebugInScene = false;
    
    private NavMeshAgent navAgent;
    private bool isInitialized = false;
    private float currentSpeed = 0f;

    void Start()
    {
        InitializeMonster();
    }

    void InitializeMonster()
    {
        // Trouver le joueur si non assigné
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                // Chercher par le component FirstPersonMovement
                FirstPersonMovement fpm = FindFirstObjectByType<FirstPersonMovement>();
                if (fpm != null)
                {
                    player = fpm.transform;
                }
                else
                {
                    Debug.LogWarning("MonsterAI: Joueur non trouvé! Assurez-vous qu'il a le tag 'Player' ou un component FirstPersonMovement.");
                    return;
                }
            }
        }

        // Obtenir ou ajouter l'Animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("MonsterAI: Aucun Animator trouvé sur le monstre!");
            }
        }

        // Obtenir ou ajouter NavMeshAgent
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
            Debug.Log("MonsterAI: NavMeshAgent ajouté automatiquement au monstre.");
        }

        // Configurer le NavMeshAgent
        navAgent.speed = runSpeed; // Vitesse max par défaut
        navAgent.stoppingDistance = stoppingDistance;
        navAgent.acceleration = 8f;
        navAgent.angularSpeed = 120f;

        isInitialized = true;
        Debug.Log("MonsterAI: Monstre initialisé et prêt à poursuivre le joueur!");
    }

    void Update()
    {
        if (!isInitialized || player == null)
            return;

        // Se déplacer vers le joueur
        MoveTowardsPlayer();

        // Mettre à jour les animations
        UpdateAnimations();
    }

    void MoveTowardsPlayer()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Déterminer si le monstre doit courir ou marcher
        bool shouldRun = distanceToPlayer > runDistance;
        float targetSpeed = shouldRun ? runSpeed : walkSpeed;
        
        if (navAgent != null && navAgent.enabled && navAgent.isOnNavMesh)
        {
            // Utiliser NavMeshAgent seulement s'il est sur un NavMesh
            navAgent.SetDestination(player.position);
            
            // Ajuster la vitesse en fonction de la distance
            if (distanceToPlayer > stoppingDistance)
            {
                navAgent.speed = targetSpeed;
                currentSpeed = navAgent.velocity.magnitude;
            }
            else
            {
                navAgent.speed = 0f;
                currentSpeed = 0f;
            }
        }
        else
        {
            // Méthode de secours sans NavMeshAgent ou si pas sur NavMesh
            if (distanceToPlayer > stoppingDistance)
            {
                // Regarder vers le joueur
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                // Se déplacer vers le joueur
                transform.position += transform.forward * targetSpeed * Time.deltaTime;
                currentSpeed = targetSpeed;
            }
            else
            {
                currentSpeed = 0f;
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"Distance: {distanceToPlayer:F2}m | Speed: {currentSpeed:F2} | Running: {shouldRun}");
        }
    }

    void UpdateAnimations()
    {
        if (animator == null || player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Déterminer l'état du monstre
        bool isMoving = currentSpeed > 0.1f;
        bool isRunning = isMoving && distanceToPlayer > runDistance;
        bool isWalking = isMoving && !isRunning;
        
        // IMPORTANT: Normaliser la vitesse pour l'animator (0-1)
        float normalizedSpeed = isRunning ? 1f : (isWalking ? 0.5f : 0f);
        
        // Mettre à jour le paramètre Speed (Float)
        if (HasParameter(speedParameterName, AnimatorControllerParameterType.Float))
        {
            animator.SetFloat(speedParameterName, normalizedSpeed);
        }
        
        // Mettre à jour le paramètre IsRunning (Bool)
        if (HasParameter(isRunningParameterName, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(isRunningParameterName, isRunning);
        }
        
        // Mettre à jour le paramètre IsWalking (Bool)
        if (HasParameter(isWalkingParameterName, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(isWalkingParameterName, isWalking);
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"Animation State - Speed: {normalizedSpeed:F2} | Walking: {isWalking} | Running: {isRunning}");
            
            // Afficher l'état actuel de l'animator
            if (animator.isActiveAndEnabled)
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                Debug.Log($"Current Animation State: {stateInfo.shortNameHash} | Time: {stateInfo.normalizedTime:F2}");
            }
        }
    }
    
    // Vérifier si un paramètre existe dans l'Animator
    bool HasParameter(string paramName, AnimatorControllerParameterType paramType)
    {
        if (animator == null || string.IsNullOrEmpty(paramName))
            return false;
            
        foreach (var param in animator.parameters)
        {
            if (param.name == paramName && param.type == paramType)
            {
                return true;
            }
        }
        return false;
    }

    // Afficher le rayon de détection dans l'éditeur
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        }
    }
    
    // Afficher les infos debug dans la scène
    void OnGUI()
    {
        if (!showDebugInScene || !Application.isPlaying)
            return;
            
        if (animator == null)
            return;
        
        // Calculer la position 2D du monstre
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 3f);
        
        if (screenPos.z > 0)
        {
            // Créer un style
            GUIStyle style = new GUIStyle();
            style.fontSize = 14;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            
            // Fond noir semi-transparent
            Rect bgRect = new Rect(screenPos.x - 100, Screen.height - screenPos.y - 60, 200, 120);
            GUI.Box(bgRect, "");
            
            // Texte d'information
            string info = $"MONSTRE DEBUG\n\n";
            info += $"Speed: {animator.GetFloat("Speed"):F2}\n";
            info += $"IsWalking: {animator.GetBool("IsWalking")}\n";
            info += $"IsRunning: {animator.GetBool("IsRunning")}\n";
            
            if (animator.isActiveAndEnabled)
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                info += $"Time: {stateInfo.normalizedTime:F2}";
            }
            
            GUI.Label(bgRect, info, style);
        }
    }
}
