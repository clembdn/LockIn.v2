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
    
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 0.2f;

    [Header("Paramètres de Contrôle")]
    public KeyCode toggleKey = KeyCode.Mouse0;
    
    // NOUVEAU: Glissez votre "AudioSource" ici
    public AudioSource toggleAudioSource; 
    
    // NOUVEAU: Glissez votre son MP3 ici
    public AudioClip toggleSound; 

    // Variables privées
    private Light lightComponent;
    private bool playerHasFlashlight = false;
    private bool isLightOn = false;
    
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
            if (hit.collider.CompareTag("FlashlightPickup"))
            {
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null)
                {
                    SetHighlight(rend, true); 
                    currentlyHighlighted = rend;
                }
                
                if (Input.GetKeyDown(pickupKey))
                {
                    PickUpFlashlight(hit.collider.gameObject);
                    ClearHighlight();
                }
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }
    
    void SetHighlight(Renderer rend, bool highlight)
    {
        if (rend == null) return;
        
        Material mat = rend.material;
        if (highlight)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", highlightColor * highlightIntensity);
        }
        else
        {
            mat.SetColor("_EmissionColor", Color.black);
        }
    }
    
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

            // NOUVEAU: Joue le son de "clic"
            if (toggleAudioSource != null && toggleSound != null)
            {
                toggleAudioSource.PlayOneShot(toggleSound, 0.2f);
            }
        }
    }
}