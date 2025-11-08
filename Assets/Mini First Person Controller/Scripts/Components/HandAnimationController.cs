using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Simple controller to drive hand animations:
/// - Plays the Idle state at Start (the Animator should have an "Idle" state set as default).
/// - Triggers the "Collect" parameter when the player presses E.
/// - Triggers the "Attack" parameter when the player right-clicks (mouse button 1).
///
/// Works with both the legacy Input Manager and the new Input System using compilation symbols.
/// Setup in Unity:
/// 1) Create an Animator Controller for your hands.
/// 2) Add an "Idle" animation state (loop enabled on the clip) and a "Collect" animation.
/// 3) Add a Trigger parameter named "Collect" in the Animator.
/// 4) Create a transition from AnyState (or Idle) to Collect using the Collect trigger,
///    and transition back to Idle after the collect animation (use Exit Time or a transition back).
/// 5) Assign the Animator Controller to the Animator component on the hands GameObject and
///    attach this script to a convenient GameObject (the hands root). Assign the Animator reference.
///
/// Note: this script only triggers the animations. Configure looping and transitions in the Animator window.
/// </summary>
public class HandAnimationController : MonoBehaviour
{
    [Tooltip("Animator that controls the hands (assign the Animator component)")]
    [SerializeField] private Animator handsAnimator;

    [Tooltip("Name of the idle state. Must match the state name in the Animator.")]
    [SerializeField] private string idleStateName = "Idle";

    [Tooltip("Name of the Trigger parameter used to play the collect animation.")]
    [SerializeField] private string collectTriggerName = "Collect";
    [Tooltip("Optional: name of the Collect state to avoid retrigger while it's playing. Leave empty to skip the check.")]
    [SerializeField] private string collectStateName = "Collect";

    [Header("Attack Settings")]
    [Tooltip("Name of the Trigger parameter used to play the attack animation.")]
    [SerializeField] private string attackTriggerName = "Attack";
    [Tooltip("Optional: name of the Attack state to avoid retrigger while it's playing. Leave empty to skip the check.")]
    [SerializeField] private string attackStateName = "Attack";

    // cached hashes for performance
    private int collectTriggerHash = -1;
    private int idleStateHash = -1;
    private int collectStateHash = -1;
    private bool collectParameterExists = false;
    private int attackTriggerHash = -1;
    private int attackStateHash = -1;
    private bool attackParameterExists = false;

    [Header("Debug")]
    [Tooltip("If true, will log animator current state every second.")]
    [SerializeField] private bool debugVerbose = false;
    private float _nextDebugTime;

    private void Reset()
    {
        // Try to auto-assign an Animator if present on the same GameObject
        handsAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (handsAnimator == null)
        {
            Debug.LogWarning("HandAnimationController: no Animator assigned.");
            return;
        }
        // Cache hashes
        collectTriggerHash = Animator.StringToHash(collectTriggerName);
        idleStateHash = Animator.StringToHash(idleStateName);
        if (!string.IsNullOrEmpty(collectStateName)) collectStateHash = Animator.StringToHash(collectStateName);
    attackTriggerHash = Animator.StringToHash(attackTriggerName);
    if (!string.IsNullOrEmpty(attackStateName)) attackStateHash = Animator.StringToHash(attackStateName);

        // Verify trigger parameter exists
        collectParameterExists = HasTriggerParameter(collectTriggerName);
        if (!collectParameterExists)
        {
            string paramList = "";
            foreach (var p in handsAnimator.parameters)
                paramList += $"{p.name}({p.type}) ";
            Debug.LogWarning($"HandAnimationController: Trigger parameter '{collectTriggerName}' not found. Available: {paramList}\nAdd a Trigger named '{collectTriggerName}' in the Animator Parameters panel.");
        }

        attackParameterExists = HasTriggerParameter(attackTriggerName);
        if (!attackParameterExists)
        {
            string paramList = "";
            foreach (var p in handsAnimator.parameters)
                paramList += $"{p.name}({p.type}) ";
            Debug.LogWarning($"HandAnimationController: Trigger parameter '{attackTriggerName}' not found. Available: {paramList}\nAdd a Trigger named '{attackTriggerName}' in the Animator Parameters panel.");
        }

    Debug.Log($"HandAnimationController: starting. Animator='{handsAnimator.name}', IdleState='{idleStateName}', CollectTrigger='{collectTriggerName}', AttackTrigger='{attackTriggerName}'");

        // Ensure animator component is enabled so it updates in Play Mode
        if (!handsAnimator.enabled)
        {
            handsAnimator.enabled = true;
            Debug.Log("HandAnimationController: Animator was disabled, enabling it.");
        }

        // Check that the Idle state exists on layer 0 (helps debugging wrong state names)
        if (idleStateHash != 0 && !handsAnimator.HasState(0, idleStateHash))
        {
            Debug.LogWarning($"HandAnimationController: Animator does not contain a state named '{idleStateName}' on layer 0. Make sure state names match.");
        }

        // Play the Idle state at startup (ensure Idle exists and is configured to loop in the Animator)
        handsAnimator.Play(idleStateName, 0, 0f);
        if (debugVerbose)
            Debug.Log("HandAnimationController: Requested play of Idle at Start.");
    }

    // Helper context menu commands to test from the Inspector without using input
    [ContextMenu("Play Idle (Inspector)")]
    private void ContextPlayIdle()
    {
        if (handsAnimator == null)
        {
            Debug.LogWarning("HandAnimationController: no Animator assigned (ContextPlayIdle).");
            return;
        }
        Debug.Log("HandAnimationController: ContextPlayIdle -> playing Idle");
        handsAnimator.Play(idleStateName, 0, 0f);
    }

    [ContextMenu("Trigger Collect (Inspector)")]
    private void ContextTriggerCollect()
    {
        if (handsAnimator == null)
        {
            Debug.LogWarning("HandAnimationController: no Animator assigned (ContextTriggerCollect).");
            return;
        }
        Debug.Log("HandAnimationController: ContextTriggerCollect -> setting Collect trigger");
        if (collectTriggerHash != -1)
            handsAnimator.SetTrigger(collectTriggerHash);
        else
            handsAnimator.SetTrigger(collectTriggerName);
    }

    [ContextMenu("Trigger Attack (Inspector)")]
    private void ContextTriggerAttack()
    {
        if (handsAnimator == null)
        {
            Debug.LogWarning("HandAnimationController: no Animator assigned (ContextTriggerAttack).");
            return;
        }
        Debug.Log("HandAnimationController: ContextTriggerAttack -> setting Attack trigger");
        if (attackTriggerHash != -1)
            handsAnimator.SetTrigger(attackTriggerHash);
        else
            handsAnimator.SetTrigger(attackTriggerName);
    }

    private void Update()
    {
        if (handsAnimator == null) return;

    bool collectPressed = false;
    bool attackPressed = false;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.eKey.wasPressedThisFrame)
                collectPressed = true;
            // Right mouse button for attack
            var mouse = Mouse.current;
            if (mouse != null && mouse.rightButton.wasPressedThisFrame)
                attackPressed = true;
        }
        // also support common gamepad buttons (South for collect, West for attack?)
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            if (!collectPressed && gamepad.buttonSouth.wasPressedThisFrame)
                collectPressed = true;
            if (!attackPressed && gamepad.buttonWest.wasPressedThisFrame)
                attackPressed = true; // Square/X depending on layout
        }
#else
        if (Input.GetKeyDown(KeyCode.E))
            collectPressed = true;
        if (Input.GetMouseButtonDown(1)) // right click
            attackPressed = true;
#endif

        if (collectPressed)
        {
            // Prevent retriggering if the collect state is already playing (optional)
            if (collectStateHash != 0)
            {
                var info = handsAnimator.GetCurrentAnimatorStateInfo(0);
                if (info.shortNameHash == collectStateHash || info.IsName(collectStateName))
                {
                    Debug.Log("HandAnimationController: collect pressed but Collect animation already playing -- ignoring.");
                    return;
                }
            }

            Debug.Log("HandAnimationController: Collect input detected -> setting trigger");
            if (collectParameterExists)
            {
                if (collectTriggerHash != -1)
                    handsAnimator.SetTrigger(collectTriggerHash);
                else
                    handsAnimator.SetTrigger(collectTriggerName);
            }
            else
            {
                Debug.LogWarning($"HandAnimationController: Cannot set trigger '{collectTriggerName}' because it does not exist.");
            }
        }

        if (attackPressed)
        {
            // Prevent retriggering if attack state already playing
            if (attackStateHash != 0)
            {
                var info = handsAnimator.GetCurrentAnimatorStateInfo(0);
                if (info.shortNameHash == attackStateHash || info.IsName(attackStateName))
                {
                    Debug.Log("HandAnimationController: attack pressed but Attack animation already playing -- ignoring.");
                    return;
                }
            }

            Debug.Log("HandAnimationController: Attack input detected -> setting trigger");
            if (attackParameterExists)
            {
                if (attackTriggerHash != -1)
                    handsAnimator.SetTrigger(attackTriggerHash);
                else
                    handsAnimator.SetTrigger(attackTriggerName);
            }
            else
            {
                Debug.LogWarning($"HandAnimationController: Cannot set trigger '{attackTriggerName}' because it does not exist.");
            }
        }

        // Periodic verbose state logging
        if (debugVerbose)
        {
            var st = handsAnimator.GetCurrentAnimatorStateInfo(0);
            // Debug.Log($"HandAnimationController Debug: StateHash={st.shortNameHash} normalizedTime={st.normalizedTime:F2} IsIdle={st.shortNameHash==idleStateHash || st.IsName(idleStateName)} IsCollect={(collectStateHash!=0 && (st.shortNameHash==collectStateHash || st.IsName(collectStateName)))}");
            _nextDebugTime = Time.time + 1f;
        }
    }

    private bool HasTriggerParameter(string name)
    {
        if (handsAnimator == null || string.IsNullOrEmpty(name)) return false;
        foreach (var p in handsAnimator.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == name)
                return true;
        }
        return false;
    }
}
