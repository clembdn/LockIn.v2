using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Script Editor pour ajouter automatiquement le monstre dans la scène SampleScene
/// </summary>
public class AddMonsterToScene : EditorWindow
{
    private GameObject monsterPrefab;
    private Vector3 spawnOffset = new Vector3(5f, 0f, 0f);
    
    [MenuItem("LockIn/Add Monster to Scene")]
    public static void ShowWindow()
    {
        GetWindow<AddMonsterToScene>("Add Monster to Scene");
    }

    void OnGUI()
    {
        GUILayout.Label("Ajouter le monstre à la scène", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        monsterPrefab = (GameObject)EditorGUILayout.ObjectField(
            "Monster Prefab", 
            monsterPrefab, 
            typeof(GameObject), 
            false
        );

        EditorGUILayout.Space();
        spawnOffset = EditorGUILayout.Vector3Field("Spawn Offset (from player)", spawnOffset);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Ce script va:\n" +
            "1. Chercher un prefab de monstre si non assigné\n" +
            "2. Créer un MonsterSpawner dans la scène\n" +
            "3. Configurer le spawner avec le prefab\n" +
            "4. Sauvegarder la scène",
            MessageType.Info
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Auto-Find Monster Prefab", GUILayout.Height(30)))
        {
            FindMonsterPrefab();
        }

        if (GUILayout.Button("Add Monster System to Current Scene", GUILayout.Height(40)))
        {
            AddMonsterSystem();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Quick Setup: Find Prefab + Add to Scene", GUILayout.Height(50)))
        {
            FindMonsterPrefab();
            if (monsterPrefab != null)
            {
                AddMonsterSystem();
            }
        }
    }

    void FindMonsterPrefab()
    {
        // Chercher un prefab configuré avec MonsterAI
        string[] guids = AssetDatabase.FindAssets("t:Prefab MonsterMutant7");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                // Vérifier si le prefab a MonsterAI
                if (prefab.GetComponent<MonsterAI>() != null)
                {
                    monsterPrefab = prefab;
                    Debug.Log($"✓ Prefab de monstre configuré trouvé: {path}");
                    return;
                }
            }
        }

        // Si pas trouvé avec MonsterAI, chercher les prefabs originaux
        guids = AssetDatabase.FindAssets("Base mesh MonsterMutant7 skin1");
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            monsterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Debug.LogWarning($"⚠ Prefab original trouvé mais non configuré: {path}");
            Debug.LogWarning("Utilisez QuickMonsterSetup pour le configurer d'abord!");
        }
        else
        {
            Debug.LogError("✗ Aucun prefab de monstre trouvé!");
        }
    }

    void AddMonsterSystem()
    {
        if (monsterPrefab == null)
        {
            EditorUtility.DisplayDialog(
                "Erreur", 
                "Veuillez assigner un prefab de monstre d'abord!", 
                "OK"
            );
            return;
        }

        // Vérifier qu'une scène est ouverte
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid())
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Aucune scène n'est ouverte!",
                "OK"
            );
            return;
        }

        Debug.Log("=== Ajout du système de monstre à la scène ===");

        // Vérifier si un MonsterSpawner existe déjà
        MonsterSpawner existingSpawner = FindFirstObjectByType<MonsterSpawner>();
        if (existingSpawner != null)
        {
            bool replace = EditorUtility.DisplayDialog(
                "MonsterSpawner existant",
                "Un MonsterSpawner existe déjà dans la scène. Voulez-vous le remplacer?",
                "Remplacer",
                "Annuler"
            );

            if (!replace)
            {
                Debug.Log("Opération annulée.");
                return;
            }

            DestroyImmediate(existingSpawner.gameObject);
            Debug.Log("✓ Ancien MonsterSpawner supprimé");
        }

        // Créer le MonsterSpawner
        GameObject spawnerObject = new GameObject("MonsterSpawner");
        MonsterSpawner spawner = spawnerObject.AddComponent<MonsterSpawner>();
        
        // Configurer le spawner
        spawner.monsterPrefab = monsterPrefab;
        spawner.spawnOffset = spawnOffset;
        spawner.spawnOnStart = true;

        // Trouver le joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            spawner.player = player.transform;
            Debug.Log("✓ Joueur trouvé et assigné au spawner");
        }
        else
        {
            Debug.LogWarning("⚠ Joueur non trouvé avec le tag 'Player'. Le spawner le trouvera au runtime.");
        }

        // Marquer la scène comme modifiée
        EditorSceneManager.MarkSceneDirty(activeScene);
        
        Debug.Log("✓ MonsterSpawner créé et configuré!");
        Debug.Log($"  - Prefab: {monsterPrefab.name}");
        Debug.Log($"  - Spawn Offset: {spawnOffset}");
        Debug.Log($"  - Spawn On Start: {spawner.spawnOnStart}");

        // Sélectionner le spawner dans la hiérarchie
        Selection.activeGameObject = spawnerObject;

        // Demander si on veut sauvegarder
        bool save = EditorUtility.DisplayDialog(
            "Succès!",
            "MonsterSpawner ajouté à la scène!\n\nVoulez-vous sauvegarder la scène maintenant?",
            "Sauvegarder",
            "Plus tard"
        );

        if (save)
        {
            EditorSceneManager.SaveScene(activeScene);
            Debug.Log("✓ Scène sauvegardée!");
        }

        Debug.Log("=== Configuration terminée! ===");
        Debug.Log("Appuyez sur Play pour tester!");
    }

    [MenuItem("LockIn/Quick Add Monster to SampleScene")]
    public static void QuickAddMonster()
    {
        // Ouvrir la scène SampleScene si elle n'est pas déjà ouverte
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != "SampleScene")
        {
            string scenePath = "Assets/Scenes/SampleScene.unity";
            if (System.IO.File.Exists(scenePath))
            {
                EditorSceneManager.OpenScene(scenePath);
                Debug.Log("✓ Scène SampleScene ouverte");
            }
            else
            {
                Debug.LogError("✗ Scène SampleScene non trouvée!");
                return;
            }
        }

        // Créer une fenêtre temporaire pour exécuter la logique
        AddMonsterToScene window = CreateInstance<AddMonsterToScene>();
        window.FindMonsterPrefab();
        
        if (window.monsterPrefab != null)
        {
            window.AddMonsterSystem();
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Impossible de trouver un prefab de monstre!\n\n" +
                "Veuillez:\n" +
                "1. Glisser un prefab MonsterMutant7 dans la scène\n" +
                "2. Ajouter QuickMonsterSetup\n" +
                "3. Cliquer sur 'Setup Monster Components'\n" +
                "4. Créer le prefab\n" +
                "5. Réessayer",
                "OK"
            );
        }
    }
}
