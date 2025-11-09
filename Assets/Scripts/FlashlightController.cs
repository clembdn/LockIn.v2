using UnityEngine;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Références de la Lampe")]
    public GameObject flashlightInHand; 
    public Animator handAnimator; 
    public string holdFlashlightBoolName = "HasFlashlight"; 

    [Header("Paramètres de Ramassage")]
    public Transform playerCamera; 
    public float pickupDistance = 3f;
    public KeyCode pickupKey = KeyCode.E;

    // NOUVEAU: Couleur de la surbrillance
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 0.2f;

    [Header("Paramètres de Contrôle")]
    public KeyCode toggleKey = KeyCode.Mouse0;

    // Variables privées
    private Light lightComponent;
    private bool playerHasFlashlight = false;
    private bool isLightOn = false;

    // NOUVEAU: Variable pour mémoriser l'objet regardé
    private Renderer currentlyHighlighted; 

    void Start()
    {
        if (flashlightInHand != null)
        {
            lightComponent = flashlightInHand.GetComponentInChildren<Light>();
            flashlightInHand.SetActive(false); 
            if (lightComponent) lightComponent.enabled = false;
        }
    }

    void Update()
    {
        if (!playerHasFlashlight)
        {
            CheckForPickup();
        }
        else
        {
            HandleToggle();
        }
    }

    void CheckForPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, pickupDistance))
        {
            // Si on touche un objet avec le bon tag
            if (hit.collider.CompareTag("FlashlightPickup"))
            {
                // NOUVEAU: Logique de surbrillance
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null)
                {
                    SetHighlight(rend, true); // Allume la surbrillance
                    currentlyHighlighted = rend; // Mémorise l'objet
                }

                // Logique de ramassage (ne change pas)
                if (Input.GetKeyDown(pickupKey))
                {
                    PickUpFlashlight(hit.collider.gameObject);
                    ClearHighlight(); // Efface la surbrillance après ramassage
                }
            }
            // NOUVEAU: Si on regarde un autre objet
            else
            {
                ClearHighlight(); // Efface l'ancienne surbrillance
            }
        }
        // NOUVEAU: Si on ne regarde rien
        else
        {
            ClearHighlight(); // Efface l'ancienne surbrillance
        }
    }
    
    // NOUVEAU: Fonction pour activer/désactiver la surbrillance
    void SetHighlight(Renderer rend, bool highlight)
    {
        if (rend == null) return;
        
        Material mat = rend.material;
        if (highlight)
        {
            mat.EnableKeyword("_EMISSION"); // Active la propriété d'émission
            // Met la couleur d'émission au jaune (multiplié par 0.5 pour ne pas être aveuglant)
            mat.SetColor("_EmissionColor", highlightColor * highlightIntensity);
        }
        else
        {
            // Remet l'émission à noir (éteint)
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    // NOUVEAU: Fonction pour effacer la surbrillance actuelle
    void ClearHighlight()
    {
        if (currentlyHighlighted != null)
        {
            SetHighlight(currentlyHighlighted, false);
            currentlyHighlighted = null;
        }
    }

    void PickUpFlashlight(GameObject flashlightOnGround)
    {
        Destroy(flashlightOnGround);
        flashlightInHand.SetActive(true);
        playerHasFlashlight = true;
        if (handAnimator != null)
        {
            handAnimator.SetBool(holdFlashlightBoolName, true);
        }
    }

    void HandleToggle()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isLightOn = !isLightOn;
            if (lightComponent) lightComponent.enabled = isLightOn;
        }
    }
}