using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Script pour ajouter rapidement un monstre directement dans la scène active
/// Plus simple et direct que le spawner
/// </summary>
public class DirectMonsterAdder
{
    [MenuItem("GameObject/LockIn/Add Monster Here", false, 10)]
    public static void AddMonsterToScene()
    {
        // Trouver le prefab du monstre
        string[] guids = AssetDatabase.FindAssets("Base mesh MonsterMutant7 skin1 t:Prefab");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Prefab MonsterMutant7 non trouvé!\n\nAssurez-vous que le dossier 'MonsterMutant 7' est dans le projet.",
                "OK"
            );
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        GameObject monsterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (monsterPrefab == null)
        {
            Debug.LogError("Impossible de charger le prefab du monstre!");
            return;
        }

        // Trouver le joueur pour positionner le monstre à côté
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 spawnPosition = Vector3.zero;
        
        if (player != null)
        {
            spawnPosition = player.transform.position + player.transform.right * 5f;
            Debug.Log("✓ Joueur trouvé, monstre placé à 5m à droite");
        }
        else
        {
            // Chercher par FirstPersonMovement
            FirstPersonMovement fpm = Object.FindFirstObjectByType<FirstPersonMovement>();
            if (fpm != null)
            {
                player = fpm.gameObject;
                spawnPosition = player.transform.position + player.transform.right * 5f;
                Debug.Log("✓ Joueur trouvé via FirstPersonMovement, monstre placé à 5m à droite");
            }
            else
            {
                Debug.LogWarning("⚠ Joueur non trouvé, monstre placé à l'origine");
            }
        }

        // Instancier le monstre dans la scène
        GameObject monster = (GameObject)PrefabUtility.InstantiatePrefab(monsterPrefab);
        monster.transform.position = spawnPosition;
        monster.name = "MonsterMutant7";

        // Configurer automatiquement le monstre
        ConfigureMonster(monster, player);

        // Sélectionner le monstre
        Selection.activeGameObject = monster;

        // Marquer la scène comme modifiée
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log("✓ Monstre ajouté à la scène!");
        Debug.Log("Pour terminer la configuration:");
        Debug.Log("1. Sélectionnez le monstre");
        Debug.Log("2. Ajoutez le composant 'QuickMonsterSetup' (ou configurez manuellement)");
        Debug.Log("3. Cliquez sur 'SETUP MONSTER COMPONENTS'");

        // Afficher un message
        EditorUtility.DisplayDialog(
            "Monstre ajouté!",
            "Le monstre a été ajouté à la scène.\n\n" +
            "Pour terminer:\n" +
            "1. Avec le monstre sélectionné, ajoutez 'QuickMonsterSetup'\n" +
            "2. Cliquez 'SETUP MONSTER COMPONENTS'\n" +
            "3. Assignez l'Animator Controller\n" +
            "4. Testez avec Play!",
            "OK"
        );
    }

    [MenuItem("LockIn/Add Configured Monster to Scene")]
    public static void AddConfiguredMonster()
    {
        // Chercher un prefab déjà configuré (dans Assets/Prefabs/)
        string[] guids = AssetDatabase.FindAssets("ConfiguredMonsterMutant7 t:Prefab");
        GameObject configuredPrefab = null;

        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            configuredPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        // Si pas de prefab configuré trouvé, chercher n'importe quel prefab avec MonsterAI
        if (configuredPrefab == null)
        {
            guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null && prefab.GetComponent<MonsterAI>() != null)
                {
                    configuredPrefab = prefab;
                    break;
                }
            }
        }

        if (configuredPrefab == null)
        {
            bool createNew = EditorUtility.DisplayDialog(
                "Aucun monstre configuré",
                "Aucun prefab de monstre configuré n'a été trouvé.\n\n" +
                "Voulez-vous ajouter un monstre non configuré à la scène?",
                "Oui",
                "Annuler"
            );

            if (createNew)
            {
                AddMonsterToScene();
            }
            return;
        }

        // Trouver le joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 spawnPosition = Vector3.zero;
        
        if (player == null)
        {
            FirstPersonMovement fpm = Object.FindFirstObjectByType<FirstPersonMovement>();
            if (fpm != null)
            {
                player = fpm.gameObject;
            }
        }

        if (player != null)
        {
            spawnPosition = player.transform.position + player.transform.right * 5f;
        }

        // Instancier le monstre configuré
        GameObject monster = (GameObject)PrefabUtility.InstantiatePrefab(configuredPrefab);
        monster.transform.position = spawnPosition;

        // S'assurer que le joueur est assigné
        MonsterAI monsterAI = monster.GetComponent<MonsterAI>();
        if (monsterAI != null && player != null)
        {
            monsterAI.player = player.transform;
        }

        // Sélectionner le monstre
        Selection.activeGameObject = monster;

        // Marquer la scène comme modifiée
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log($"✓ Monstre configuré ajouté: {configuredPrefab.name}");
        Debug.Log("✓ Prêt à tester! Appuyez sur Play!");

        EditorUtility.DisplayDialog(
            "Succès!",
            "Monstre configuré ajouté à la scène!\n\n" +
            "Le monstre est prêt à être testé.\n" +
            "Appuyez sur Play ▶️",
            "OK"
        );
    }

    private static void ConfigureMonster(GameObject monster, GameObject player)
    {
        // Vérifier si QuickMonsterSetup existe et l'ajouter
        if (monster.GetComponent<QuickMonsterSetup>() == null)
        {
            QuickMonsterSetup setup = monster.AddComponent<QuickMonsterSetup>();
            Debug.Log("✓ QuickMonsterSetup ajouté automatiquement");
        }
    }

    [MenuItem("LockIn/Complete Setup: Monster + Spawner")]
    public static void CompleteSetup()
    {
        bool proceed = EditorUtility.DisplayDialog(
            "Configuration complète",
            "Cette option va:\n\n" +
            "1. Chercher ou créer un prefab de monstre configuré\n" +
            "2. Ajouter un MonsterSpawner à la scène\n" +
            "3. Configurer tout automatiquement\n\n" +
            "Continuer?",
            "Oui",
            "Annuler"
        );

        if (!proceed) return;

        // Étape 1: Chercher un prefab configuré
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        GameObject configuredPrefab = null;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null && prefab.GetComponent<MonsterAI>() != null)
            {
                configuredPrefab = prefab;
                Debug.Log($"✓ Prefab configuré trouvé: {path}");
                break;
            }
        }

        if (configuredPrefab == null)
        {
            EditorUtility.DisplayDialog(
                "Prefab manquant",
                "Aucun prefab de monstre configuré trouvé!\n\n" +
                "Veuillez d'abord:\n" +
                "1. Ajouter un monstre à la scène (GameObject > LockIn > Add Monster Here)\n" +
                "2. Le configurer avec QuickMonsterSetup\n" +
                "3. Créer le prefab\n" +
                "4. Réessayer cette option",
                "OK"
            );
            return;
        }

        // Étape 2: Créer le MonsterSpawner
        MonsterSpawner existingSpawner = Object.FindFirstObjectByType<MonsterSpawner>();
        if (existingSpawner != null)
        {
            bool replace = EditorUtility.DisplayDialog(
                "Spawner existant",
                "Un MonsterSpawner existe déjà. Remplacer?",
                "Oui",
                "Non"
            );

            if (replace)
            {
                Object.DestroyImmediate(existingSpawner.gameObject);
            }
            else
            {
                return;
            }
        }

        GameObject spawnerObject = new GameObject("MonsterSpawner");
        MonsterSpawner spawner = spawnerObject.AddComponent<MonsterSpawner>();
        
        spawner.monsterPrefab = configuredPrefab;
        spawner.spawnOffset = new Vector3(5f, 0f, 0f);
        spawner.spawnOnStart = true;

        // Trouver et assigner le joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            FirstPersonMovement fpm = Object.FindFirstObjectByType<FirstPersonMovement>();
            if (fpm != null)
            {
                player = fpm.gameObject;
            }
        }

        if (player != null)
        {
            spawner.player = player.transform;
            Debug.Log("✓ Joueur assigné au spawner");
        }

        // Sélectionner le spawner
        Selection.activeGameObject = spawnerObject;

        // Marquer la scène comme modifiée
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log("✓✓✓ Configuration complète terminée!");
        Debug.Log($"   - Prefab: {configuredPrefab.name}");
        Debug.Log("   - MonsterSpawner créé et configuré");
        Debug.Log("   - Prêt à tester!");

        EditorUtility.DisplayDialog(
            "Configuration terminée!",
            "Tout est prêt!\n\n" +
            "✓ Prefab de monstre trouvé\n" +
            "✓ MonsterSpawner créé\n" +
            "✓ Joueur assigné\n\n" +
            "Appuyez sur Play pour tester! ▶️",
            "Super!"
        );
    }
}
