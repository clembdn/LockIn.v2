using UnityEngine;
using UnityEditor;

/// <summary>
/// Script pour configurer automatiquement les animations du monstre dans la scène
/// Menu: LockIn > Fix Monster Animations
/// </summary>
public class FixMonsterAnimations : EditorWindow
{
    [MenuItem("LockIn/Fix Monster Animations NOW!")]
    public static void FixAnimations()
    {
        Debug.Log("=== Configuration des animations du monstre ===");

        // 1. Trouver le monstre dans la scène
        MonsterAI monsterAI = Object.FindFirstObjectByType<MonsterAI>();
        
        if (monsterAI == null)
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Aucun monstre trouvé dans la scène!\n\nAjoutez d'abord un monstre avec:\nLockIn > Add Monster to SampleScene NOW!",
                "OK"
            );
            return;
        }

        GameObject monster = monsterAI.gameObject;
        Debug.Log($"✓ Monstre trouvé: {monster.name}");

        // 2. Obtenir l'Animator
        Animator animator = monster.GetComponent<Animator>();
        if (animator == null)
        {
            animator = monster.GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Aucun Animator trouvé sur le monstre!",
                "OK"
            );
            return;
        }

        Debug.Log("✓ Animator trouvé");

        // 3. Vérifier et assigner l'Animator Controller
        if (animator.runtimeAnimatorController == null)
        {
            string[] controllerGuids = AssetDatabase.FindAssets("MonsterMutant7 Animator Controller t:AnimatorController");
            if (controllerGuids.Length > 0)
            {
                string controllerPath = AssetDatabase.GUIDToAssetPath(controllerGuids[0]);
                RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
                animator.runtimeAnimatorController = controller;
                Debug.Log("✓ Animator Controller assigné");
            }
            else
            {
                Debug.LogError("✗ Animator Controller non trouvé!");
                return;
            }
        }

        // 4. Assigner l'Animator au MonsterAI
        monsterAI.animator = animator;
        Debug.Log("✓ Animator assigné au MonsterAI");

        // 5. Configurer les paramètres du MonsterAI
        monsterAI.speedParameterName = "Speed";
        monsterAI.isRunningParameterName = "IsRunning";
        monsterAI.isWalkingParameterName = "IsWalking";
        Debug.Log("✓ Paramètres d'animation configurés");

        // 6. Vérifier que l'Animator est en Update Mode = Normal
        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        Debug.Log("✓ Update Mode configuré");

        // 7. Marquer les objets comme modifiés
        EditorUtility.SetDirty(monster);
        EditorUtility.SetDirty(monsterAI);
        EditorUtility.SetDirty(animator);

        // 8. Afficher les paramètres de l'Animator
        Debug.Log("\n=== Paramètres de l'Animator ===");
        foreach (var param in animator.parameters)
        {
            Debug.Log($"  - {param.name} ({param.type})");
        }

        Debug.Log("\n=== Configuration terminée! ===");
        Debug.Log("Paramètres MonsterAI:");
        Debug.Log($"  Walk Speed: {monsterAI.walkSpeed}");
        Debug.Log($"  Run Speed: {monsterAI.runSpeed}");
        Debug.Log($"  Run Distance: {monsterAI.runDistance}");
        Debug.Log($"  Stopping Distance: {monsterAI.stoppingDistance}");

        EditorUtility.DisplayDialog(
            "Succès!",
            "Les animations du monstre ont été configurées!\n\n" +
            "Configuration:\n" +
            "✓ Animator Controller assigné\n" +
            "✓ Paramètres Speed, IsWalking, IsRunning\n" +
            "✓ Update Mode configuré\n\n" +
            "Le monstre devrait maintenant s'animer correctement!\n\n" +
            "Appuyez sur Play pour tester! ▶️",
            "Super!"
        );
    }
}
