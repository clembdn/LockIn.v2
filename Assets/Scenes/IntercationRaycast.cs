using UnityEngine;
using TMPro; // si tu utilises TextMeshPro

public class InteractionRaycast : MonoBehaviour
{
    public float rayDistance = 10f; // distance de détection
    public LayerMask interactableLayer; // couche pour les objets interactifs

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Debug.Log("Update tourne");
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(cam.transform.position, cam.transform.forward * rayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
        {
            Debug.Log("Ray touche : " + hit.collider.name);

            if (hit.collider.CompareTag("Interactable"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Objet ramassé !");
                    Destroy(hit.collider.gameObject);
                }
            }
        }
        else
        {
            // interactionText.text = "";
        }
    }
}

 