using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

/// <summary>
/// Script simple pour ajouter et configurer le monstre directement dans SampleScene
/// Exécute: Menu LockIn > Add Monster to SampleScene NOW!
/// </summary>
public class QuickAddMonsterToScene : EditorWindow
{
    [MenuItem("LockIn/Add Monster to SampleScene NOW!")]
    public static void AddMonsterNow()
    {
        Debug.Log("=== Ajout du monstre à SampleScene ===");

        // 1. Ouvrir SampleScene si ce n'est pas déjà fait
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != "SampleScene")
        {
            string scenePath = "Assets/Scenes/SampleScene.unity";
            if (System.IO.File.Exists(scenePath))
            {
                EditorSceneManager.OpenScene(scenePath);
                activeScene = SceneManager.GetActiveScene();
                Debug.Log("✓ SampleScene ouverte");
            }
            else
            {
                EditorUtility.DisplayDialog("Erreur", "SampleScene non trouvée!", "OK");
                return;
            }
        }

        // 2. Trouver le joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            // Chercher par FirstPersonMovement
            FirstPersonMovement fpm = Object.FindFirstObjectByType<FirstPersonMovement>();
            if (fpm != null)
            {
                player = fpm.gameObject;
                // Ajouter le tag Player
                player.tag = "Player";
                Debug.Log("✓ Tag 'Player' ajouté au joueur");
            }
            else
            {
                // Chercher un objet nommé Player
                player = GameObject.Find("Player");
                if (player != null)
                {
                    player.tag = "Player";
                    Debug.Log("✓ Tag 'Player' ajouté au joueur");
                }
            }
        }

        if (player == null)
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Impossible de trouver le joueur dans la scène!\n\nAssurez-vous qu'un objet 'Player' existe.",
                "OK"
            );
            return;
        }

        Debug.Log($"✓ Joueur trouvé: {player.name}");

        // 3. Chercher le prefab du monstre
        string[] guids = AssetDatabase.FindAssets("Base mesh MonsterMutant7 skin1 t:Prefab");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Prefab du monstre non trouvé!\n\nAssurez-vous que le dossier 'Assets/MonsterMutant 7/Prefab/' existe.",
                "OK"
            );
            return;
        }

        string prefabPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        GameObject monsterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        Debug.Log($"✓ Prefab trouvé: {prefabPath}");

        // 4. Supprimer les anciens monstres s'il y en a
        MonsterAI[] existingMonsters = Object.FindObjectsByType<MonsterAI>(FindObjectsSortMode.None);
        foreach (var existingMonster in existingMonsters)
        {
            DestroyImmediate(existingMonster.gameObject);
        }

        // 5. Instancier le monstre à côté du joueur
        Vector3 spawnPosition = player.transform.position + player.transform.right * 5f;
        GameObject monster = (GameObject)PrefabUtility.InstantiatePrefab(monsterPrefab);
        monster.transform.position = spawnPosition;
        monster.transform.rotation = Quaternion.identity;
        monster.name = "MonsterMutant7";

        Debug.Log($"✓ Monstre instancié à {spawnPosition}");

        // 6. Configurer le monstre
        ConfigureMonster(monster, player);

        // 7. Sauvegarder la scène
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        // 8. Sélectionner le monstre
        Selection.activeGameObject = monster;

        Debug.Log("=== Configuration terminée! ===");
        Debug.Log("Appuyez sur Play pour tester!");

        EditorUtility.DisplayDialog(
            "Succès!",
            "Le monstre a été ajouté à SampleScene!\n\n" +
            "Configuration:\n" +
            "✓ Monstre placé à 5m à droite du joueur\n" +
            "✓ MonsterAI configuré\n" +
            "✓ Animator Controller assigné\n" +
            "✓ Collider et Rigidbody ajoutés\n\n" +
            "Appuyez sur Play pour tester! ▶️",
            "Super!"
        );
    }

    static void ConfigureMonster(GameObject monster, GameObject player)
    {
        Debug.Log("Configuration du monstre...");

        // 1. Ajouter MonsterAI
        MonsterAI monsterAI = monster.GetComponent<MonsterAI>();
        if (monsterAI == null)
        {
            monsterAI = monster.AddComponent<MonsterAI>();
            Debug.Log("✓ MonsterAI ajouté");
        }

        // Configurer MonsterAI
        monsterAI.player = player.transform;
        monsterAI.walkSpeed = 2f;
        monsterAI.runSpeed = 3.5f;
        monsterAI.runDistance = 10f;
        monsterAI.stoppingDistance = 2f;

        // 2. Configurer l'Animator
        Animator animator = monster.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            animator = monster.GetComponent<Animator>();
        }

        if (animator != null)
        {
            // Chercher l'Animator Controller
            string[] controllerGuids = AssetDatabase.FindAssets("MonsterMutant7 Animator Controller t:AnimatorController");
            if (controllerGuids.Length > 0)
            {
                string controllerPath = AssetDatabase.GUIDToAssetPath(controllerGuids[0]);
                RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
                animator.runtimeAnimatorController = controller;
                monsterAI.animator = animator;
                Debug.Log("✓ Animator Controller assigné");
            }
        }
        else
        {
            Debug.LogWarning("⚠ Animator non trouvé sur le monstre");
        }

        // 3. Ajouter un Capsule Collider
        CapsuleCollider collider = monster.GetComponent<CapsuleCollider>();
        if (collider == null)
        {
            collider = monster.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.5f;
            collider.center = new Vector3(0, 1f, 0);
            Debug.Log("✓ Capsule Collider ajouté");
        }

        // 4. Ajouter un Rigidbody
        Rigidbody rb = monster.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = monster.AddComponent<Rigidbody>();
            rb.mass = 80f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            Debug.Log("✓ Rigidbody ajouté");
        }

        // 5. Optionnel: Ajouter NavMeshAgent (désactivé par défaut car pas de NavMesh)
        NavMeshAgent navAgent = monster.GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = monster.AddComponent<NavMeshAgent>();
            navAgent.speed = 3.5f;
            navAgent.acceleration = 8f;
            navAgent.angularSpeed = 120f;
            navAgent.stoppingDistance = 2f;
            // Désactiver le NavMeshAgent car il n'y a pas de NavMesh dans la scène
            navAgent.enabled = false;
            Debug.Log("✓ NavMeshAgent ajouté (désactivé - pas de NavMesh)");
        }
        else
        {
            // Désactiver s'il existe déjà
            navAgent.enabled = false;
            Debug.Log("✓ NavMeshAgent désactivé (pas de NavMesh)");
        }

        // 6. Ajouter le tag Enemy
        if (monster.tag == "Untagged")
        {
            monster.tag = "Enemy";
            Debug.Log("✓ Tag 'Enemy' ajouté");
        }

        Debug.Log("✓ Configuration du monstre terminée!");
    }
}
