using UnityEngine;

/// <summary>
/// Script de test pour vérifier que le système de monstre fonctionne correctement
/// Ajoutez ce script à un GameObject vide dans votre scène pour des tests rapides
/// </summary>
public class MonsterSystemTester : MonoBehaviour
{
    [Header("Références")]
    public MonsterSpawner spawner;
    public GameObject monsterPrefab;
    
    [Header("Tests")]
    public bool runTestsOnStart = false;
    public KeyCode spawnKey = KeyCode.M;
    public KeyCode testKey = KeyCode.T;

    void Start()
    {
        if (runTestsOnStart)
        {
            RunAllTests();
        }
    }

    void Update()
    {
        // Spawn un monstre avec la touche M
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnTestMonster();
        }

        // Lancer les tests avec la touche T
        if (Input.GetKeyDown(testKey))
        {
            RunAllTests();
        }
    }

    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        Debug.Log("========================================");
        Debug.Log("TESTS DU SYSTÈME DE MONSTRE");
        Debug.Log("========================================");

        TestPlayerDetection();
        TestMonsterPrefab();
        TestSpawner();
        TestMonsterAI();

        Debug.Log("========================================");
        Debug.Log("FIN DES TESTS");
        Debug.Log("========================================");
    }

    void TestPlayerDetection()
    {
        Debug.Log("\n[TEST] Détection du joueur...");
        
        GameObject playerByTag = GameObject.FindGameObjectWithTag("Player");
        if (playerByTag != null)
        {
            Debug.Log("✓ Joueur trouvé par Tag: " + playerByTag.name);
        }
        else
        {
            Debug.LogWarning("✗ Joueur non trouvé par Tag 'Player'");
        }

        FirstPersonMovement fpm = FindFirstObjectByType<FirstPersonMovement>();
        if (fpm != null)
        {
            Debug.Log("✓ Joueur trouvé par FirstPersonMovement: " + fpm.gameObject.name);
        }
        else
        {
            Debug.LogWarning("✗ Joueur non trouvé par FirstPersonMovement");
        }

        if (playerByTag == null && fpm == null)
        {
            Debug.LogError("✗✗ ERREUR: Aucun joueur détecté! Le système ne fonctionnera pas.");
        }
    }

    void TestMonsterPrefab()
    {
        Debug.Log("\n[TEST] Vérification du prefab de monstre...");
        
        if (monsterPrefab == null)
        {
            Debug.LogWarning("⚠ Aucun prefab de monstre assigné dans le Tester");
            return;
        }

        // Vérifier les composants
        bool hasMonsterAI = monsterPrefab.GetComponent<MonsterAI>() != null;
        bool hasAnimator = monsterPrefab.GetComponent<Animator>() != null || monsterPrefab.GetComponentInChildren<Animator>() != null;
        bool hasCollider = monsterPrefab.GetComponent<Collider>() != null;
        bool hasRigidbody = monsterPrefab.GetComponent<Rigidbody>() != null;

        Debug.Log($"MonsterAI: {(hasMonsterAI ? "✓" : "✗")}");
        Debug.Log($"Animator: {(hasAnimator ? "✓" : "✗")}");
        Debug.Log($"Collider: {(hasCollider ? "✓" : "⚠")} (recommandé)");
        Debug.Log($"Rigidbody: {(hasRigidbody ? "✓" : "⚠")} (recommandé)");

        if (!hasMonsterAI)
        {
            Debug.LogError("✗✗ ERREUR: Le prefab n'a pas de MonsterAI!");
        }
        if (!hasAnimator)
        {
            Debug.LogError("✗✗ ERREUR: Le prefab n'a pas d'Animator!");
        }
    }

    void TestSpawner()
    {
        Debug.Log("\n[TEST] Vérification du Spawner...");
        
        if (spawner == null)
        {
            spawner = FindFirstObjectByType<MonsterSpawner>();
        }

        if (spawner == null)
        {
            Debug.LogWarning("⚠ Aucun MonsterSpawner trouvé dans la scène");
            return;
        }

        Debug.Log("✓ MonsterSpawner trouvé: " + spawner.gameObject.name);

        if (spawner.monsterPrefab == null)
        {
            Debug.LogError("✗✗ ERREUR: Le Spawner n'a pas de prefab assigné!");
        }
        else
        {
            Debug.Log("✓ Prefab assigné au Spawner");
        }

        Debug.Log($"Spawn On Start: {spawner.spawnOnStart}");
        Debug.Log($"Spawn Offset: {spawner.spawnOffset}");
    }

    void TestMonsterAI()
    {
        Debug.Log("\n[TEST] Vérification des MonsterAI actifs...");
        
        MonsterAI[] monsters = FindObjectsByType<MonsterAI>(FindObjectsSortMode.None);
        
        if (monsters.Length == 0)
        {
            Debug.Log("ℹ Aucun monstre actuellement dans la scène");
            return;
        }

        Debug.Log($"Nombre de monstres actifs: {monsters.Length}");

        for (int i = 0; i < monsters.Length; i++)
        {
            MonsterAI monster = monsters[i];
            Debug.Log($"\nMonstre {i + 1}:");
            Debug.Log($"  - Nom: {monster.gameObject.name}");
            Debug.Log($"  - Position: {monster.transform.position}");
            Debug.Log($"  - Joueur assigné: {(monster.player != null ? "✓" : "✗")}");
            Debug.Log($"  - Animator assigné: {(monster.animator != null ? "✓" : "✗")}");
            
            if (monster.player != null)
            {
                float distance = Vector3.Distance(monster.transform.position, monster.player.position);
                Debug.Log($"  - Distance au joueur: {distance:F2}m");
            }
        }
    }

    [ContextMenu("Spawn Test Monster")]
    public void SpawnTestMonster()
    {
        if (spawner != null)
        {
            spawner.SpawnMonster();
            Debug.Log("Monstre spawné via le Spawner!");
        }
        else if (monsterPrefab != null)
        {
            // Trouver le joueur
            Transform playerTransform = null;
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                FirstPersonMovement fpm = FindFirstObjectByType<FirstPersonMovement>();
                if (fpm != null)
                {
                    playerTransform = fpm.transform;
                }
            }

            if (playerTransform != null)
            {
                Vector3 spawnPos = playerTransform.position + playerTransform.right * 5f;
                GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                Debug.Log($"Monstre spawné directement à {spawnPos}!");
            }
            else
            {
                Debug.LogError("Impossible de spawner: joueur non trouvé!");
            }
        }
        else
        {
            Debug.LogError("Impossible de spawner: aucun Spawner ni Prefab assigné!");
        }
    }

    [ContextMenu("Clear All Monsters")]
    public void ClearAllMonsters()
    {
        MonsterAI[] monsters = FindObjectsByType<MonsterAI>(FindObjectsSortMode.None);
        int count = monsters.Length;
        
        foreach (MonsterAI monster in monsters)
        {
            if (Application.isPlaying)
            {
                Destroy(monster.gameObject);
            }
            else
            {
                DestroyImmediate(monster.gameObject);
            }
        }
        
        Debug.Log($"Tous les monstres supprimés! ({count} monstre(s))");
    }

    void OnGUI()
    {
        // Afficher les touches de contrôle à l'écran
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 16;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;

        string info = $"[{spawnKey}] Spawn Monster\n[{testKey}] Run Tests";
        GUI.Label(new Rect(10, 10, 300, 100), info, style);
    }
}
