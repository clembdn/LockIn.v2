using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Diagnostique complet de l'Animator du monstre
/// Menu: LockIn > Diagnose Monster Animator
/// </summary>
public class DiagnoseMonsterAnimator : EditorWindow
{
    [MenuItem("LockIn/Diagnose Monster Animator")]
    public static void DiagnoseAnimator()
    {
        Debug.Log("=== DIAGNOSTIC ANIMATOR MONSTRE ===\n");

        // Trouver le monstre dans la scène
        MonsterAI[] monsters = FindObjectsByType<MonsterAI>(FindObjectsSortMode.None);
        
        if (monsters.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Aucun monstre trouvé",
                "Aucun monstre avec MonsterAI trouvé dans la scène.\n\n" +
                "Ajoutez d'abord le monstre avec:\n" +
                "LockIn > Add Monster to SampleScene NOW!",
                "OK"
            );
            return;
        }

        foreach (MonsterAI monster in monsters)
        {
            Debug.Log($"\n🦖 MONSTRE: {monster.gameObject.name}");
            Debug.Log("═══════════════════════════════════════\n");

            Animator animator = monster.GetComponent<Animator>();
            
            if (animator == null)
            {
                Debug.LogError("❌ PAS D'ANIMATOR sur le monstre!");
                continue;
            }

            // 1. Vérifier l'Animator Controller
            Debug.Log("📋 ANIMATOR CONTROLLER:");
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError("❌ Aucun Animator Controller assigné!");
                continue;
            }
            else
            {
                Debug.Log($"✅ Controller: {animator.runtimeAnimatorController.name}");
            }

            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            
            if (controller == null)
            {
                Debug.LogWarning("⚠️ Impossible de lire le controller");
                continue;
            }

            // 2. Vérifier les paramètres
            Debug.Log("\n🎛️ PARAMÈTRES:");
            var parameters = animator.parameters;
            if (parameters.Length == 0)
            {
                Debug.LogError("❌ Aucun paramètre trouvé!");
            }
            else
            {
                foreach (var param in parameters)
                {
                    string value = "";
                    switch (param.type)
                    {
                        case AnimatorControllerParameterType.Float:
                            value = animator.GetFloat(param.name).ToString("F2");
                            break;
                        case AnimatorControllerParameterType.Bool:
                            value = animator.GetBool(param.name).ToString();
                            break;
                        case AnimatorControllerParameterType.Int:
                            value = animator.GetInteger(param.name).ToString();
                            break;
                    }
                    Debug.Log($"  • {param.name} ({param.type}) = {value}");
                }
            }

            // 3. Vérifier les états (layers)
            Debug.Log("\n📊 ÉTATS (LAYERS):");
            for (int i = 0; i < controller.layers.Length; i++)
            {
                var layer = controller.layers[i];
                Debug.Log($"\n  Layer {i}: {layer.name}");
                
                var stateMachine = layer.stateMachine;
                Debug.Log($"  États dans ce layer: {stateMachine.states.Length}");
                
                foreach (var state in stateMachine.states)
                {
                    string animName = "AUCUNE";
                    if (state.state.motion != null)
                    {
                        animName = state.state.motion.name;
                    }
                    
                    Debug.Log($"    • {state.state.name} → Animation: {animName}");
                    
                    // Vérifier les transitions
                    if (state.state.transitions.Length > 0)
                    {
                        Debug.Log($"      Transitions: {state.state.transitions.Length}");
                        foreach (var transition in state.state.transitions)
                        {
                            string destName = transition.destinationState != null ? transition.destinationState.name : "ANY";
                            Debug.Log($"        → vers {destName}");
                            
                            if (transition.conditions.Length > 0)
                            {
                                foreach (var condition in transition.conditions)
                                {
                                    Debug.Log($"          Condition: {condition.parameter} {condition.mode} {condition.threshold}");
                                }
                            }
                            else
                            {
                                Debug.LogWarning("          ⚠️ Pas de conditions!");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"      ⚠️ Aucune transition depuis {state.state.name}!");
                    }
                }
            }

            // 4. État en cours (Play mode seulement)
            if (Application.isPlaying)
            {
                Debug.Log("\n🎬 ÉTAT ACTUEL (Play Mode):");
                var currentState = animator.GetCurrentAnimatorStateInfo(0);
                Debug.Log($"  État actuel: {currentState.shortNameHash}");
                Debug.Log($"  Temps normalisé: {currentState.normalizedTime:F2}");
                Debug.Log($"  Longueur: {currentState.length:F2}s");
                Debug.Log($"  Loop: {currentState.loop}");
                Debug.Log($"  Speed: {currentState.speed}");
                
                // Vérifier les valeurs des paramètres
                Debug.Log("\n  Valeurs actuelles:");
                Debug.Log($"    Speed = {animator.GetFloat("Speed"):F2}");
                Debug.Log($"    IsWalking = {animator.GetBool("IsWalking")}");
                Debug.Log($"    IsRunning = {animator.GetBool("IsRunning")}");
            }
            else
            {
                Debug.Log("\n⚠️ Lancez le Play mode pour voir l'état actuel de l'animation");
            }

            Debug.Log("\n═══════════════════════════════════════\n");
        }

        string message = Application.isPlaying
            ? "Diagnostic complet affiché dans la Console!\n\nVérifiez les états, transitions et paramètres."
            : "Diagnostic affiché dans la Console!\n\nPour voir l'état actuel de l'animation, lancez le Play mode et relancez ce diagnostic.";

        EditorUtility.DisplayDialog(
            "Diagnostic terminé",
            message,
            "OK"
        );
    }
}
