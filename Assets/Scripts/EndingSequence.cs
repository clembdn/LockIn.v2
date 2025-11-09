using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Gère la séquence de fin du jeu :
/// 1. Robot dit sa phrase avant de mourir
/// 2. Fade to black progressif
/// 3. Texte qui s'écrit lettre par lettre
/// 4. Boucle infinie jusqu'à appui sur Echap (ferme l'application)
/// 
/// Attach to a Canvas GameObject in the scene.
/// </summary>
public class EndingSequence : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Image noire pour le fade to black.")]
    public Image blackOverlay;

    [Tooltip("Texte pour la phrase du robot avant de mourir.")]
    public Text robotDialogueText;

    [Tooltip("Texte pour le message de fin (typing effect).")]
    public Text endingMessageText;

    [Header("Timing")]
    [Tooltip("Durée du fade to black (secondes).")]
    public float fadeDuration = 3f;

    [Tooltip("Délai avant d'afficher la phrase du robot (secondes).")]
    public float dialogueDelay = 0.5f;

    [Tooltip("Durée d'affichage de la phrase du robot avant le fade (secondes).")]
    public float dialogueDisplayTime = 4f;

    [Tooltip("Vitesse du typing effect (caractères par seconde).")]
    public float typingSpeed = 30f;

    [Tooltip("Délai avant de commencer le typing effect après le fade complet (secondes).")]
    public float typingDelay = 1f;

    [Header("Ending Message")]
    [Tooltip("Message de fin qui s'affiche lettre par lettre.")]
    [TextArea(10, 20)]
    public string endingMessage = @"Félicitations...

Vous avez surmonté vos peurs.
Vous avez affronté le danger.
Vous avez vaincu... Marvin.

Ce robot correcteur automatique de TAs qui terrorisait vos nuits blanches,
qui scrutait chaque ligne de votre code avec un œil impitoyable,
qui osait vous donner des 'Insufficient' alors que vous aviez codé avec votre âme...

Aujourd'hui, vous pouvez être fier.
Vous avez repris le contrôle.

Marvin ne corrigera plus jamais vos TAs.

(Enfin... jusqu'au prochain projet.)

🎓 FIN 🎓

Merci d'avoir joué !

Appuyez sur ECHAP pour retourner à la réalité...";

    [Header("Audio")]
    [Tooltip("Musique/ambiance de fin (optionnel).")]
    public AudioClip endingMusic;

    [Tooltip("Son du typing effect (optionnel, tick par lettre).")]
    public AudioClip typingSound;

    [Header("Debug")]
    [Tooltip("Afficher les logs de progression.")]
    public bool debugMode = false;

    private AudioSource audioSource;
    private bool endingStarted = false;
    private bool endingComplete = false;
    private Canvas canvas;

    void Awake()
    {
        // Get canvas
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // On top of everything
        }

        // Ensure CanvasScaler
        if (GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Hide UI at start
        if (blackOverlay != null)
        {
            Color c = blackOverlay.color;
            c.a = 0f;
            blackOverlay.color = c;
            blackOverlay.gameObject.SetActive(false);
        }

        if (robotDialogueText != null)
        {
            robotDialogueText.text = "";
            robotDialogueText.gameObject.SetActive(false);
        }

        if (endingMessageText != null)
        {
            endingMessageText.text = "";
            endingMessageText.gameObject.SetActive(false);
        }

        // Hide canvas initially
        canvas.enabled = false;
    }

    void Update()
    {
        // Check for Escape key to quit
        if (endingComplete)
        {
            bool escapePressed = false;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
                escapePressed = true;
#else
            if (Input.GetKeyDown(KeyCode.Escape))
                escapePressed = true;
#endif

            if (escapePressed)
            {
                QuitGame();
            }
        }
    }

    /// <summary>
    /// Démarre la séquence de fin avec la phrase du robot.
    /// </summary>
    public void StartEnding(string robotFinalWords)
    {
        if (endingStarted)
            return;

        endingStarted = true;
        canvas.enabled = true;

        if (debugMode)
        {
            Debug.Log("EndingSequence: Starting ending sequence...");
        }

        // Disable player controls
        DisablePlayerControls();

        StartCoroutine(EndingSequenceCoroutine(robotFinalWords));
    }

    IEnumerator EndingSequenceCoroutine(string robotFinalWords)
    {
        // Step 1: Show robot dialogue
        if (robotDialogueText != null && !string.IsNullOrEmpty(robotFinalWords))
        {
            yield return new WaitForSeconds(dialogueDelay);

            robotDialogueText.gameObject.SetActive(true);
            robotDialogueText.text = robotFinalWords;

            if (debugMode)
            {
                Debug.Log($"EndingSequence: Robot says: '{robotFinalWords}'");
            }

            yield return new WaitForSeconds(dialogueDisplayTime);
        }

        // Step 2: Fade to black
        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(true);
            yield return StartCoroutine(FadeToBlack());
        }

        // Hide robot dialogue
        if (robotDialogueText != null)
        {
            robotDialogueText.gameObject.SetActive(false);
        }

        // Step 3: Play ending music
        if (endingMusic != null && audioSource != null)
        {
            audioSource.clip = endingMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        // Step 4: Wait before typing
        yield return new WaitForSeconds(typingDelay);

        // Step 5: Type ending message
        if (endingMessageText != null)
        {
            endingMessageText.gameObject.SetActive(true);
            yield return StartCoroutine(TypeText(endingMessage));
        }

        // Step 6: Mark ending as complete (enable quit)
        endingComplete = true;

        if (debugMode)
        {
            Debug.Log("EndingSequence: Ending complete. Press ESC to quit.");
        }
    }

    IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        Color startColor = blackOverlay.color;
        Color endColor = new Color(0, 0, 0, 1);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            blackOverlay.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        blackOverlay.color = endColor;

        if (debugMode)
        {
            Debug.Log("EndingSequence: Fade to black complete.");
        }
    }

    IEnumerator TypeText(string message)
    {
        endingMessageText.text = "";
        
        foreach (char c in message)
        {
            endingMessageText.text += c;

            // Play typing sound
            if (typingSound != null && audioSource != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(typingSound, 0.1f);
            }

            yield return new WaitForSeconds(1f / typingSpeed);
        }

        if (debugMode)
        {
            Debug.Log("EndingSequence: Typing complete.");
        }
    }

    void DisablePlayerControls()
    {
        // Disable FirstPersonMovement
        FirstPersonMovement fpm = FindFirstObjectByType<FirstPersonMovement>();
        if (fpm != null)
        {
            fpm.enabled = false;
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable knife
        KnifeWeapon knife = FindFirstObjectByType<KnifeWeapon>();
        if (knife != null)
        {
            knife.enabled = false;
        }

        if (debugMode)
        {
            Debug.Log("EndingSequence: Player controls disabled.");
        }
    }

    void QuitGame()
    {
        if (debugMode)
        {
            Debug.Log("EndingSequence: Quitting game...");
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

#if UNITY_EDITOR
    [ContextMenu("Test: Start Ending (Play Mode)")]
    private void TestEnding()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("EndingSequence: This test only works in Play mode.");
            return;
        }

        StartEnding("Non... pas comme ça... Je ne faisais que... corriger vos TAs...");
    }
#endif
}
