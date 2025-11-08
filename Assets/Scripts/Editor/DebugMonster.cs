using UnityEngine;
using UnityEditor;

/// <summary>
/// Script pour activer le debug du monstre
/// </summary>
public class DebugMonster : EditorWindow
{
    [MenuItem("LockIn/Debug Monster Info")]
    public static void ShowDebugInfo()
    {
        Debug.Log("=== DEBUG MONSTRE ===");

        MonsterAI[] monsters = Object.FindObjectsByType<MonsterAI>(FindObjectsSortMode.None);
        
        if (monsters.Length == 0)
        {
            Debug.LogWarning("Aucun monstre trouvé dans la scène!");
            EditorUtility.DisplayDialog(
                "Aucun monstre",
                "Aucun monstre trouvé dans la scène!\n\nAjoutez d'abord un monstre.",
                "OK"
            );
            return;
        }

        foreach (MonsterAI monster in monsters)
        {
            Debug.Log($"\n--- Monstre: {monster.name} ---");
            Debug.Log($"Position: {monster.transform.position}");
            
            // Activer le debug
            monster.showDebugInfo = true;
            EditorUtility.SetDirty(monster);
            Debug.Log("✓ Debug activé");
            
            // Info joueur
            if (monster.player != null)
            {
                Debug.Log($"✓ Joueur trouvé: {monster.player.name}");
                float distance = Vector3.Distance(monster.transform.position, monster.player.position);
                Debug.Log($"  Distance au joueur: {distance:F2}m");
            }
            else
            {
                Debug.LogError("✗ Joueur NON assigné!");
            }
            
            // Info Animator
            if (monster.animator != null)
            {
                Debug.Log($"✓ Animator assigné");
                if (monster.animator.runtimeAnimatorController != null)
                {
                    Debug.Log($"  Controller: {monster.animator.runtimeAnimatorController.name}");
                    Debug.Log("  Paramètres:");
                    foreach (var param in monster.animator.parameters)
                    {
                        Debug.Log($"    - {param.name} ({param.type})");
                    }
                }
                else
                {
                    Debug.LogError("  ✗ Animator Controller NON assigné!");
                }
            }
            else
            {
                Debug.LogError("✗ Animator NON assigné!");
            }
            
            // Info vitesses
            Debug.Log($"Vitesses:");
            Debug.Log($"  Walk Speed: {monster.walkSpeed}");
            Debug.Log($"  Run Speed: {monster.runSpeed}");
            Debug.Log($"  Run Distance: {monster.runDistance}");
            Debug.Log($"  Stopping Distance: {monster.stoppingDistance}");
            
            // Info composants
            UnityEngine.AI.NavMeshAgent navAgent = monster.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent != null)
            {
                Debug.Log($"NavMeshAgent: Activé={navAgent.enabled}, OnNavMesh={navAgent.isOnNavMesh}");
            }
            else
            {
                Debug.Log("NavMeshAgent: Aucun");
            }
            
            Rigidbody rb = monster.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log($"Rigidbody: Mass={rb.mass}, UseGravity={rb.useGravity}, IsKinematic={rb.isKinematic}");
            }
        }

        Debug.Log("\n=== FIN DEBUG ===");
        Debug.Log("Le debug est maintenant activé. Lancez Play pour voir les logs en temps réel.");

        EditorUtility.DisplayDialog(
            "Debug activé",
            $"Debug activé pour {monsters.Length} monstre(s)!\n\n" +
            "Informations affichées dans la Console.\n\n" +
            "Lancez Play pour voir les logs en temps réel.",
            "OK"
        );
    }

    [MenuItem("LockIn/Test Monster Movement")]
    public static void TestMovement()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog(
                "Mode Play requis",
                "Cette fonction ne fonctionne qu'en mode Play.\n\nAppuyez sur Play d'abord!",
                "OK"
            );
            return;
        }

        MonsterAI monster = Object.FindFirstObjectByType<MonsterAI>();
        if (monster == null)
        {
            Debug.LogError("Aucun monstre trouvé!");
            return;
        }

        Debug.Log("=== TEST MOUVEMENT ===");
        
        if (monster.animator != null)
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"État animation actuel: {stateInfo.shortNameHash}");
            Debug.Log($"Temps normalisé: {stateInfo.normalizedTime:F2}");
            
            foreach (var param in monster.animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Float)
                {
                    float value = monster.animator.GetFloat(param.name);
                    Debug.Log($"Paramètre {param.name}: {value:F2}");
                }
                else if (param.type == AnimatorControllerParameterType.Bool)
                {
                    bool value = monster.animator.GetBool(param.name);
                    Debug.Log($"Paramètre {param.name}: {value}");
                }
            }
        }
        
        if (monster.player != null)
        {
            float distance = Vector3.Distance(monster.transform.position, monster.player.position);
            Debug.Log($"Distance au joueur: {distance:F2}m");
        }
    }
}
