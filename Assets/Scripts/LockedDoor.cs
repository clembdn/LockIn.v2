using UnityEngine;

/// <summary>
/// Script pour une porte verrouillée qui nécessite une clé
/// À attacher sur la porte qui doit être verrouillée
/// </summary>
public class LockedDoor : MonoBehaviour
{
    [Header("Lock Settings")]
    [Tooltip("Nom de la clé requise dans l'inventaire")]
    public string requiredKeyName = "Clé";
    
    [Tooltip("La porte est-elle verrouillée au départ ?")]
    public bool isLocked = true;

    [Header("Messages")]
    [Tooltip("Message quand la porte est verrouillée")]
    public string lockedMessage = "🔒 Cette porte est verrouillée. Il vous faut une clé.";
    
    [Tooltip("Message quand la porte est déverrouillée")]
    public string unlockedMessage = "🔓 Vous avez déverrouillé la porte avec la clé !";
    
    [Tooltip("Retirer la clé de l'inventaire après usage ?")]
    public bool consumeKey = false;

    [Header("Audio (Optionnel)")]
    public AudioClip lockedSound;
    public AudioClip unlockSound;

    private DoorInteraction doorScript;
    private AudioSource audioSource;

    void Start()
    {
        // Récupérer le script DoorInteraction
        doorScript = GetComponent<DoorInteraction>();
        
        if (doorScript == null)
        {
            Debug.LogError($"❌ LockedDoor sur {gameObject.name} : Aucun script DoorInteraction trouvé !");
        }

        // Setup audio
        if (lockedSound != null || unlockSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }

        Debug.Log($"🔒 Porte {gameObject.name} : Verrouillée = {isLocked}");
    }

    /// <summary>
    /// Vérifie si le joueur a la clé et tente d'ouvrir la porte
    /// </summary>
    public bool TryOpenDoor()
    {
        // Si la porte n'est pas verrouillée, on peut l'ouvrir normalement
        if (!isLocked)
        {
            if (doorScript != null)
            {
                doorScript.ToggleDoor();
                return true;
            }
        }

        // Sinon, vérifier si le joueur a la clé
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        
        if (inventory == null)
        {
            Debug.LogWarning("⚠️ Aucun inventaire trouvé !");
            return false;
        }

        // Le joueur a-t-il la clé ?
        if (inventory.HasItem(requiredKeyName))
        {
            // Déverrouiller la porte
            UnlockDoor();
            
            // Retirer la clé si demandé
            if (consumeKey)
            {
                inventory.RemoveItem(requiredKeyName);
                Debug.Log($"🔑 {requiredKeyName} utilisée et retirée de l'inventaire");
            }
            
            // Ouvrir la porte
            if (doorScript != null)
            {
                doorScript.ToggleDoor();
            }
            
            return true;
        }
        else
        {
            // Pas de clé !
            ShowLockedMessage();
            PlayLockedSound();
            return false;
        }
    }

    /// <summary>
    /// Déverrouiller la porte
    /// </summary>
    public void UnlockDoor()
    {
        if (!isLocked) return;

        isLocked = false;
        Debug.Log($"🔓 {gameObject.name} : Porte déverrouillée !");
        Debug.Log(unlockedMessage);
        
        PlayUnlockSound();
    }

    /// <summary>
    /// Verrouiller la porte
    /// </summary>
    public void LockDoor()
    {
        isLocked = true;
        Debug.Log($"🔒 {gameObject.name} : Porte verrouillée");
    }

    /// <summary>
    /// Afficher le message de porte verrouillée
    /// </summary>
    private void ShowLockedMessage()
    {
        Debug.Log(lockedMessage);
        // Vous pouvez aussi afficher un message UI ici
    }

    /// <summary>
    /// Jouer le son de porte verrouillée
    /// </summary>
    private void PlayLockedSound()
    {
        if (audioSource != null && lockedSound != null)
        {
            audioSource.clip = lockedSound;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Jouer le son de déverrouillage
    /// </summary>
    private void PlayUnlockSound()
    {
        if (audioSource != null && unlockSound != null)
        {
            audioSource.clip = unlockSound;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Vérifie si la porte est verrouillée
    /// </summary>
    public bool IsLocked()
    {
        return isLocked;
    }

    // Visualisation dans l'éditeur
    private void OnDrawGizmos()
    {
        if (isLocked)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
