using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Script de test pour déclencher la transformation monstre → robot avec la touche K.
/// Attache ce script à n'importe quel GameObject dans la scène (ou au monstre directement).
/// </summary>
public class TransformationTestTrigger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Le monstre avec le component MonsterToRobotTransition. Auto-trouvé si null.")]
    public MonsterToRobotTransition monsterTransition;

    [Header("Settings")]
    [Tooltip("Touche pour déclencher la transformation (défaut: K).")]
    public KeyCode triggerKey = KeyCode.K;

    [Header("Debug")]
    [Tooltip("Afficher les logs de test.")]
    public bool debugMode = true;

    void Start()
    {
        // Auto-find monster if not assigned
        if (monsterTransition == null)
        {
            monsterTransition = FindFirstObjectByType<MonsterToRobotTransition>();
            
            if (monsterTransition != null && debugMode)
            {
                Debug.Log($"TransformationTestTrigger: Found MonsterToRobotTransition on '{monsterTransition.name}'");
            }
        }

        if (monsterTransition == null)
        {
            Debug.LogWarning("TransformationTestTrigger: No MonsterToRobotTransition found! Assign manually or add component to monster.");
        }
        else if (debugMode)
        {
            Debug.Log($"TransformationTestTrigger: Press '{triggerKey}' to trigger transformation.");
        }
    }

    void Update()
    {
        bool keyPressed = false;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.kKey.wasPressedThisFrame)
            keyPressed = true;
#else
        if (Input.GetKeyDown(triggerKey))
            keyPressed = true;
#endif

        if (keyPressed && monsterTransition != null)
        {
            if (debugMode)
            {
                Debug.Log("TransformationTestTrigger: K pressed! Triggering transformation...");
            }

            monsterTransition.TriggerTransformation();
        }
    }
}
