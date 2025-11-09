using UnityEngine;
using TMPro;

/// <summary>
/// Script pour détecter et interagir avec les coffres (version simple)
/// À ajouter sur la Camera du joueur
/// </summary>
public class SimpleChestRaycast : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Distance maximale pour interagir")]
    public float rayDistance = 3f;
    
    [Tooltip("Couche des objets coffre")]
    public LayerMask chestLayer;

    [Header("UI")]
    [Tooltip("Texte UI (optionnel)")]
    public TextMeshProUGUI interactionText;

    [Header("Input")]
    [Tooltip("Touche pour interagir")]
    public KeyCode interactKey = KeyCode.E;

    private Camera cam;
    private SimpleChestInteraction currentChest;

    void Start()
    {
        cam = Camera.main;
        
        if (cam == null)
        {
            Debug.LogError("❌ Aucune caméra principale trouvée !");
        }
    }

    void Update()
    {
        // Reset
        currentChest = null;

        // Raycast
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // Debug
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.yellow);

        // Détection
        if (Physics.Raycast(ray, out hit, rayDistance, chestLayer))
        {
            // Chercher le script
            SimpleChestInteraction chest = hit.collider.GetComponent<SimpleChestInteraction>();
            
            if (chest == null)
            {
                chest = hit.collider.GetComponentInParent<SimpleChestInteraction>();
            }

            if (chest != null)
            {
                currentChest = chest;
                
                // Afficher le texte
                if (interactionText != null)
                {
                    if (chest.IsOpen())
                    {
                        interactionText.text = $"Coffre déjà ouvert";
                    }
                    else
                    {
                        interactionText.text = $"Appuyez sur {interactKey} pour ouvrir";
                    }
                }

                // Input
                if (Input.GetKeyDown(interactKey))
                {
                    chest.PlayChestAnimation();
                    Debug.Log($"📦 Ouverture du coffre : {hit.collider.name}");
                }
            }
        }
        else if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    public SimpleChestInteraction GetCurrentChest()
    {
        return currentChest;
    }
}
