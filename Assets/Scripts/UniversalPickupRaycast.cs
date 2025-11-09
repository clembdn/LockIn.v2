using UnityEngine;
using TMPro;

/// <summary>
/// Script universel pour ramasser tous types d'objets
/// Remplace/complète IntercationRaycast.cs
/// À attacher sur la Camera du joueur
/// </summary>
public class UniversalPickupRaycast : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 5f;
    public LayerMask interactableLayer;

    [Header("UI")]
    public TextMeshProUGUI interactionText;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    private Camera cam;
    private PlayerInventory inventory;

    void Start()
    {
        cam = Camera.main;
        
        // Trouver l'inventaire du joueur
        inventory = GetComponentInParent<PlayerInventory>();
        if (inventory == null)
        {
            inventory = FindObjectOfType<PlayerInventory>();
        }
        
        if (inventory == null)
        {
            Debug.LogWarning("⚠️ Aucun PlayerInventory trouvé ! Ajoutez le script PlayerInventory sur le Player.");
        }

        if (interactionText != null)
            interactionText.text = "";
    }

    void Update()
    {
        // Reset texte
        if (interactionText != null)
            interactionText.text = "";

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // Debug
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.cyan);

        if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
        {
            // Vérifier si c'est un objet avec le script PickupItem
            PickupItem pickupItem = hit.collider.GetComponent<PickupItem>();
            if (pickupItem == null)
            {
                pickupItem = hit.collider.GetComponentInParent<PickupItem>();
            }

            if (pickupItem != null && !pickupItem.IsPickedUp())
            {
                // Afficher le texte
                if (interactionText != null)
                {
                    interactionText.text = $"Appuyez sur {interactKey} pour ramasser {pickupItem.itemName}";
                }

                // Ramasser avec E
                if (Input.GetKeyDown(interactKey))
                {
                    Transform handTransform = GetHandTransform();
                    pickupItem.Pickup(handTransform);
                    Debug.Log($"📚 {pickupItem.itemName} ramassé !");
                }
            }
            // Ancien système pour compatibilité avec le couteau
            else if (hit.collider.CompareTag("Interactable"))
            {
                if (interactionText != null)
                {
                    interactionText.text = $"Appuyez sur {interactKey} pour ramasser";
                }

                if (Input.GetKeyDown(interactKey))
                {
                    Debug.Log($"🎯 Ramassage de : {hit.collider.name}");
                    
                    // Notifier l'inventaire si c'est un objet spécial
                    if (inventory != null)
                    {
                        inventory.AddItem(hit.collider.name, ItemType.Other);
                    }
                    
                    Destroy(hit.collider.transform.root.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Trouver le transform de la main du joueur
    /// </summary>
    private Transform GetHandTransform()
    {
        // Chercher un objet nommé "Hand" ou similaire
        Transform hand = transform.root.Find("Hand");
        if (hand == null)
        {
            hand = transform.root.GetComponentInChildren<Transform>();
        }
        return hand;
    }
}
