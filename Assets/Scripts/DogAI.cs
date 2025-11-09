using UnityEngine;
using UnityEngine.AI; // Pour la navigation intelligente

public class DogAI : MonoBehaviour
{
    [Header("Références")]
    public Transform player; // Le joueur à suivre
    public Transform houseZone; // Zone de la maison à protéger
    public Animator animator;
    public NavMeshAgent navAgent; // Agent de navigation pour éviter les obstacles
    
    [Header("Détection du Monstre")]
    [Tooltip("Distance à laquelle le chien détecte le monstre près de la maison")]
    public float monsterDetectionRange = 20f;
    [Tooltip("Distance à laquelle le chien aboie sur le monstre")]
    public float barkRange = 10f;
    
    [Header("Suivi du Joueur")]
    [Tooltip("Distance minimale avec le joueur")]
    public float followDistance = 3f;
    [Tooltip("Distance maximale avant de suivre")]
    public float maxFollowDistance = 10f;
    
    [Header("Navigation")]
    [Tooltip("Utiliser NavMesh pour éviter les obstacles (murs, portes)")]
    public bool useNavMesh = true;
    
    [Header("Mouvement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float roamingRadius = 20f;
    public float roamingWaitTime = 4f;
    
    [Header("Audio")]
    public AudioClip barkSound;
    public AudioClip growlSound;
    [Range(0f, 1f)]
    public float barkSoundVolume = 0.8f;
    
    [Header("Animation")]
    public string isWalkingParameter = "IsWalking";
    public string barkTrigger = "Bark";
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private enum State { FollowPlayer, Alert, Bark }
    private State currentState = State.FollowPlayer;
    
    private Transform detectedMonster = null;
    
    private AudioSource audioSource;
    private Vector3 currentRoamTarget;
    private float roamWaitTimer = 0f;
    private bool isWaiting = false;
    private float lastBarkTime = 0f;
    private float barkCooldown = 3f;
    private float currentSpeed = 0f;

    void Start()
    {
        // Trouve le joueur
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
            else
            {
                FirstPersonMovement fpm = FindFirstObjectByType<FirstPersonMovement>();
                if (fpm) player = fpm.transform;
            }
        }
        
        if (animator == null) animator = GetComponentInChildren<Animator>();
        
        // Configure NavMeshAgent si disponible
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null && useNavMesh)
        {
            navAgent.speed = walkSpeed;
            navAgent.acceleration = 8f;
            navAgent.angularSpeed = 120f;
            navAgent.stoppingDistance = followDistance;
            if (showDebugInfo) Debug.Log("✅ NavMesh activé - Le chien évitera les obstacles !");
        }
        else if (useNavMesh)
        {
            Debug.LogWarning("⚠️ NavMeshAgent manquant ! Ajoutez-le pour la navigation intelligente.");
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
        }
        
        if (houseZone == null)
        {
            GameObject hz = new GameObject("HouseZone");
            houseZone = hz.transform;
            houseZone.position = transform.position;
        }

        SetNewRoamTarget();
        
        if (showDebugInfo)
            Debug.Log("🐕 Chien initialisé - Allié du joueur, protège la maison !");
    }

    void Update()
    {
        if (player == null) return;
        
        // Cherche le monstre dans la scène
        DetectMonster();

        switch (currentState)
        {
            case State.FollowPlayer:
                // Priorité 1 : Si un monstre est détecté (à 20m du chien)
                if (detectedMonster != null)
                {
                    float distToMonster = Vector3.Distance(transform.position, detectedMonster.position);
                    
                    if (distToMonster <= monsterDetectionRange)
                    {
                        SwitchToAlert();
                        break;
                    }
                }
                
                // Priorité 2 : Suivre le joueur (au pied)
                float distToPlayer = Vector3.Distance(transform.position, player.position);
                
                if (distToPlayer > maxFollowDistance)
                {
                    // Trop loin, court vers le joueur
                    MoveTowards(player.position, runSpeed);
                }
                else if (distToPlayer > followDistance)
                {
                    // Distance normale, marche vers le joueur
                    MoveTowards(player.position, walkSpeed);
                }
                else
                {
                    // Au pied du joueur, reste immobile
                    currentSpeed = 0f;
                }
                break;
                
            case State.Alert:
                if (detectedMonster == null)
                {
                    // Le monstre a disparu, retour au joueur
                    currentState = State.FollowPlayer;
                    if (showDebugInfo) Debug.Log("🐕 Monstre disparu, retour au joueur");
                }
                else
                {
                    float distToMonster = Vector3.Distance(transform.position, detectedMonster.position);
                    
                    if (distToMonster > monsterDetectionRange)
                    {
                        // Le monstre s'est éloigné, retour au joueur
                        currentState = State.FollowPlayer;
                        if (showDebugInfo) Debug.Log("🐕 Monstre éloigné, retour au joueur");
                    }
                    else if (distToMonster <= barkRange)
                    {
                        // Assez proche pour aboyer
                        currentState = State.Bark;
                    }
                    else
                    {
                        // Reste en alerte, regarde le monstre
                        LookAt(detectedMonster.position);
                        currentSpeed = 0f;
                    }
                }
                break;
                
            case State.Bark:
                if (detectedMonster == null)
                {
                    currentState = State.FollowPlayer;
                }
                else
                {
                    float distToMonster = Vector3.Distance(transform.position, detectedMonster.position);
                    
                    if (distToMonster > monsterDetectionRange)
                    {
                        // Le monstre s'est trop éloigné, retour au joueur
                        currentState = State.FollowPlayer;
                        if (showDebugInfo) Debug.Log("🐕 Monstre parti ! Retour au joueur");
                    }
                    else if (distToMonster > barkRange * 1.5f)
                    {
                        // Le monstre s'éloigne un peu
                        currentState = State.Alert;
                    }
                    else
                    {
                        // Continue d'aboyer sur le monstre
                        LookAt(detectedMonster.position);
                        currentSpeed = 0f;
                        
                        if (Time.time >= lastBarkTime + barkCooldown)
                        {
                            Bark();
                            lastBarkTime = Time.time;
                        }
                    }
                }
                break;
        }

        UpdateAnimations();
    }

    void DetectMonster()
    {
        // Cherche tous les objets avec le script MonsterAI
        MonsterAI monster = FindFirstObjectByType<MonsterAI>();
        
        if (monster != null)
        {
            detectedMonster = monster.transform;
        }
        else
        {
            detectedMonster = null;
        }
    }

    void SwitchToAlert()
    {
        currentState = State.Alert;
        
        if (growlSound && audioSource)
        {
            audioSource.PlayOneShot(growlSound, barkSoundVolume * 0.5f);
        }
        
        if (showDebugInfo) 
            Debug.Log("🐕 ALERTE ! Monstre détecté près de la maison !");
    }

    void Bark()
    {
        // Animation d'aboiement
        if (animator)
        {
            // Vérifier si le trigger existe
            bool hasTrigger = false;
            foreach (var param in animator.parameters)
            {
                if (param.name == barkTrigger && param.type == AnimatorControllerParameterType.Trigger)
                {
                    hasTrigger = true;
                    break;
                }
            }
            
            if (hasTrigger)
            {
                animator.SetTrigger(barkTrigger);
            }
            else if (showDebugInfo)
            {
                Debug.LogWarning($"Le trigger '{barkTrigger}' n'existe pas dans l'Animator!");
            }
        }
        
        // Jouer le son d'aboiement
        if (barkSound && audioSource)
        {
            audioSource.PlayOneShot(barkSound, barkSoundVolume);
            if (showDebugInfo) Debug.Log("🐕 WOOF WOOF ! (Alerte monstre !)");
        }
    }

    void SetNewRoamTarget()
    {
        Vector2 rnd = Random.insideUnitCircle * roamingRadius;
        currentRoamTarget = houseZone.position + new Vector3(rnd.x, 0, rnd.y);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        // Si NavMesh est activé, utilise la navigation intelligente
        if (useNavMesh && navAgent != null && navAgent.enabled && navAgent.isOnNavMesh)
        {
            navAgent.speed = speed;
            navAgent.SetDestination(target);
            
            // Synchronise currentSpeed avec la vitesse réelle de l'agent
            currentSpeed = navAgent.velocity.magnitude;
        }
        else
        {
            // Mouvement basique sans évitement (ancien système)
            Vector3 dir = (target - transform.position).normalized;
            dir.y = 0;
            
            if (dir.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
                transform.position += dir * speed * Time.deltaTime;
                currentSpeed = speed;
            }
            else
            {
                currentSpeed = 0f;
            }
        }
    }

    void LookAt(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0;
        if (dir.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        }
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        bool moving = currentSpeed > 0.1f;
        
        // Utilise IsWalking - Le contrôleur fait automatiquement la transition
        // entre "Breathing" (idle) et "Walking" (marche)
        animator.SetBool(isWalkingParameter, moving);
        
        if (showDebugInfo && Time.frameCount % 60 == 0) // Debug toutes les 60 frames
        {
            Debug.Log($"🐕 Animation: IsWalking={moving}, Speed={currentSpeed:F2}");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Zone de détection du monstre autour de la maison
        if (houseZone)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(houseZone.position, monsterDetectionRange);
        }
        
        // Zone d'aboiement du chien
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, barkRange);
        
        // Zone de patrouille
        if (houseZone)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(houseZone.position, roamingRadius);
        }
        
        // Ligne vers le monstre détecté
        if (detectedMonster != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, detectedMonster.position);
        }
    }
}
