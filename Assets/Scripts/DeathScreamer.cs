using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Joue un GIF animé en screamer à la mort du joueur
/// </summary>
public class DeathScreamer : MonoBehaviour
{
    [Header("Screamer Configuration")]
    [Tooltip("Frames du GIF (importe les frames comme Sprites)")]
    public Sprite[] gifFrames;
    
    [Tooltip("Son du screamer (cri horrifique)")]
    public AudioClip screamerSound;
    
    [Range(0f, 1f)]
    public float screamerVolume = 1f;
    
    [Tooltip("Durée totale du screamer")]
    public float screamerDuration = 2f;
    
    [Tooltip("Frames par seconde du GIF")]
    public int framesPerSecond = 30;
    
    private Canvas screamerCanvas;
    private Image gifImage;
    private AudioSource audioSource;
    private bool isPlaying = false;

    void Awake()
    {
        // Créer le canvas pour le screamer
        GameObject canvasObj = new GameObject("ScreamerCanvas");
        canvasObj.transform.SetParent(transform);
        
        screamerCanvas = canvasObj.AddComponent<Canvas>();
        screamerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        screamerCanvas.sortingOrder = 9999; // Au-dessus de tout
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Image pour le GIF
        GameObject imageObj = new GameObject("GifImage");
        imageObj.transform.SetParent(canvasObj.transform);
        
        gifImage = imageObj.AddComponent<Image>();
        RectTransform rect = gifImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        
        // AudioSource pour le cri
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Son 2D
        
        // Désactiver au départ
        screamerCanvas.gameObject.SetActive(false);
        
        // Charger le son si non assigné
        if (screamerSound == null)
        {
            screamerSound = Resources.Load<AudioClip>("Sounds/screamer");
        }
    }

    /// <summary>
    /// Déclenche l'effet de screamer avec le GIF
    /// </summary>
    public void TriggerScreamer(System.Action onComplete = null)
    {
        if (!isPlaying)
        {
            StartCoroutine(PlayGifScreamer(onComplete));
        }
    }

    private IEnumerator PlayGifScreamer(System.Action onComplete)
    {
        if (gifFrames == null || gifFrames.Length == 0)
        {
            Debug.LogWarning("DeathScreamer: Aucune frame de GIF assignée!");
            onComplete?.Invoke();
            yield break;
        }
        
        isPlaying = true;
        
        // Activer le canvas
        screamerCanvas.gameObject.SetActive(true);
        
        // Jouer le son
        if (screamerSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(screamerSound, screamerVolume);
        }
        
        // Calculer le délai entre chaque frame
        float frameDelay = 1f / framesPerSecond;
        float elapsed = 0f;
        int currentFrame = 0;
        
        // Jouer l'animation du GIF
        while (elapsed < screamerDuration)
        {
            // Afficher la frame actuelle
            gifImage.sprite = gifFrames[currentFrame];
            
            // Passer à la frame suivante
            currentFrame = (currentFrame + 1) % gifFrames.Length;
            
            // Attendre
            yield return new WaitForSeconds(frameDelay);
            elapsed += frameDelay;
        }
        
        // Fade out rapide
        float fadeTime = 0.2f;
        float fadeElapsed = 0f;
        
        while (fadeElapsed < fadeTime)
        {
            fadeElapsed += Time.deltaTime;
            float alpha = 1f - (fadeElapsed / fadeTime);
            
            Color color = gifImage.color;
            color.a = alpha;
            gifImage.color = color;
            
            yield return null;
        }
        
        // Désactiver
        screamerCanvas.gameObject.SetActive(false);
        gifImage.color = Color.white; // Reset alpha
        isPlaying = false;
        
        // Callback
        onComplete?.Invoke();
    }
}
