using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Vérifie et corrige les problèmes courants de l'Animator Controller
/// Menu: LockIn > Fix Animator Controller Issues
/// </summary>
public class FixAnimatorControllerIssues : EditorWindow
{
    [MenuItem("LockIn/Fix Animator Controller Issues")]
    public static void FixAnimatorIssues()
    {
        Debug.Log("=== CORRECTION ANIMATOR CONTROLLER ===\n");

        // Trouver l'Animator Controller
        string[] guids = AssetDatabase.FindAssets("MonsterMutant7 Animator Controller t:AnimatorController");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Animator Controller 'MonsterMutant7 Animator Controller' non trouvé!",
                "OK"
            );
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
        
        if (controller == null)
        {
            Debug.LogError("Impossible de charger le controller!");
            return;
        }

        Debug.Log($"✅ Controller trouvé: {controller.name}\n");

        bool madeChanges = false;

        // 1. Vérifier et configurer l'Update Mode
        var layer = controller.layers[0];
        var stateMachine = layer.stateMachine;

        // 2. Vérifier les états
        Debug.Log("📊 Vérification des états:");
        
        AnimatorState idleState = FindOrGetState(stateMachine, "Idle");
        AnimatorState walkState = FindOrGetState(stateMachine, "Walk");
        AnimatorState runState = FindOrGetState(stateMachine, "Run");

        // 3. Assigner les animations aux états
        Debug.Log("\n🎬 Assignation des animations:");
        
        AnimationClip idleClip = FindAnimationClip("idle1");
        AnimationClip walkClip = FindAnimationClip("walk2");
        AnimationClip runClip = FindAnimationClip("run1");

        if (idleClip != null && idleState.motion == null)
        {
            idleState.motion = idleClip;
            Debug.Log($"✅ Animation assignée à Idle: {idleClip.name}");
            madeChanges = true;
        }
        else if (idleState.motion != null)
        {
            Debug.Log($"✅ Idle a déjà une animation: {idleState.motion.name}");
        }

        if (walkClip != null && walkState.motion == null)
        {
            walkState.motion = walkClip;
            Debug.Log($"✅ Animation assignée à Walk: {walkClip.name}");
            madeChanges = true;
        }
        else if (walkState.motion != null)
        {
            Debug.Log($"✅ Walk a déjà une animation: {walkState.motion.name}");
        }

        if (runClip != null && runState.motion == null)
        {
            runState.motion = runClip;
            Debug.Log($"✅ Animation assignée à Run: {runClip.name}");
            madeChanges = true;
        }
        else if (runState.motion != null)
        {
            Debug.Log($"✅ Run a déjà une animation: {runState.motion.name}");
        }

        // 4. Configurer les vitesses d'animation
        Debug.Log("\n⚡ Configuration des vitesses:");
        if (idleState.speed != 1f)
        {
            idleState.speed = 1f;
            Debug.Log("✅ Vitesse Idle = 1");
            madeChanges = true;
        }
        if (walkState.speed != 1f)
        {
            walkState.speed = 1f;
            Debug.Log("✅ Vitesse Walk = 1");
            madeChanges = true;
        }
        if (runState.speed != 1f)
        {
            runState.speed = 1f;
            Debug.Log("✅ Vitesse Run = 1");
            madeChanges = true;
        }

        // 5. Vérifier et créer les transitions
        Debug.Log("\n🔄 Vérification des transitions:");
        
        // Transitions de Idle vers Walk/Run
        if (!HasTransitionTo(idleState, walkState))
        {
            var transition = idleState.AddTransition(walkState);
            transition.hasExitTime = false;
            transition.duration = 0.1f;
            transition.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
            Debug.Log("✅ Transition créée: Idle → Walk");
            madeChanges = true;
        }

        if (!HasTransitionTo(idleState, runState))
        {
            var transition = idleState.AddTransition(runState);
            transition.hasExitTime = false;
            transition.duration = 0.1f;
            transition.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
            Debug.Log("✅ Transition créée: Idle → Run");
            madeChanges = true;
        }

        // Transitions de Walk vers Idle/Run
        if (!HasTransitionTo(walkState, idleState))
        {
            var transition = walkState.AddTransition(idleState);
            transition.hasExitTime = false;
            transition.duration = 0.1f;
            transition.AddCondition(AnimatorConditionMode.IfNot, 0, "IsWalking");
            transition.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
            Debug.Log("✅ Transition créée: Walk → Idle");
            madeChanges = true;
        }

        if (!HasTransitionTo(walkState, runState))
        {
            var transition = walkState.AddTransition(runState);
            transition.hasExitTime = false;
            transition.duration = 0.1f;
            transition.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
            Debug.Log("✅ Transition créée: Walk → Run");
            madeChanges = true;
        }

        // Transitions de Run vers Idle/Walk
        if (!HasTransitionTo(runState, idleState))
        {
            var transition = runState.AddTransition(idleState);
            transition.hasExitTime = false;
            transition.duration = 0.1f;
            transition.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
            transition.AddCondition(AnimatorConditionMode.IfNot, 0, "IsWalking");
            Debug.Log("✅ Transition créée: Run → Idle");
            madeChanges = true;
        }

        if (!HasTransitionTo(runState, walkState))
        {
            var transition = runState.AddTransition(walkState);
            transition.hasExitTime = false;
            transition.duration = 0.1f;
            transition.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
            transition.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
            Debug.Log("✅ Transition créée: Run → Walk");
            madeChanges = true;
        }

        if (madeChanges)
        {
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            Debug.Log("\n✅ Modifications sauvegardées!");
        }
        else
        {
            Debug.Log("\n✅ Tout est déjà configuré correctement!");
        }

        Debug.Log("\n═══════════════════════════════════════\n");

        EditorUtility.DisplayDialog(
            "Correction terminée!",
            "L'Animator Controller a été vérifié et corrigé.\n\n" +
            "Les états Idle, Walk et Run sont maintenant correctement configurés avec:\n" +
            "• Animations assignées\n" +
            "• Transitions avec conditions\n" +
            "• Vitesses normales\n\n" +
            "Testez maintenant en Play mode!",
            "Super!"
        );
    }

    static AnimatorState FindOrGetState(AnimatorStateMachine stateMachine, string stateName)
    {
        foreach (var state in stateMachine.states)
        {
            if (state.state.name == stateName)
            {
                Debug.Log($"  ✅ État trouvé: {stateName}");
                return state.state;
            }
        }

        Debug.LogWarning($"  ⚠️ État '{stateName}' non trouvé, création...");
        var newState = stateMachine.AddState(stateName);
        return newState;
    }

    static AnimationClip FindAnimationClip(string partialName)
    {
        string[] guids = AssetDatabase.FindAssets($"MutantMonster2@{partialName} t:AnimationClip");
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            
            foreach (Object asset in assets)
            {
                if (asset is AnimationClip)
                {
                    AnimationClip clip = asset as AnimationClip;
                    if (!clip.name.Contains("__preview__"))
                    {
                        Debug.Log($"    Trouvé: {clip.name}");
                        return clip;
                    }
                }
            }
        }
        
        Debug.LogWarning($"    Animation '{partialName}' non trouvée");
        return null;
    }

    static bool HasTransitionTo(AnimatorState fromState, AnimatorState toState)
    {
        foreach (var transition in fromState.transitions)
        {
            if (transition.destinationState == toState)
            {
                return true;
            }
        }
        return false;
    }
}
