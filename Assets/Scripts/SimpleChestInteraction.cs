using UnityEngine;

/// <summary>
/// Script simple pour jouer une animation de coffre quand on appuie sur E
/// À attacher sur l'objet coffre qui a le composant Animation
/// </summary>
public class SimpleChestInteraction : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Nom de l'animation à jouer (ex: 'ChestAnim')")]
    public string animationName = "ChestAnim";
    
    [Tooltip("L'animation se joue-t-elle une seule fois ou en boucle ?")]
    public bool playOnce = true;

    [Header("Audio (Optionnel)")]
    public AudioClip openSound;
    
    private Animation animationComponent;
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        // Récupérer le composant Animation (pas Animator !)
        animationComponent = GetComponent<Animation>();
        
        if (animationComponent == null)
        {
            animationComponent = GetComponentInChildren<Animation>();
        }
        
        if (animationComponent == null)
        {
            Debug.LogError($"❌ COFFRE {gameObject.name} : Aucun composant Animation trouvé !");
            return;
        }
        
        // Désactiver "Play Automatically" par code
        animationComponent.playAutomatically = false;
        
        // Arrêter l'animation si elle tourne
        animationComponent.Stop();
        
        // Vérifier le collider
        Collider chestCollider = GetComponentInChildren<Collider>();
        if (chestCollider == null)
        {
            Debug.LogWarning($"⚠️ COFFRE {gameObject.name} : Aucun Collider trouvé ! Ajoutez un Box Collider.");
        }
        
        // Setup audio
        if (openSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
        
        Debug.Log($"✅ Coffre {gameObject.name} initialisé (Animation: {animationName})");
    }

    /// <summary>
    /// Jouer l'animation du coffre
    /// </summary>
    public void PlayChestAnimation()
    {
        if (animationComponent == null) return;

        // Si playOnce est activé et que l'animation a déjà été jouée, ne rien faire
        if (playOnce && hasPlayed)
        {
            Debug.Log($"📦 Coffre déjà ouvert");
            return;
        }

        // Jouer l'animation
        animationComponent.Play(animationName);
        hasPlayed = true;

        // Désactiver le collider pour permettre d'accéder au contenu
        Collider chestCollider = GetComponent<Collider>();
        if (chestCollider != null)
        {
            chestCollider.enabled = false;
            Debug.Log("📦 Collider du coffre désactivé - vous pouvez maintenant accéder au contenu !");
        }

        // Jouer le son
        if (audioSource != null && openSound != null)
        {
            audioSource.clip = openSound;
            audioSource.Play();
        }

        Debug.Log($"📦 Coffre : Animation '{animationName}' en cours");
    }

    /// <summary>
    /// Réinitialiser le coffre (fermer)
    /// </summary>
    public void ResetChest()
    {
        if (animationComponent == null) return;

        // Rembobiner l'animation au début
        animationComponent[animationName].time = 0;
        animationComponent.Sample();
        animationComponent.Stop();
        
        hasPlayed = false;
        Debug.Log($"📦 Coffre réinitialisé");
    }

    /// <summary>
    /// Toggle : Jouer ou réinitialiser
    /// </summary>
    public void ToggleChest()
    {
        if (!playOnce)
        {
            // Mode toggle : alterner entre ouvert/fermé
            if (animationComponent.IsPlaying(animationName))
            {
                ResetChest();
            }
            else
            {
                PlayChestAnimation();
            }
        }
        else
        {
            // Mode une fois : juste jouer
            PlayChestAnimation();
        }
    }

    /// <summary>
    /// Vérifie si l'animation est en cours
    /// </summary>
    public bool IsPlaying()
    {
        if (animationComponent == null) return false;
        return animationComponent.IsPlaying(animationName);
    }

    /// <summary>
    /// Vérifie si le coffre a été ouvert
    /// </summary>
    public bool IsOpen()
    {
        return hasPlayed;
    }

    // Visualisation
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
