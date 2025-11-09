using UnityEngine;
using TMPro;

/// <summary>
/// Script pour détecter et interagir avec les coffres
/// À ajouter sur la Camera du joueur (en plus de DoorRaycastInteraction)
/// </summary>
public class ChestRaycastInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Distance maximale pour interagir avec un coffre")]
    public float rayDistance = 3f;
    
    [Tooltip("Couche des objets coffre")]
    public LayerMask chestLayer;

    [Header("UI")]
    [Tooltip("Texte UI pour afficher les instructions (optionnel)")]
    public TextMeshProUGUI interactionText;

    [Header("Input")]
    [Tooltip("Touche pour interagir avec le coffre")]
    public KeyCode interactKey = KeyCode.E;

    private Camera cam;
    private ChestAnimationTrigger currentChest;

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

        // Créer un raycast
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // Debug : visualiser le raycast
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.yellow);

        // Vérifier si le raycast touche un coffre
        if (Physics.Raycast(ray, out hit, rayDistance, chestLayer))
        {
            // Chercher le script ChestAnimationTrigger
            ChestAnimationTrigger chest = hit.collider.GetComponent<ChestAnimationTrigger>();
            
            if (chest == null)
            {
                chest = hit.collider.GetComponentInParent<ChestAnimationTrigger>();
            }

            if (chest != null)
            {
                currentChest = chest;
                
                // Afficher le texte (si configuré)
                if (interactionText != null)
                {
                    if (chest.IsOpen())
                    {
                        interactionText.text = $"Appuyez sur {interactKey} pour fermer le coffre";
                    }
                    else
                    {
                        interactionText.text = $"Appuyez sur {interactKey} pour ouvrir le coffre";
                    }
                }

                // Détecter l'input
                if (Input.GetKeyDown(interactKey))
                {
                    chest.ToggleChest();
                    Debug.Log($"📦 Interaction avec le coffre : {hit.collider.name}");
                }
            }
        }
        else if (interactionText != null)
        {
            // Reset le texte si on ne regarde plus un coffre
            interactionText.text = "";
        }
    }

    /// <summary>
    /// Obtenir le coffre actuellement visé
    /// </summary>
    public ChestAnimationTrigger GetCurrentChest()
    {
        return currentChest;
    }
}
