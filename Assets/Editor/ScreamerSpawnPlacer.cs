using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool to quickly place ScreamerSpawnPoint objects by clicking in the Scene view.
/// Open via Tools > Screamer Spawn Placer.
/// Usage: Toggle "Place Mode", then Shift+Left-Click in the Scene view to place spawn points.
/// </summary>
public class ScreamerSpawnPlacer : EditorWindow
{
    private bool placeModeEnabled = false;
    private GameObject spawnPointPrefab;
    private GameObject spawnPointsParent;
    private Color gizmoColor = Color.red;
    private float displayDuration = 0.5f;
    private bool faceSceneCamera = true;
    private bool faceHitNormal = false;
    private string spawnPointNamePrefix = "ScreamerSpawn_";
    private int spawnPointCounter = 1;

    [MenuItem("Tools/Screamer Spawn Placer")]
    public static void ShowWindow()
    {
        GetWindow<ScreamerSpawnPlacer>("Screamer Spawn Placer");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        // Try to find or create the parent group
        FindOrCreateParent();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("Screamer Spawn Point Placer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Toggle 'Place Mode' below, then:\n" +
            "• Shift+Left-Click in the Scene view to place a spawn point on a surface.\n" +
            "• The spawn point will be added under the parent group automatically.",
            MessageType.Info
        );

        EditorGUILayout.Space();

        placeModeEnabled = EditorGUILayout.Toggle("Place Mode Enabled", placeModeEnabled);

        EditorGUILayout.Space();
        GUILayout.Label("Spawn Point Settings", EditorStyles.boldLabel);

        spawnPointsParent = (GameObject)EditorGUILayout.ObjectField(
            "Parent Group",
            spawnPointsParent,
            typeof(GameObject),
            true
        );

        if (GUILayout.Button("Create/Find Parent Group"))
        {
            FindOrCreateParent();
        }

        EditorGUILayout.Space();

        spawnPointNamePrefix = EditorGUILayout.TextField("Name Prefix", spawnPointNamePrefix);
        displayDuration = EditorGUILayout.FloatField("Display Duration", displayDuration);
        gizmoColor = EditorGUILayout.ColorField("Gizmo Color", gizmoColor);

        EditorGUILayout.Space();
        GUILayout.Label("Orientation", EditorStyles.boldLabel);
        faceSceneCamera = EditorGUILayout.Toggle("Face Scene Camera", faceSceneCamera);
        faceHitNormal = EditorGUILayout.Toggle("Face Hit Surface Normal", faceHitNormal);

        EditorGUILayout.Space();

        if (GUILayout.Button("Clear All Spawn Points"))
        {
            if (EditorUtility.DisplayDialog(
                "Clear All Spawn Points",
                "Are you sure you want to delete all ScreamerSpawnPoint objects?",
                "Yes",
                "Cancel"))
            {
                ClearAllSpawnPoints();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            $"Total spawn points: {CountSpawnPoints()}",
            MessageType.None
        );
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!placeModeEnabled) return;

        Event e = Event.current;

        // Shift + Left Click to place
        if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PlaceSpawnPoint(hit.point, hit.normal, sceneView);
                e.Use(); // Consume the event
            }
            else
            {
                // Place at a default distance from camera if no hit
                Vector3 position = ray.origin + ray.direction * 5f;
                PlaceSpawnPoint(position, -ray.direction, sceneView);
                e.Use();
            }
        }

        // Draw helper text in Scene view
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 10, 300, 100));
        GUILayout.Label("PLACE MODE ACTIVE", EditorStyles.whiteLargeLabel);
        GUILayout.Label("Shift+Left-Click to place spawn point", EditorStyles.whiteLabel);
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private void PlaceSpawnPoint(Vector3 position, Vector3 normal, SceneView sceneView)
    {
        FindOrCreateParent();

        // Create new empty GameObject
        GameObject spawnPoint = new GameObject($"{spawnPointNamePrefix}{spawnPointCounter++}");
        spawnPoint.transform.position = position;

        // Set rotation
        if (faceSceneCamera && sceneView != null)
        {
            spawnPoint.transform.rotation = sceneView.camera.transform.rotation;
        }
        else if (faceHitNormal)
        {
            spawnPoint.transform.rotation = Quaternion.LookRotation(-normal, Vector3.up);
        }

        // Add the ScreamerSpawnPoint component
        var component = spawnPoint.AddComponent<ScreamerSpawnPoint>();
        component.displayDuration = displayDuration;
        component.gizmoColor = gizmoColor;

        // Parent it
        if (spawnPointsParent != null)
        {
            spawnPoint.transform.SetParent(spawnPointsParent.transform);
        }

        // Register undo
        Undo.RegisterCreatedObjectUndo(spawnPoint, "Place Screamer Spawn Point");

        // Select the new spawn point
        Selection.activeGameObject = spawnPoint;

        Debug.Log($"Placed {spawnPoint.name} at {position}");
    }

    private void FindOrCreateParent()
    {
        if (spawnPointsParent == null)
        {
            // Try to find existing
            spawnPointsParent = GameObject.Find("ScreamerSpawnPoints");

            if (spawnPointsParent == null)
            {
                // Create new
                spawnPointsParent = new GameObject("ScreamerSpawnPoints");
                Undo.RegisterCreatedObjectUndo(spawnPointsParent, "Create Screamer Spawn Points Parent");
                Debug.Log("Created ScreamerSpawnPoints parent group.");
            }
        }
    }

    private int CountSpawnPoints()
    {
        return FindObjectsOfType<ScreamerSpawnPoint>().Length;
    }

    private void ClearAllSpawnPoints()
    {
        var spawnPoints = FindObjectsOfType<ScreamerSpawnPoint>();
        foreach (var sp in spawnPoints)
        {
            Undo.DestroyObjectImmediate(sp.gameObject);
        }
        spawnPointCounter = 1;
        Debug.Log($"Cleared {spawnPoints.Length} spawn points.");
    }
}
