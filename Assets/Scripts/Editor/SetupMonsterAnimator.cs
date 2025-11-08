using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;

/// <summary>
/// Script pour configurer automatiquement les états et transitions du MonsterMutant7 Animator Controller
/// </summary>
public class SetupMonsterAnimator : EditorWindow
{
    private AnimatorController animatorController;
    private bool setupComplete = false;

    [MenuItem("LockIn/Setup Monster Animator")]
    public static void ShowWindow()
    {
        GetWindow<SetupMonsterAnimator>("Setup Monster Animator");
    }

    void OnGUI()
    {
        GUILayout.Label("Configuration de l'Animator du Monstre", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        animatorController = (AnimatorController)EditorGUILayout.ObjectField(
            "Animator Controller",
            animatorController,
            typeof(AnimatorController),
            false
        );

        EditorGUILayout.Space();

        if (animatorController == null)
        {
            EditorGUILayout.HelpBox(
                "Assignez le MonsterMutant7 Animator Controller.\n\n" +
                "Il se trouve dans:\n" +
                "Assets/MonsterMutant 7/MonsterMutant7 Animator Controller.controller",
                MessageType.Info
            );

            if (GUILayout.Button("Auto-Find Animator Controller", GUILayout.Height(30)))
            {
                FindAnimatorController();
            }
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Ce script va:\n" +
                "✓ Créer les états Idle, Walk et Run\n" +
                "✓ Configurer les transitions entre états\n" +
                "✓ Utiliser les paramètres Speed, IsWalking, IsRunning\n" +
                "✓ Assigner les animations appropriées",
                MessageType.Info
            );

            EditorGUILayout.Space();

            if (GUILayout.Button("SETUP ANIMATOR STATES & TRANSITIONS", GUILayout.Height(50)))
            {
                SetupAnimator();
            }

            if (setupComplete)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("✓ Configuration terminée avec succès!", MessageType.Info);
            }
        }
    }

    void FindAnimatorController()
    {
        string[] guids = AssetDatabase.FindAssets("MonsterMutant7 Animator Controller t:AnimatorController");
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            Debug.Log($"✓ Animator Controller trouvé: {path}");
        }
        else
        {
            Debug.LogError("✗ MonsterMutant7 Animator Controller non trouvé!");
            EditorUtility.DisplayDialog(
                "Non trouvé",
                "Le MonsterMutant7 Animator Controller n'a pas été trouvé.\n\n" +
                "Assurez-vous qu'il existe dans:\n" +
                "Assets/MonsterMutant 7/",
                "OK"
            );
        }
    }

    void SetupAnimator()
    {
        if (animatorController == null)
        {
            EditorUtility.DisplayDialog("Erreur", "Aucun Animator Controller assigné!", "OK");
            return;
        }

        Debug.Log("=== Configuration de l'Animator Controller ===");

        // Obtenir le layer de base
        AnimatorControllerLayer baseLayer = animatorController.layers[0];
        AnimatorStateMachine stateMachine = baseLayer.stateMachine;

        // Trouver les animations
        AnimationClip idleClip = FindAnimation("idle1");
        AnimationClip walkClip = FindAnimation("walk2");
        AnimationClip runClip = FindAnimation("run1");

        if (idleClip == null || walkClip == null || runClip == null)
        {
            EditorUtility.DisplayDialog(
                "Animations manquantes",
                "Impossible de trouver toutes les animations nécessaires.\n\n" +
                "Vérifiez que les animations suivantes existent:\n" +
                "- idle1\n" +
                "- walk2\n" +
                "- run1",
                "OK"
            );
            return;
        }

        // Créer ou trouver les états
        AnimatorState idleState = FindOrCreateState(stateMachine, "Idle", idleClip);
        AnimatorState walkState = FindOrCreateState(stateMachine, "Walk", walkClip);
        AnimatorState runState = FindOrCreateState(stateMachine, "Run", runClip);

        // Positionner les états dans la grille
        SetStatePosition(stateMachine, idleState, new Vector3(250, 0, 0));
        SetStatePosition(stateMachine, walkState, new Vector3(250, 100, 0));
        SetStatePosition(stateMachine, runState, new Vector3(250, 200, 0));

        // Définir Idle comme état par défaut
        stateMachine.defaultState = idleState;

        // Supprimer les anciennes transitions
        ClearTransitions(idleState);
        ClearTransitions(walkState);
        ClearTransitions(runState);

        // Créer les transitions
        // Idle -> Walk
        AnimatorStateTransition idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.hasExitTime = false;
        idleToWalk.exitTime = 0;
        idleToWalk.duration = 0.2f;
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
        Debug.Log("✓ Transition Idle -> Walk créée");

        // Idle -> Run
        AnimatorStateTransition idleToRun = idleState.AddTransition(runState);
        idleToRun.hasExitTime = false;
        idleToRun.exitTime = 0;
        idleToRun.duration = 0.2f;
        idleToRun.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        Debug.Log("✓ Transition Idle -> Run créée");

        // Walk -> Idle
        AnimatorStateTransition walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.hasExitTime = false;
        walkToIdle.exitTime = 0;
        walkToIdle.duration = 0.2f;
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsWalking");
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        Debug.Log("✓ Transition Walk -> Idle créée");

        // Walk -> Run
        AnimatorStateTransition walkToRun = walkState.AddTransition(runState);
        walkToRun.hasExitTime = false;
        walkToRun.exitTime = 0;
        walkToRun.duration = 0.2f;
        walkToRun.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        Debug.Log("✓ Transition Walk -> Run créée");

        // Run -> Idle
        AnimatorStateTransition runToIdle = runState.AddTransition(idleState);
        runToIdle.hasExitTime = false;
        runToIdle.exitTime = 0;
        runToIdle.duration = 0.2f;
        runToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        runToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsWalking");
        Debug.Log("✓ Transition Run -> Idle créée");

        // Run -> Walk
        AnimatorStateTransition runToWalk = runState.AddTransition(walkState);
        runToWalk.hasExitTime = false;
        runToWalk.exitTime = 0;
        runToWalk.duration = 0.2f;
        runToWalk.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        runToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsWalking");
        Debug.Log("✓ Transition Run -> Walk créée");

        // Sauvegarder les changements
        EditorUtility.SetDirty(animatorController);
        AssetDatabase.SaveAssets();

        setupComplete = true;

        Debug.Log("=== Configuration terminée! ===");
        Debug.Log("États créés: Idle, Walk, Run");
        Debug.Log("Transitions configurées avec les paramètres IsWalking et IsRunning");

        EditorUtility.DisplayDialog(
            "Succès!",
            "L'Animator Controller a été configuré avec succès!\n\n" +
            "États créés:\n" +
            "✓ Idle (idle1)\n" +
            "✓ Walk (walk2)\n" +
            "✓ Run (run1)\n\n" +
            "Les transitions sont basées sur:\n" +
            "• IsWalking (Bool)\n" +
            "• IsRunning (Bool)\n" +
            "• Speed (Float)\n\n" +
            "Vous pouvez maintenant tester le monstre!",
            "Super!"
        );
    }

    AnimationClip FindAnimation(string animationName)
    {
        string[] guids = AssetDatabase.FindAssets($"{animationName} t:AnimationClip");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("MonsterMutant"))
            {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip != null)
                {
                    Debug.Log($"✓ Animation trouvée: {clip.name} ({path})");
                    return clip;
                }
            }
        }
        
        Debug.LogWarning($"⚠ Animation '{animationName}' non trouvée");
        return null;
    }

    AnimatorState FindOrCreateState(AnimatorStateMachine stateMachine, string stateName, AnimationClip clip)
    {
        // Chercher si l'état existe déjà
        foreach (var state in stateMachine.states)
        {
            if (state.state.name == stateName)
            {
                Debug.Log($"✓ État '{stateName}' déjà existant, mise à jour...");
                state.state.motion = clip;
                return state.state;
            }
        }

        // Créer un nouvel état
        AnimatorState newState = stateMachine.AddState(stateName);
        newState.motion = clip;
        Debug.Log($"✓ État '{stateName}' créé avec l'animation '{clip.name}'");
        return newState;
    }

    void ClearTransitions(AnimatorState state)
    {
        state.transitions = new AnimatorStateTransition[0];
    }

    void SetStatePosition(AnimatorStateMachine stateMachine, AnimatorState state, Vector3 position)
    {
        // Trouver le ChildAnimatorState correspondant dans le state machine
        for (int i = 0; i < stateMachine.states.Length; i++)
        {
            if (stateMachine.states[i].state == state)
            {
                ChildAnimatorState childState = stateMachine.states[i];
                childState.position = position;
                stateMachine.states[i] = childState;
                return;
            }
        }
    }
}
