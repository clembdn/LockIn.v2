using UnityEngine;
using TMPro;

/// <summary>
/// Script à attacher sur le joueur (caméra) pour détecter et interagir avec les portes
/// Fonctionne de manière similaire à IntercationRaycast.cs
/// </summary>
public class DoorRaycastInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Distance maximale pour interagir avec une porte")]
    public float rayDistance = 3f;
    
    [Tooltip("Couches des objets porte (configurez-le dans les Layer Settings)")]
    public LayerMask doorLayer;

    [Header("UI")]
    [Tooltip("Texte UI pour afficher les instructions d'interaction")]
    public TextMeshProUGUI interactionText;

    [Header("Input")]
    [Tooltip("Touche pour interagir avec la porte")]
    public KeyCode interactKey = KeyCode.E;

    private Camera cam;
    private DoorInteraction currentDoor;

    void Start()
    {
        cam = Camera.main;
        
        if (cam == null)
        {
            Debug.LogError("❌ Aucune caméra principale trouvée !");
        }
        
        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    void Update()
    {
        // Reset du texte et de la porte actuelle
        currentDoor = null;
        if (interactionText != null)
        {
            interactionText.text = "";
        }

        // Créer un raycast depuis la caméra
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // Debug : visualiser le raycast (ligne rouge dans la Scene view)
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.cyan);

        // Vérifier si le raycast touche une porte
        if (Physics.Raycast(ray, out hit, rayDistance, doorLayer))
        {
            // Chercher le script DoorInteraction sur l'objet touché
            DoorInteraction door = hit.collider.GetComponent<DoorInteraction>();
            
            if (door == null)
            {
                // Chercher dans le parent si pas trouvé directement
                door = hit.collider.GetComponentInParent<DoorInteraction>();
            }

            if (door != null)
            {
                currentDoor = door;
                
                // Afficher le texte approprié selon l'état de la porte
                if (interactionText != null)
                {
                    if (door.IsOpen())
                    {
                        interactionText.text = $"Appuyez sur {interactKey} pour fermer";
                    }
                    else
                    {
                        interactionText.text = $"Appuyez sur {interactKey} pour ouvrir";
                    }
                }

                // Détecter l'input pour interagir
                if (Input.GetKeyDown(interactKey))
                {
                    door.ToggleDoor();
                    Debug.Log($"🚪 Interaction avec la porte : {hit.collider.name}");
                }
            }
        }
    }

    /// <summary>
    /// Obtenir la porte actuellement visée
    /// </summary>
    public DoorInteraction GetCurrentDoor()
    {
        return currentDoor;
    }

    // Dessiner un Gizmo dans la Scene view pour visualiser la distance
    private void OnDrawGizmos()
    {
        if (cam != null && Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(cam.transform.position, cam.transform.position + cam.transform.forward * rayDistance);
        }
    }
}
