using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the monster's screamer behavior in the spawn zone and transitions to chase mode.
/// While in spawn zone: teleports to random ScreamerSpawnPoints for brief scary appearances.
/// After exiting spawn zone: spawns near player and activates MonsterAI chase behavior.
/// </summary>
[RequireComponent(typeof(MonsterAI))]
public class MonsterScreamerController : MonoBehaviour
{
    [Header("Screamer Settings")]
    [Tooltip("Time the monster appears at each spawn point (seconds).")]
    public float screamerDisplayDuration = 0.5f;

    [Tooltip("Time between each screamer appearance (seconds).")]
    public float screamerInterval = 3f;

    [Tooltip("Optional: Audio to play when screamer appears.")]
    public AudioClip screamerSound;

    [Tooltip("Optional: AudioSource to play sounds (will create one if null).")]
    public AudioSource audioSource;

    [Header("Spawn Points")]
    [Tooltip("Parent object containing all ScreamerSpawnPoint children. Leave null to auto-find.")]
    public Transform spawnPointsParent;

    [Tooltip("If true, finds all ScreamerSpawnPoint components in scene automatically.")]
    public bool autoFindSpawnPoints = true;

    [Header("Chase Mode")]
    [Tooltip("Specific spawn point for chase mode. If null, uses chaseSpawnOffset from player.")]
    public Transform chaseSpawnPoint;

    [Tooltip("Fallback: Offset from player position if chaseSpawnPoint is not assigned.")]
    public Vector3 chaseSpawnOffset = new Vector3(3f, 0f, 3f);

    [Header("Debug")]
    [Tooltip("Log screamer events to console.")]
    public bool debugMode = false;

    private MonsterAI monsterAI;
    private ScreamerSpawnPoint[] spawnPoints;
    private bool isInScreamerMode = false;
    private Coroutine screamerCoroutine;
    private bool hasStartedChase = false;

    // Renderers to hide/show the monster
    private Renderer[] monsterRenderers;

    void Start()
    {
        // Get MonsterAI component
        monsterAI = GetComponent<MonsterAI>();
        if (monsterAI == null)
        {
            Debug.LogError("MonsterScreamerController: MonsterAI component not found!");
            enabled = false;
            return;
        }

        // Initially disable the MonsterAI chase behavior
        monsterAI.enabled = false;

        // Get all renderers to control visibility
        monsterRenderers = GetComponentsInChildren<Renderer>();

        // Setup audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f; // 3D sound
            }
        }

        // Find spawn points
        FindSpawnPoints();

        // Start hidden
        SetMonsterVisibility(false);

        if (debugMode)
        {
            Debug.Log($"MonsterScreamerController: Initialized with {spawnPoints?.Length ?? 0} spawn points.");
        }
    }

    void FindSpawnPoints()
    {
        if (autoFindSpawnPoints)
        {
            // Try to find parent first
            if (spawnPointsParent == null)
            {
                GameObject parent = GameObject.Find("ScreamerSpawnPoints");
                if (parent != null)
                {
                    spawnPointsParent = parent.transform;
                }
            }

            // Get spawn points from parent or find all in scene
            if (spawnPointsParent != null)
            {
                spawnPoints = spawnPointsParent.GetComponentsInChildren<ScreamerSpawnPoint>();
            }
            else
            {
                spawnPoints = FindObjectsOfType<ScreamerSpawnPoint>();
            }

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning("MonsterScreamerController: No ScreamerSpawnPoint found in scene! Create spawn points using Tools > Screamer Spawn Placer.");
            }
        }
    }

    /// <summary>
    /// Enable or disable screamer mode (called by SpawnRoomZone)
    /// </summary>
    public void SetScreamerMode(bool enable)
    {
        isInScreamerMode = enable;

        if (enable && !hasStartedChase)
        {
            if (debugMode)
            {
                Debug.Log("MonsterScreamerController: Screamer mode ENABLED. Starting screamer loop.");
            }

            // Start screamer behavior
            if (screamerCoroutine != null)
            {
                StopCoroutine(screamerCoroutine);
            }
            screamerCoroutine = StartCoroutine(ScreamerLoop());
        }
        else
        {
            if (debugMode)
            {
                Debug.Log("MonsterScreamerController: Screamer mode DISABLED.");
            }

            // Stop screamer behavior
            if (screamerCoroutine != null)
            {
                StopCoroutine(screamerCoroutine);
                screamerCoroutine = null;
            }
            SetMonsterVisibility(false);
        }
    }

    /// <summary>
    /// Called when player exits spawn zone - transition to chase mode
    /// </summary>
    public void ExitSpawnZone(Vector3 playerPosition)
    {
        if (hasStartedChase)
            return;

        hasStartedChase = true;

        // Stop screamer mode
        SetScreamerMode(false);

        if (debugMode)
        {
            Debug.Log("MonsterScreamerController: Player exited zone. Spawning monster for chase!");
        }

        // Determine spawn position
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (chaseSpawnPoint != null)
        {
            // Use specific spawn point
            spawnPosition = chaseSpawnPoint.position;
            spawnRotation = chaseSpawnPoint.rotation;
            
            if (debugMode)
            {
                Debug.Log($"MonsterScreamerController: Spawning at specific point '{chaseSpawnPoint.name}' at {spawnPosition}");
            }
        }
        else
        {
            // Fallback: spawn near player with offset
            spawnPosition = playerPosition + chaseSpawnOffset;
            spawnRotation = Quaternion.identity;
            
            if (debugMode)
            {
                Debug.LogWarning("MonsterScreamerController: No chaseSpawnPoint assigned, using offset from player.");
            }
        }

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        // Make visible
        SetMonsterVisibility(true);

        // Enable chase AI
        if (monsterAI != null)
        {
            monsterAI.enabled = true;
        }

        // Play sound
        if (screamerSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(screamerSound);
        }
    }

    IEnumerator ScreamerLoop()
    {
        while (isInScreamerMode && !hasStartedChase)
        {
            // Wait for interval
            yield return new WaitForSeconds(screamerInterval);

            // Check if we still have spawn points
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                if (debugMode)
                {
                    Debug.LogWarning("MonsterScreamerController: No spawn points available for screamer.");
                }
                yield break;
            }

            // Pick random spawn point
            ScreamerSpawnPoint randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Teleport to spawn point
            transform.position = randomSpawn.transform.position;
            transform.rotation = randomSpawn.transform.rotation;

            // Show monster
            SetMonsterVisibility(true);

            // Play sound
            if (screamerSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(screamerSound);
            }

            if (debugMode)
            {
                Debug.Log($"MonsterScreamerController: Screamer at '{randomSpawn.name}' for {randomSpawn.displayDuration}s");
            }

            // Wait for display duration (use spawn point's duration if available)
            float displayTime = randomSpawn.displayDuration > 0 ? randomSpawn.displayDuration : screamerDisplayDuration;
            yield return new WaitForSeconds(displayTime);

            // Hide monster
            SetMonsterVisibility(false);
        }
    }

    void SetMonsterVisibility(bool visible)
    {
        if (monsterRenderers != null)
        {
            foreach (var renderer in monsterRenderers)
            {
                if (renderer != null)
                {
                    renderer.enabled = visible;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw chase spawn point preview
        if (chaseSpawnPoint != null)
        {
            // Draw the specific spawn point
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(chaseSpawnPoint.position, 0.5f);
            Gizmos.DrawLine(transform.position, chaseSpawnPoint.position);
            
            // Draw forward direction
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(chaseSpawnPoint.position, chaseSpawnPoint.forward * 2f);
        }
        else if (monsterAI != null && monsterAI.player != null)
        {
            // Draw offset preview (fallback)
            Vector3 chaseSpawnPos = monsterAI.player.position + chaseSpawnOffset;
            Gizmos.color = new Color(1f, 0f, 1f, 0.5f); // semi-transparent magenta
            Gizmos.DrawWireSphere(chaseSpawnPos, 0.5f);
            Gizmos.DrawLine(monsterAI.player.position, chaseSpawnPos);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Test: Trigger Single Screamer")]
    private void TestSingleScreamer()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("MonsterScreamerController: This test only works in Play mode.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            FindSpawnPoints();
        }

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            StartCoroutine(TestScreamerCoroutine());
        }
    }

    IEnumerator TestScreamerCoroutine()
    {
        ScreamerSpawnPoint randomSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        transform.position = randomSpawn.transform.position;
        transform.rotation = randomSpawn.transform.rotation;
        SetMonsterVisibility(true);

        if (screamerSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(screamerSound);
        }

        yield return new WaitForSeconds(screamerDisplayDuration);
        SetMonsterVisibility(false);
    }

    [ContextMenu("Test: Start Chase Mode")]
    private void TestChaseMode()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("MonsterScreamerController: This test only works in Play mode.");
            return;
        }

        if (monsterAI != null && monsterAI.player != null)
        {
            ExitSpawnZone(monsterAI.player.position);
        }
    }
#endif
}
