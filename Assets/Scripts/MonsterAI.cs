using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public Animator animator;
    public Transform forestZone;
    
    [Header("Détection et Combat")]
    public float detectionRange = 20f;
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;
    
    [Header("Mouvement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float roamingRadius = 30f;
    public float roamingWaitTime = 3f;
    
    [Header("Audio")]
    public AudioClip caughtSound;
    [Range(0f, 1f)]
    public float caughtSoundVolume = 0.8f;
    
    [Header("Animation")]
    public string speedParameter = "Speed";
    public string isRunningParameter = "IsRunning";
    public string isWalkingParameter = "IsWalking";
    public string attackTrigger = "Attack";
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private enum State { Roaming, Chase, Attack, ReturnToForest }
    private State currentState = State.Roaming;
    
    private AudioSource audioSource;
    private Vector3 currentRoamTarget;
    private float roamWaitTimer = 0f;
    private bool isWaiting = false;
    private bool hasPlayedSound = false;
    private float lastAttackTime = 0f;
    private float currentSpeed = 0f;
    
    [HideInInspector]
    public bool isPlayerInHouse = false;

    void Start()
    {
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
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
        }
        
        if (forestZone == null)
        {
            GameObject fz = new GameObject("ForestZone");
            forestZone = fz.transform;
            forestZone.position = transform.position;
        }

        SetNewRoamTarget();
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        
        switch (currentState)
        {
            case State.Roaming:
                if (dist <= detectionRange && !isPlayerInHouse)
                {
                    SwitchToChase();
                }
                else
                {
                    if (isWaiting)
                    {
                        roamWaitTimer -= Time.deltaTime;
                        if (roamWaitTimer <= 0f)
                        {
                            isWaiting = false;
                            SetNewRoamTarget();
                        }
                        currentSpeed = 0f;
                    }
                    else
                    {
                        MoveTowards(currentRoamTarget, walkSpeed);
                        if (Vector3.Distance(transform.position, currentRoamTarget) < 1f)
                        {
                            isWaiting = true;
                            roamWaitTimer = roamingWaitTime;
                        }
                    }
                }
                break;
                
            case State.Chase:
                if (isPlayerInHouse)
                {
                    SwitchToReturn();
                }
                else if (dist <= attackRange)
                {
                    currentState = State.Attack;
                }
                else
                {
                    MoveTowards(player.position, runSpeed);
                }
                break;
                
            case State.Attack:
                if (isPlayerInHouse)
                {
                    SwitchToReturn();
                }
                else if (dist > attackRange + 1f)
                {
                    currentState = State.Chase;
                }
                else
                {
                    LookAt(player.position);
                    currentSpeed = 0f;
                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        Attack();
                        lastAttackTime = Time.time;
                    }
                }
                break;
                
            case State.ReturnToForest:
                if (!isPlayerInHouse && dist <= detectionRange)
                {
                    SwitchToChase();
                }
                else if (Vector3.Distance(transform.position, forestZone.position) < 2f)
                {
                    currentState = State.Roaming;
                    hasPlayedSound = false;
                    SetNewRoamTarget();
                }
                else
                {
                    MoveTowards(forestZone.position, walkSpeed);
                }
                break;
        }

        UpdateAnimations();
    }

    void SwitchToChase()
    {
        currentState = State.Chase;
        if (!hasPlayedSound && caughtSound && audioSource)
        {
            audioSource.PlayOneShot(caughtSound, caughtSoundVolume);
            hasPlayedSound = true;
        }
    }

    void SwitchToReturn()
    {
        currentState = State.ReturnToForest;
        hasPlayedSound = false;
    }

    void Attack()
    {
        // Animation d'attaque
        if (animator)
        {
            // Vérifier si le trigger existe
            bool hasTrigger = false;
            foreach (var param in animator.parameters)
            {
                if (param.name == attackTrigger && param.type == AnimatorControllerParameterType.Trigger)
                {
                    hasTrigger = true;
                    break;
                }
            }
            
            if (hasTrigger)
            {
                animator.SetTrigger(attackTrigger);
            }
            else if (showDebugInfo)
            {
                Debug.LogWarning($"Le trigger '{attackTrigger}' n'existe pas dans l'Animator!");
            }
        }
        
        // Infliger les dégâts
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph)
        {
            ph.TakeDamage(1);
            if (showDebugInfo) Debug.Log("Monster attacks player!");
        }
        else if (showDebugInfo)
        {
            Debug.LogWarning("PlayerHealth not found on player!");
        }
    }

    void SetNewRoamTarget()
    {
        Vector2 rnd = Random.insideUnitCircle * roamingRadius;
        currentRoamTarget = forestZone.position + new Vector3(rnd.x, 0, rnd.y);
    }

    void MoveTowards(Vector3 target, float speed)
    {
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
        bool running = currentState == State.Chase && moving;
        bool walking = (currentState == State.Roaming || currentState == State.ReturnToForest) && moving;
        
        float speed = running ? 1f : (walking ? 0.5f : 0f);
        
        animator.SetFloat(speedParameter, speed);
        animator.SetBool(isRunningParameter, running);
        animator.SetBool(isWalkingParameter, walking);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (forestZone)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(forestZone.position, roamingRadius);
        }
    }
}
