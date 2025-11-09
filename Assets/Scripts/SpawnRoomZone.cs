using UnityEngine;

/// <summary>
/// Defines the spawn room zone where the monster performs screamers.
/// When the player exits this zone, the monster spawns at the player's location and starts chasing.
/// Attach this to a GameObject with a Collider set to "Is Trigger".
/// </summary>
[RequireComponent(typeof(Collider))]
public class SpawnRoomZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [Tooltip("The monster that will be controlled by this zone.")]
    public MonsterScreamerController monsterController;

    [Tooltip("Auto-find the player by tag or component.")]
    public bool autoFindPlayer = true;

    [Tooltip("Visualize the zone bounds in the Scene view.")]
    public Color gizmoColor = new Color(1f, 0.5f, 0f, 0.3f);

    private Transform player;
    private bool playerInZone = false;
    private bool hasExited = false; // Track if player has exited (to avoid re-triggering)

    void Start()
    {
        // Ensure collider is set to trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"SpawnRoomZone: Collider on '{name}' was not set to Trigger. Fixed automatically.");
        }

        // Auto-find player
        if (autoFindPlayer)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                FirstPersonMovement fpm = FindFirstObjectByType<FirstPersonMovement>();
                if (fpm != null)
                {
                    player = fpm.transform;
                }
            }
        }

        // Auto-find monster controller if not assigned
        if (monsterController == null)
        {
            monsterController = FindFirstObjectByType<MonsterScreamerController>();
            if (monsterController != null)
            {
                Debug.Log($"SpawnRoomZone: Auto-found MonsterScreamerController '{monsterController.name}'");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if it's the player entering
        if (IsPlayer(other))
        {
            playerInZone = true;
            Debug.Log("SpawnRoomZone: Player entered spawn zone. Monster will perform screamers.");

            if (monsterController != null)
            {
                monsterController.SetScreamerMode(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if it's the player exiting
        if (IsPlayer(other) && !hasExited)
        {
            playerInZone = false;
            hasExited = true;
            Debug.Log("SpawnRoomZone: Player exited spawn zone. Monster will now chase the player!");

            if (monsterController != null)
            {
                // Get player position when they exit
                Transform playerTransform = other.transform;
                monsterController.ExitSpawnZone(playerTransform.position);
            }
        }
    }

    private bool IsPlayer(Collider other)
    {
        // Check by tag
        if (other.CompareTag("Player"))
            return true;

        // Check by component
        if (other.GetComponent<FirstPersonMovement>() != null)
            return true;

        // Check if it's our tracked player
        if (player != null && other.transform == player)
            return true;

        return false;
    }

    void OnDrawGizmos()
    {
        // Draw the zone bounds
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;

            if (col is BoxCollider boxCol)
            {
                Gizmos.DrawCube(boxCol.center, boxCol.size);
                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
                Gizmos.DrawWireCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawSphere(sphereCol.center, sphereCol.radius);
                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
                Gizmos.DrawWireSphere(sphereCol.center, sphereCol.radius);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw more visible bounds when selected
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;

            if (col is BoxCollider boxCol)
            {
                Gizmos.DrawWireCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawWireSphere(sphereCol.center, sphereCol.radius);
            }
        }
    }
}
