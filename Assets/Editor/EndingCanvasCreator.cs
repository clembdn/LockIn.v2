using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to create the Ending UI Canvas in the scene.
/// Run via Tools > Create Ending Canvas menu.
/// </summary>
public class EndingCanvasCreator
{
#if UNITY_EDITOR
    [MenuItem("Tools/Create Ending Canvas")]
    public static void CreateEndingCanvas()
    {
        // Check if already exists
        EndingSequence existing = Object.FindFirstObjectByType<EndingSequence>();
        if (existing != null)
        {
            Debug.LogWarning("EndingCanvasCreator: Ending Canvas already exists in scene!");
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        // Create Canvas
        GameObject canvasObj = new GameObject("EndingCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Add EndingSequence component
        EndingSequence endingSequence = canvasObj.AddComponent<EndingSequence>();

        // Create Black Overlay
        GameObject blackOverlayObj = new GameObject("BlackOverlay");
        blackOverlayObj.transform.SetParent(canvasObj.transform, false);
        
        RectTransform blackRT = blackOverlayObj.AddComponent<RectTransform>();
        blackRT.anchorMin = Vector2.zero;
        blackRT.anchorMax = Vector2.one;
        blackRT.sizeDelta = Vector2.zero;
        blackRT.anchoredPosition = Vector2.zero;

        Image blackImage = blackOverlayObj.AddComponent<Image>();
        blackImage.color = new Color(0, 0, 0, 0); // Start transparent
        blackImage.raycastTarget = false;

        // Create Robot Dialogue Text
        GameObject dialogueObj = new GameObject("RobotDialogue");
        dialogueObj.transform.SetParent(canvasObj.transform, false);

        RectTransform dialogueRT = dialogueObj.AddComponent<RectTransform>();
        dialogueRT.anchorMin = new Vector2(0.5f, 0.3f);
        dialogueRT.anchorMax = new Vector2(0.5f, 0.3f);
        dialogueRT.sizeDelta = new Vector2(1200, 200);
        dialogueRT.anchoredPosition = Vector2.zero;

        Text dialogueText = dialogueObj.AddComponent<Text>();
        dialogueText.text = "";
        dialogueText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        dialogueText.fontSize = 36;
        dialogueText.alignment = TextAnchor.MiddleCenter;
        dialogueText.color = Color.white;
        dialogueText.raycastTarget = false;

        // Add shadow for readability
        Shadow dialogueShadow = dialogueObj.AddComponent<Shadow>();
        dialogueShadow.effectColor = new Color(0, 0, 0, 0.8f);
        dialogueShadow.effectDistance = new Vector2(2, -2);

        // Create Ending Message Text
        GameObject endingMsgObj = new GameObject("EndingMessage");
        endingMsgObj.transform.SetParent(canvasObj.transform, false);

        RectTransform endingRT = endingMsgObj.AddComponent<RectTransform>();
        endingRT.anchorMin = new Vector2(0.5f, 0.5f);
        endingRT.anchorMax = new Vector2(0.5f, 0.5f);
        endingRT.sizeDelta = new Vector2(1400, 900);
        endingRT.anchoredPosition = Vector2.zero;

        Text endingText = endingMsgObj.AddComponent<Text>();
        endingText.text = "";
        endingText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        endingText.fontSize = 32;
        endingText.alignment = TextAnchor.MiddleCenter;
        endingText.color = Color.white;
        endingText.raycastTarget = false;
        endingText.horizontalOverflow = HorizontalWrapMode.Wrap;
        endingText.verticalOverflow = VerticalWrapMode.Overflow;

        // Add shadow
        Shadow endingShadow = endingMsgObj.AddComponent<Shadow>();
        endingShadow.effectColor = new Color(0, 0, 0, 0.9f);
        endingShadow.effectDistance = new Vector2(3, -3);

        // Assign references to EndingSequence
        endingSequence.blackOverlay = blackImage;
        endingSequence.robotDialogueText = dialogueText;
        endingSequence.endingMessageText = endingText;

        // Hide UI elements initially
        blackOverlayObj.SetActive(false);
        dialogueObj.SetActive(false);
        endingMsgObj.SetActive(false);

        // Disable canvas initially (will be enabled when ending starts)
        canvas.enabled = false;

        // Register undo
        Undo.RegisterCreatedObjectUndo(canvasObj, "Create Ending Canvas");

        // Select the created canvas
        Selection.activeGameObject = canvasObj;

        Debug.Log("EndingCanvasCreator: Ending Canvas created successfully! Configure EndingSequence component in Inspector.");
    }
#endif
}
