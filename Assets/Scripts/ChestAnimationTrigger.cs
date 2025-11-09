using UnityEngine;

/// <summary>
/// Script pour déclencher l'animation d'ouverture/fermeture d'un coffre
/// À attacher sur l'objet coffre qui a l'Animator
/// </summary>
public class ChestAnimationTrigger : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Nom du paramètre bool dans l'Animator (ex: 'IsOpen')")]
    public string openParameterName = "IsOpen";
    
    [Tooltip("Ou nom du trigger (ex: 'Open', 'Close')")]
    public string openTriggerName = "Open";
    
    [Tooltip("Utiliser un bool (true) ou un trigger (false)")]
    public bool useBoolParameter = true;

    [Header("Audio (Optionnel)")]
    public AudioClip openSound;
    public AudioClip closeSound;
    
    private Animator animator;
    private AudioSource audioSource;
    private bool isOpen = false;

    void Start()
    {
        // Récupérer l'Animator
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        if (animator == null)
        {
            Debug.LogError($"❌ COFFRE {gameObject.name} : Aucun Animator trouvé !");
            return;
        }
        
        // Vérifier le collider
        Collider chestCollider = GetComponentInChildren<Collider>();
        if (chestCollider == null)
        {
            Debug.LogWarning($"⚠️ COFFRE {gameObject.name} : Aucun Collider trouvé ! Ajoutez un Box Collider.");
        }
        
        // Setup audio
        if (openSound != null || closeSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
        
        Debug.Log($"✅ Coffre {gameObject.name} initialisé avec animation");
    }

    /// <summary>
    /// Bascule l'état du coffre (ouvrir/fermer)
    /// </summary>
    public void ToggleChest()
    {
        if (animator == null) return;

        isOpen = !isOpen;

        if (useBoolParameter)
        {
            // Utiliser un paramètre bool
            animator.SetBool(openParameterName, isOpen);
            Debug.Log($"📦 Coffre : Animation {(isOpen ? "Ouverture" : "Fermeture")} - SetBool('{openParameterName}', {isOpen})");
        }
        else
        {
            // Utiliser un trigger
            animator.SetTrigger(openTriggerName);
            Debug.Log($"📦 Coffre : Animation - SetTrigger('{openTriggerName}')");
        }

        // Jouer le son
        if (audioSource != null)
        {
            if (isOpen && openSound != null)
            {
                audioSource.clip = openSound;
                audioSource.Play();
            }
            else if (!isOpen && closeSound != null)
            {
                audioSource.clip = closeSound;
                audioSource.Play();
            }
        }
    }

    /// <summary>
    /// Ouvrir le coffre
    /// </summary>
    public void OpenChest()
    {
        if (!isOpen && animator != null)
        {
            ToggleChest();
        }
    }

    /// <summary>
    /// Fermer le coffre
    /// </summary>
    public void CloseChest()
    {
        if (isOpen && animator != null)
        {
            ToggleChest();
        }
    }

    /// <summary>
    /// Vérifie si le coffre est ouvert
    /// </summary>
    public bool IsOpen()
    {
        return isOpen;
    }

    // Visualisation dans l'éditeur
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
