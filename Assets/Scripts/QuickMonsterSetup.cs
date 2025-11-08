using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Script d'aide pour configurer rapidement le monstre dans la scène
/// Ajoute automatiquement tous les composants nécessaires
/// </summary>
public class QuickMonsterSetup : MonoBehaviour
{
    [Header("Configuration automatique")]
    [Tooltip("Configurer automatiquement au démarrage de l'éditeur")]
    public bool autoSetupInEditor = true;

    [Header("Composants")]
    public Animator animator;
    public MonsterAI monsterAI;
    public CapsuleCollider capsuleCollider;
    public Rigidbody rb;

    #if UNITY_EDITOR
    void Reset()
    {
        SetupMonster();
    }

    [ContextMenu("Setup Monster Components")]
    public void SetupMonster()
    {
        Debug.Log("=== Configuration du MonsterMutant7 ===");

        // 1. Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        if (animator != null)
        {
            Debug.Log("✓ Animator trouvé");
        }
        else
        {
            Debug.LogWarning("⚠ Animator non trouvé - ajoutez-le manuellement");
        }

        // 2. MonsterAI
        monsterAI = GetComponent<MonsterAI>();
        if (monsterAI == null)
        {
            monsterAI = gameObject.AddComponent<MonsterAI>();
            Debug.Log("✓ MonsterAI ajouté");
        }
        else
        {
            Debug.Log("✓ MonsterAI déjà présent");
        }

        // Configurer MonsterAI
        if (monsterAI != null && animator != null)
        {
            monsterAI.animator = animator;
        }

        // 3. Collider
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.height = 2f;
            capsuleCollider.radius = 0.5f;
            capsuleCollider.center = new Vector3(0, 1f, 0);
            Debug.Log("✓ CapsuleCollider ajouté et configuré");
        }
        else
        {
            Debug.Log("✓ CapsuleCollider déjà présent");
        }

        // 4. Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 80f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Empêche le monstre de basculer
            Debug.Log("✓ Rigidbody ajouté et configuré");
        }
        else
        {
            Debug.Log("✓ Rigidbody déjà présent");
        }

        // 5. Tag
        if (gameObject.tag == "Untagged")
        {
            gameObject.tag = "Enemy";
            Debug.Log("✓ Tag 'Enemy' assigné");
        }

        Debug.Log("=== Configuration terminée! ===");
        Debug.Log("N'oubliez pas de:");
        Debug.Log("1. Vérifier que l'Animator Controller est assigné");
        Debug.Log("2. Configurer les paramètres dans l'Animator (Speed, IsRunning)");
        Debug.Log("3. Créer un prefab à partir de cet objet configuré");
        
        #if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
        #endif
    }

    [ContextMenu("Create Prefab from this Monster")]
    public void CreatePrefab()
    {
        #if UNITY_EDITOR
        string path = "Assets/Prefabs/ConfiguredMonsterMutant7.prefab";
        
        // Créer le dossier Prefabs s'il n'existe pas
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Créer le prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, path);
        
        if (prefab != null)
        {
            Debug.Log($"✓ Prefab créé avec succès: {path}");
            EditorGUIUtility.PingObject(prefab);
        }
        else
        {
            Debug.LogError("✗ Échec de la création du prefab");
        }
        #endif
    }

    [ContextMenu("Validate Configuration")]
    public void ValidateConfiguration()
    {
        Debug.Log("=== Validation de la configuration ===");
        
        bool isValid = true;

        // Check Animator
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("✗ Animator ou Animator Controller manquant");
            isValid = false;
        }
        else
        {
            Debug.Log("✓ Animator configuré");
            
            // Check parameters
            bool hasSpeedParam = false;
            foreach (var param in animator.parameters)
            {
                if (param.name == "Speed" && param.type == AnimatorControllerParameterType.Float)
                {
                    hasSpeedParam = true;
                    break;
                }
            }
            
            if (!hasSpeedParam)
            {
                Debug.LogWarning("⚠ Paramètre 'Speed' (Float) non trouvé dans l'Animator");
            }
            else
            {
                Debug.Log("✓ Paramètre 'Speed' présent");
            }
        }

        // Check MonsterAI
        if (monsterAI == null)
        {
            Debug.LogError("✗ MonsterAI manquant");
            isValid = false;
        }
        else
        {
            Debug.Log("✓ MonsterAI présent");
        }

        // Check Collider
        if (capsuleCollider == null)
        {
            Debug.LogWarning("⚠ CapsuleCollider manquant (recommandé)");
        }
        else
        {
            Debug.Log("✓ CapsuleCollider présent");
        }

        // Check Rigidbody
        if (rb == null)
        {
            Debug.LogWarning("⚠ Rigidbody manquant (recommandé)");
        }
        else
        {
            Debug.Log("✓ Rigidbody présent");
        }

        if (isValid)
        {
            Debug.Log("=== Configuration valide! Le monstre est prêt à être utilisé. ===");
        }
        else
        {
            Debug.LogError("=== Configuration incomplète! Veuillez corriger les erreurs. ===");
        }
    }
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(QuickMonsterSetup))]
public class QuickMonsterSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QuickMonsterSetup setup = (QuickMonsterSetup)target;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Cet outil configure automatiquement tous les composants nécessaires pour le monstre.",
            MessageType.Info
        );

        EditorGUILayout.Space();

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.fontSize = 12;

        if (GUILayout.Button("⚙ SETUP MONSTER COMPONENTS", buttonStyle, GUILayout.Height(40)))
        {
            setup.SetupMonster();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("✓ Validate Configuration", GUILayout.Height(30)))
        {
            setup.ValidateConfiguration();
        }

        if (GUILayout.Button("💾 Create Prefab", GUILayout.Height(30)))
        {
            setup.CreatePrefab();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "1. Cliquez sur 'SETUP MONSTER COMPONENTS'\n" +
            "2. Assignez l'Animator Controller\n" +
            "3. Cliquez sur 'Validate Configuration'\n" +
            "4. Cliquez sur 'Create Prefab'",
            MessageType.None
        );
    }
}
#endif
