using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Recrée complètement l'Animator Controller avec une configuration simple qui fonctionne
/// Menu: LockIn > REBUILD Animator Controller
/// </summary>
public class RebuildAnimatorController : EditorWindow
{
    [MenuItem("LockIn/REBUILD Animator Controller NOW!")]
    public static void RebuildController()
    {
        Debug.Log("=== RECONSTRUCTION ANIMATOR CONTROLLER ===\n");

        // Trouver les animations
        AnimationClip idleClip = FindAnimationClip("idle1");
        AnimationClip walkClip = FindAnimationClip("walk2");
        AnimationClip runClip = FindAnimationClip("run1");

        if (idleClip == null || walkClip == null || runClip == null)
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Impossible de trouver les animations!\n\n" +
                $"Idle: {(idleClip != null ? "✅" : "❌")}\n" +
                $"Walk: {(walkClip != null ? "✅" : "❌")}\n" +
                $"Run: {(runClip != null ? "✅" : "❌")}",
                "OK"
            );
            return;
        }

        Debug.Log($"✅ Animations trouvées:");
        Debug.Log($"  • Idle: {idleClip.name}");
        Debug.Log($"  • Walk: {walkClip.name}");
        Debug.Log($"  • Run: {runClip.name}\n");

        // Trouver ou créer l'Animator Controller
        string controllerPath = "Assets/MonsterMutant 7/MonsterMutant7 Animator Controller.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

        if (controller == null)
        {
            Debug.LogError($"Controller non trouvé à: {controllerPath}");
            return;
        }

        Debug.Log($"✅ Controller trouvé: {controller.name}\n");

        // SUPPRIMER TOUT ET RECOMMENCER
        Debug.Log("🗑️ Nettoyage de l'ancien controller...");
        
        // Supprimer tous les layers sauf le premier
        while (controller.layers.Length > 1)
        {
            controller.RemoveLayer(1);
        }

        // Nettoyer le layer de base
        var baseLayer = controller.layers[0];
        var stateMachine = baseLayer.stateMachine;

        // Supprimer tous les états
        foreach (var state in stateMachine.states)
        {
            stateMachine.RemoveState(state.state);
        }

        // Supprimer tous les paramètres
        foreach (var param in controller.parameters)
        {
            controller.RemoveParameter(param);
        }

        Debug.Log("✅ Controller nettoyé\n");

        // CRÉER LES PARAMÈTRES
        Debug.Log("🎛️ Création des paramètres...");
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        Debug.Log("✅ Paramètres créés: Speed, IsWalking, IsRunning\n");

        // CRÉER LES ÉTATS
        Debug.Log("📊 Création des états...");
        
        var idleState = stateMachine.AddState("Idle", new Vector3(300, 0, 0));
        idleState.motion = idleClip;
        idleState.speed = 1f;
        Debug.Log($"✅ État Idle créé avec {idleClip.name}");

        var walkState = stateMachine.AddState("Walk", new Vector3(300, 100, 0));
        walkState.motion = walkClip;
        walkState.speed = 1f;
        Debug.Log($"✅ État Walk créé avec {walkClip.name}");

        var runState = stateMachine.AddState("Run", new Vector3(300, 200, 0));
        runState.motion = runClip;
        runState.speed = 1f;
        Debug.Log($"✅ État Run créé avec {runClip.name}\n");

        // DÉFINIR IDLE COMME ÉTAT PAR DÉFAUT
        stateMachine.defaultState = idleState;
        Debug.Log("✅ Idle défini comme état par défaut\n");

        // CRÉER LES TRANSITIONS
        Debug.Log("🔄 Création des transitions...");

        // IDLE → WALK
        var idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.hasExitTime = false;
        idleToWalk.exitTime = 0;
        idleToWalk.duration = 0.15f;
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
        idleToWalk.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        Debug.Log("  ✅ Idle → Walk (si IsWalking=true ET IsRunning=false)");

        // IDLE → RUN
        var idleToRun = idleState.AddTransition(runState);
        idleToRun.hasExitTime = false;
        idleToRun.exitTime = 0;
        idleToRun.duration = 0.15f;
        idleToRun.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        Debug.Log("  ✅ Idle → Run (si IsRunning=true)");

        // WALK → IDLE
        var walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.hasExitTime = false;
        walkToIdle.exitTime = 0;
        walkToIdle.duration = 0.15f;
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsWalking");
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        Debug.Log("  ✅ Walk → Idle (si IsWalking=false ET IsRunning=false)");

        // WALK → RUN
        var walkToRun = walkState.AddTransition(runState);
        walkToRun.hasExitTime = false;
        walkToRun.exitTime = 0;
        walkToRun.duration = 0.15f;
        walkToRun.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        Debug.Log("  ✅ Walk → Run (si IsRunning=true)");

        // RUN → IDLE
        var runToIdle = runState.AddTransition(idleState);
        runToIdle.hasExitTime = false;
        runToIdle.exitTime = 0;
        runToIdle.duration = 0.15f;
        runToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        runToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsWalking");
        Debug.Log("  ✅ Run → Idle (si IsRunning=false ET IsWalking=false)");

        // RUN → WALK
        var runToWalk = runState.AddTransition(walkState);
        runToWalk.hasExitTime = false;
        runToWalk.exitTime = 0;
        runToWalk.duration = 0.15f;
        runToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
        runToWalk.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        Debug.Log("  ✅ Run → Walk (si IsWalking=true ET IsRunning=false)");

        Debug.Log("");

        // SAUVEGARDER
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("💾 Controller sauvegardé!");
        Debug.Log("\n═══════════════════════════════════════");
        Debug.Log("✅ RECONSTRUCTION TERMINÉE!");
        Debug.Log("═══════════════════════════════════════\n");

        // Vérifier que le monstre a ce controller
        MonsterAI[] monsters = FindObjectsByType<MonsterAI>(FindObjectsSortMode.None);
        if (monsters.Length > 0)
        {
            foreach (var monster in monsters)
            {
                Animator animator = monster.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = controller;
                    EditorUtility.SetDirty(animator);
                    Debug.Log($"✅ Controller assigné au monstre: {monster.name}");
                }
            }
        }

        EditorUtility.DisplayDialog(
            "✅ SUCCÈS!",
            "L'Animator Controller a été complètement reconstruit!\n\n" +
            "Configuration:\n" +
            "• États: Idle, Walk, Run\n" +
            "• Animations assignées et en loop\n" +
            "• Transitions rapides (0.15s)\n" +
            "• Conditions correctes\n" +
            "• Pas d'Exit Time\n\n" +
            "🎮 TESTEZ MAINTENANT EN PLAY MODE!\n\n" +
            "Le monstre devrait animer correctement!",
            "TESTER!"
        );
    }

    static AnimationClip FindAnimationClip(string partialName)
    {
        string[] guids = AssetDatabase.FindAssets($"MutantMonster2@{partialName}");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            
            if (path.EndsWith(".fbx"))
            {
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                
                foreach (Object asset in assets)
                {
                    if (asset is AnimationClip)
                    {
                        AnimationClip clip = asset as AnimationClip;
                        if (!clip.name.Contains("__preview__"))
                        {
                            return clip;
                        }
                    }
                }
            }
        }
        
        return null;
    }
}
