using UnityEngine;

/// <summary>
/// Script à attacher sur les objets ramassables (livre, clé, etc.)
/// </summary>
public class PickupItem : MonoBehaviour
{
    [Header("Item Settings")]
    [Tooltip("Nom de l'objet (affiché dans l'inventaire)")]
    public string itemName = "Livre";
    
    [Tooltip("Type d'objet")]
    public ItemType itemType = ItemType.Book;
    
    [Tooltip("L'objet disparaît quand il est ramassé ?")]
    public bool destroyOnPickup = true;
    
    [Tooltip("L'objet apparaît dans la main du joueur ?")]
    public bool showInHand = false;
    
    [Tooltip("Prefab à instancier dans la main (optionnel)")]
    public GameObject handPrefab;

    [Header("Audio")]
    public AudioClip pickupSound;

    private bool hasBeenPickedUp = false;

    void Start()
    {
        // Vérifier qu'il y a un collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning($"⚠️ {itemName} : Pas de Collider ! Ajoutez un Box Collider.");
        }
        
        // Le collider doit être un trigger OU on peut le ramasser par raycast
        // On laisse le choix
    }

    /// <summary>
    /// Ramasser l'objet
    /// </summary>
    public void Pickup(Transform handTransform = null)
    {
        if (hasBeenPickedUp) return;

        hasBeenPickedUp = true;

        // Ajouter à l'inventaire
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem(itemName, itemType, handPrefab);
            Debug.Log($"📚 {itemName} ajouté à l'inventaire !");
        }
        else
        {
            Debug.LogWarning($"⚠️ Pas d'inventaire trouvé pour stocker {itemName}");
        }

        // Jouer le son
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Afficher dans la main si demandé
        if (showInHand && handTransform != null && handPrefab != null)
        {
            GameObject itemInHand = Instantiate(handPrefab, handTransform);
            itemInHand.transform.localPosition = Vector3.zero;
            itemInHand.transform.localRotation = Quaternion.identity;
        }

        // Détruire ou désactiver l'objet
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Vérifie si l'objet a été ramassé
    /// </summary>
    public bool IsPickedUp()
    {
        return hasBeenPickedUp;
    }

    private void OnDrawGizmos()
    {
        if (!hasBeenPickedUp)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }
    }
}

/// <summary>
/// Types d'objets ramassables
/// </summary>
public enum ItemType
{
    Book,       // Livre
    Key,        // Clé
    Note,       // Note/Papier
    Tool,       // Outil
    Weapon,     // Arme
    Other       // Autre
}
