using UnityEngine;
using TMPro; // si tu utilises TextMeshPro pour le texte

public class PickupKnife : MonoBehaviour
{
    [Header("Raycast")]
    public float rayDistance = 5f;               // Distance max pour ramasser
    public LayerMask interactableLayer;          // Couches des objets interactables

    [Header("Hand")]
    public Transform handTransform;              // Empty GameObject dans la main droite
    public Vector3 knifeRotationInHand = new Vector3(90f, 0f, 0f); // rotation du couteau dans la main

    [Header("Knife Prefab")]
    public GameObject knifePrefab;               // Prefab unique du couteau

    [Header("UI")]
    public TextMeshProUGUI interactionText;      // Texte “Appuyez sur E pour ramasser”

    private Camera cam;
    private bool hasKnife = false;

    void Start()
    {
        cam = Camera.main;

        if (handTransform == null)
            Debug.LogError("HandTransform non assigné !");
        if (knifePrefab == null)
            Debug.LogError("KnifePrefab non assigné !");
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

        // Debug : ligne rouge pour visualiser le raycast
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                if (interactionText != null)
                    interactionText.text = "Appuyez sur E pour ramasser";

                if (Input.GetKeyDown(KeyCode.E) && !hasKnife)
                {
                    Debug.Log("🎯 Ramassage de : " + hit.collider.name);

                    // Supprime l’objet sur la table
                    Destroy(hit.collider.transform.root.gameObject);

                    // Instancie le couteau dans la main
                    GameObject knife = Instantiate(knifePrefab, handTransform);
                    knife.transform.localPosition = Vector3.zero;
                    knife.transform.localRotation = Quaternion.Euler(knifeRotationInHand);
                    knife.transform.localScale = (0.01f * Vector3.one); // Ajuste l’échelle si nécessaire


                    hasKnife = true;
                    Debug.Log("🔪 Couteau apparu dans la main !");
                }
            }
        }
    }
}
