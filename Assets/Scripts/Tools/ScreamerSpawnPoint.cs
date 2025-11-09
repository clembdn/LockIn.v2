using UnityEngine;

/// <summary>
/// Mark a spawn location for a screamer (monster appearing briefly at a window).
/// Attach this to an Empty GameObject; it draws a visual gizmo in the Scene view
/// so you can see spawn points and their orientation clearly.
/// </summary>
public class ScreamerSpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Duration the monster will appear at this location (seconds).")]
    public float displayDuration = 0.5f;

    [Tooltip("Optional: reference to a specific window or trigger area.")]
    public GameObject associatedWindow;

    [Header("Gizmo Visualization")]
    [Tooltip("Color of the spawn point gizmo in the Scene view.")]
    public Color gizmoColor = Color.red;

    [Tooltip("Size of the gizmo sphere/icon.")]
    public float gizmoSize = 0.3f;

    private void OnDrawGizmos()
    {
        // Draw a sphere at the spawn point
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoSize);

        // Draw a wireframe sphere to see through geometry
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        Gizmos.DrawWireSphere(transform.position, gizmoSize * 1.5f);

        // Draw forward direction (where the monster will face)
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);

        // Draw a small cross to indicate the exact position
        Gizmos.color = Color.white;
        float crossSize = gizmoSize * 0.5f;
        Gizmos.DrawLine(transform.position - transform.right * crossSize, transform.position + transform.right * crossSize);
        Gizmos.DrawLine(transform.position - transform.up * crossSize, transform.position + transform.up * crossSize);
    }

    private void OnDrawGizmosSelected()
    {
        // When selected, draw a more prominent visualization
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, gizmoSize * 2f);

        // Draw a line to the associated window if assigned
        if (associatedWindow != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, associatedWindow.transform.position);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Face Scene View Camera")]
    private void FaceSceneCamera()
    {
        // Align this spawn point to face the scene view camera (useful for quick setup)
        var sceneView = UnityEditor.SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            transform.rotation = sceneView.camera.transform.rotation;
            Debug.Log($"ScreamerSpawnPoint '{name}' now faces the Scene view camera.");
        }
    }

    [ContextMenu("Align With Scene View")]
    private void AlignWithSceneView()
    {
        // Move this spawn point to the scene view camera position and rotation
        var sceneView = UnityEditor.SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            transform.position = sceneView.camera.transform.position;
            transform.rotation = sceneView.camera.transform.rotation;
            Debug.Log($"ScreamerSpawnPoint '{name}' aligned with Scene view.");
        }
    }
#endif
}
