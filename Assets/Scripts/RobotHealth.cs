using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Gère la santé du robot et déclenche la séquence de fin quand il meurt.
/// Attaché au GameObject robot (celui qui remplace le monstre).
/// </summary>
public class RobotHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Points de vie du robot. 1 = meurt en 1 coup.")]
    public int maxHealth = 1;

    [Tooltip("Santé actuelle (pour debug/visualisation).")]
    public int currentHealth;

    [Header("Death Settings")]
    [Tooltip("Phrase que le robot dit avant de mourir.")]
    [TextArea(2, 4)]
    public string dyingWords = "Non... pas comme ça... Je ne faisais que... corriger vos TAs...";

    [Tooltip("Délai avant de lancer la séquence de fin après la mort (secondes).")]
    public float deathDelay = 1f;

    [Header("Events")]
    [Tooltip("Événement déclenché quand le robot prend des dégâts.")]
    public UnityEvent<int> onDamageTaken;

    [Tooltip("Événement déclenché quand le robot meurt (déclenche l'ending).")]
    public UnityEvent onDeath;

    [Header("Debug")]
    [Tooltip("Afficher les logs de dégâts/mort.")]
    public bool debugMode = false;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (debugMode)
        {
            Debug.Log($"RobotHealth: Robot initialized with {currentHealth} HP.");
        }
    }

    /// <summary>
    /// Inflige des dégâts au robot.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (debugMode)
        {
            Debug.Log($"RobotHealth: Robot took {damage} damage. Health: {currentHealth}/{maxHealth}");
        }

        onDamageTaken?.Invoke(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (debugMode)
        {
            Debug.Log("RobotHealth: Robot is dying... Triggering ending sequence.");
        }

        // Disable movement/AI if any
        DisableRobotBehaviors();

        // Trigger death event (will start ending sequence)
        onDeath?.Invoke();

        // Find and trigger ending sequence
        EndingSequence ending = FindFirstObjectByType<EndingSequence>();
        if (ending != null)
        {
            ending.StartEnding(dyingWords);
        }
        else
        {
            // Create ending canvas automatically if it doesn't exist
            Debug.LogWarning("RobotHealth: No EndingSequence found in scene! Creating one automatically...");
            ending = CreateEndingCanvas();
            if (ending != null)
            {
                ending.StartEnding(dyingWords);
            }
            else
            {
                Debug.LogError("RobotHealth: Failed to create EndingSequence!");
            }
        }
    }

    /// <summary>
    /// Crée automatiquement un Canvas d'ending avec tous les composants nécessaires.
    /// </summary>
    private EndingSequence CreateEndingCanvas()
    {
        // Create Canvas GameObject
        GameObject canvasObj = new GameObject("EndingCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Create Black Overlay
        GameObject blackOverlayObj = new GameObject("BlackOverlay");
        blackOverlayObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image blackImage = blackOverlayObj.AddComponent<UnityEngine.UI.Image>();
        blackImage.color = new Color(0, 0, 0, 0);
        blackImage.raycastTarget = true;
        RectTransform blackRect = blackOverlayObj.GetComponent<RectTransform>();
        blackRect.anchorMin = Vector2.zero;
        blackRect.anchorMax = Vector2.one;
        blackRect.sizeDelta = Vector2.zero;
        blackOverlayObj.SetActive(true); // Ensure it's active

        // Create Robot Dialogue Text (TRÈS GRANDE taille, centré)
        GameObject dialogueObj = new GameObject("RobotDialogueText");
        dialogueObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Text dialogueText = dialogueObj.AddComponent<UnityEngine.UI.Text>();
        dialogueText.text = "";
        dialogueText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        dialogueText.fontSize = 144; // ÉNORME: 36 -> 144 (x4 de la taille originale)
        dialogueText.alignment = TextAnchor.MiddleCenter;
        dialogueText.color = Color.white;
        dialogueText.raycastTarget = false;
        dialogueText.fontStyle = FontStyle.Bold;
        dialogueText.resizeTextForBestFit = true;
        dialogueText.resizeTextMinSize = 80;
        dialogueText.resizeTextMaxSize = 144;
        RectTransform dialogueRect = dialogueObj.GetComponent<RectTransform>();
        dialogueRect.anchorMin = new Vector2(0.05f, 0.3f);
        dialogueRect.anchorMax = new Vector2(0.95f, 0.7f);
        dialogueRect.sizeDelta = Vector2.zero;
        dialogueObj.SetActive(true); // Ensure it's active

        // Create Ending Message Text (TRÈS GRANDE taille, centré au milieu)
        GameObject endingMsgObj = new GameObject("EndingMessageText");
        endingMsgObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Text endingMsgText = endingMsgObj.AddComponent<UnityEngine.UI.Text>();
        endingMsgText.text = "";
        endingMsgText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        endingMsgText.fontSize = 112; // ÉNORME: 28 -> 112 (x4 de la taille originale)
        endingMsgText.alignment = TextAnchor.MiddleCenter; // CENTRÉ au milieu
        endingMsgText.color = Color.white;
        endingMsgText.raycastTarget = false;
        endingMsgText.fontStyle = FontStyle.Bold;
        endingMsgText.resizeTextForBestFit = false; // Garde la taille maximale
        RectTransform endingRect = endingMsgObj.GetComponent<RectTransform>();
        endingRect.anchorMin = new Vector2(0.02f, 0.02f); // Presque plein écran
        endingRect.anchorMax = new Vector2(0.98f, 0.98f); // Presque plein écran
        endingRect.sizeDelta = Vector2.zero;
        endingMsgObj.SetActive(true); // Ensure it's active

        // Add EndingSequence component and configure it
        EndingSequence ending = canvasObj.AddComponent<EndingSequence>();
        ending.blackOverlay = blackImage;
        ending.robotDialogueText = dialogueText;
        ending.endingMessageText = endingMsgText;
        ending.debugMode = true; // Always enable debug for auto-created canvas

        Debug.Log("RobotHealth: EndingCanvas created successfully with all UI components!");

        return ending;
    }

    void DisableRobotBehaviors()
    {
        // Disable any AI/movement components
        var ai = GetComponent<MonsterAI>();
        if (ai != null) ai.enabled = false;

        var navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.isStopped = true;
            navAgent.enabled = false;
        }

        // Optional: play death animation
        var animator = GetComponent<Animator>();
        if (animator != null && animator.isActiveAndEnabled)
        {
            // If you have a "Death" trigger in the animator
            if (HasParameter(animator, "Death", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("Death");
            }
        }
    }

    bool HasParameter(Animator anim, string paramName, AnimatorControllerParameterType paramType)
    {
        if (anim == null || string.IsNullOrEmpty(paramName))
            return false;

        foreach (var param in anim.parameters)
        {
            if (param.name == paramName && param.type == paramType)
                return true;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        // Visual feedback in scene view
        if (isDead)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Test: Kill Robot (Play Mode)")]
    private void TestKillRobot()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("RobotHealth: This test only works in Play mode.");
            return;
        }

        TakeDamage(9999);
    }
#endif
}
