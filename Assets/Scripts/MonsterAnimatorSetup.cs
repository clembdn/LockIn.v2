using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Script utilitaire pour configurer automatiquement l'Animator Controller du monstre
/// </summary>
public class MonsterAnimatorSetup : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("L'Animator Controller à configurer")]
    public AnimatorController animatorController;
    
    [Tooltip("Animation d'idle par défaut")]
    public AnimationClip idleAnimation;
    
    [Tooltip("Animation de course")]
    public AnimationClip runAnimation;

    [ContextMenu("Setup Animator Controller")]
    public void SetupAnimatorController()
    {
        if (animatorController == null)
        {
            Debug.LogError("Veuillez assigner un Animator Controller!");
            return;
        }

        Debug.Log("Configuration de l'Animator Controller...");

        // Créer ou obtenir les paramètres
        AddParameterIfNotExists(animatorController, "Speed", AnimatorControllerParameterType.Float);
        AddParameterIfNotExists(animatorController, "IsRunning", AnimatorControllerParameterType.Bool);

        Debug.Log("Animator Controller configuré avec succès!");
        Debug.Log("Paramètres créés: Speed (Float), IsRunning (Bool)");
        Debug.Log("Vous devez maintenant configurer manuellement les états et transitions dans l'Animator window.");
        
        EditorUtility.SetDirty(animatorController);
    }

    private void AddParameterIfNotExists(AnimatorController controller, string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in controller.parameters)
        {
            if (param.name == paramName)
            {
                Debug.Log($"Paramètre '{paramName}' existe déjà.");
                return;
            }
        }

        controller.AddParameter(paramName, type);
        Debug.Log($"Paramètre '{paramName}' créé.");
    }

    [ContextMenu("Print Available Animations")]
    public void PrintAvailableAnimations()
    {
        Animator animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Aucun Animator trouvé sur ce GameObject!");
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Aucun Animator Controller assigné!");
            return;
        }

        Debug.Log("=== Clips d'animation disponibles ===");
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            Debug.Log($"- {clip.name} (durée: {clip.length}s)");
        }
    }
}

[CustomEditor(typeof(MonsterAnimatorSetup))]
public class MonsterAnimatorSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MonsterAnimatorSetup setup = (MonsterAnimatorSetup)target;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Utilisez 'Setup Animator Controller' pour créer automatiquement les paramètres nécessaires.\n\n" +
            "Utilisez 'Print Available Animations' pour voir toutes les animations disponibles dans la console.",
            MessageType.Info
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Setup Animator Controller", GUILayout.Height(30)))
        {
            setup.SetupAnimatorController();
        }

        if (GUILayout.Button("Print Available Animations", GUILayout.Height(30)))
        {
            setup.PrintAvailableAnimations();
        }
    }
}
#endif
