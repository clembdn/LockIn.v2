using UnityEngine;

/// <summary>
/// Script à attacher sur chaque objet de porte pour gérer son ouverture/fermeture
/// </summary>
public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Angle d'ouverture de la porte (en degrés, généralement 90)")]
    public float openAngle = 90f;
    
    [Tooltip("Vitesse d'ouverture/fermeture")]
    public float doorSpeed = 2f;
    
    [Tooltip("Axe de rotation (Y par défaut pour rotation verticale)")]
    public Vector3 rotationAxis = Vector3.up;
    
    [Tooltip("Direction d'ouverture (1 = vers l'extérieur, -1 = vers l'intérieur)")]
    public float openDirection = 1f;

    [Header("Audio (Optionnel)")]
    public AudioClip openSound;
    public AudioClip closeSound;
    
    private bool isOpen = false;
    private bool isMoving = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private AudioSource audioSource;

    void Start()
    {
        // Vérification : Y a-t-il un mesh visible sur cet objet ou ses enfants ?
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogWarning($"⚠️ PORTE {gameObject.name} : Aucun Mesh Renderer trouvé ! " +
                           $"Le script DoorInteraction doit être sur l'objet PARENT qui contient le mesh visible. " +
                           $"Sinon, seul le collider bougera (invisible). Voir FIX_PORTE_NE_BOUGE_PAS.md");
        }
        
        // Vérification : Y a-t-il un collider pour l'interaction ?
        Collider doorCollider = GetComponentInChildren<Collider>();
        if (doorCollider == null)
        {
            Debug.LogWarning($"⚠️ PORTE {gameObject.name} : Aucun Collider trouvé ! " +
                           $"Ajoutez un Box Collider pour pouvoir interagir avec la porte.");
        }
        
        // Sauvegarder la rotation fermée
        closedRotation = transform.localRotation;
        
        // Calculer la rotation ouverte
        openRotation = closedRotation * Quaternion.AngleAxis(openAngle * openDirection, rotationAxis);
        
        // Setup audio si des clips sont assignés
        if (openSound != null || closeSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // Son 3D
        }
        
        Debug.Log($"✅ Porte {gameObject.name} initialisée (Angle: {openAngle}°, Speed: {doorSpeed})");
    }

    void Update()
    {
        // Animation de la porte
        if (isMoving)
        {
            Quaternion targetRotation = isOpen ? openRotation : closedRotation;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * doorSpeed);
            
            // Vérifier si l'animation est terminée
            if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.5f)
            {
                transform.localRotation = targetRotation;
                isMoving = false;
            }
        }
    }

    /// <summary>
    /// Bascule l'état de la porte (ouvrir/fermer)
    /// </summary>
    public void ToggleDoor()
    {
        if (isMoving) return; // Empêcher le spam

        isOpen = !isOpen;
        isMoving = true;

        // Jouer le son approprié
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

        Debug.Log($"🚪 Porte {gameObject.name} : {(isOpen ? "Ouverture" : "Fermeture")} (Rotation: {transform.localEulerAngles.y:F1}°)");
    }

    /// <summary>
    /// Ouvrir la porte (si elle est fermée)
    /// </summary>
    public void OpenDoor()
    {
        if (!isOpen && !isMoving)
        {
            ToggleDoor();
        }
    }

    /// <summary>
    /// Fermer la porte (si elle est ouverte)
    /// </summary>
    public void CloseDoor()
    {
        if (isOpen && !isMoving)
        {
            ToggleDoor();
        }
    }

    /// <summary>
    /// Vérifie si la porte est ouverte
    /// </summary>
    public bool IsOpen()
    {
        return isOpen;
    }

    /// <summary>
    /// Vérifie si la porte est en mouvement
    /// </summary>
    public bool IsMoving()
    {
        return isMoving;
    }

    // Visualisation dans l'éditeur
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
