using UnityEngine;

/// <summary>
/// Script à placer sur un GameObject avec un Collider (marqué Is Trigger)
/// qui représente la zone de la maison où le monstre ne peut pas poursuivre le joueur
/// </summary>
[RequireComponent(typeof(Collider))]
public class HouseSafezone : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Activer les messages de debug")]
    public bool showDebugInfo = true;
    
    [Tooltip("Couleur du gizmo dans l'éditeur")]
    public Color gizmoColor = new Color(0f, 1f, 0f, 0.3f);
    
    private void Start()
    {
        // Vérifier que le collider est bien un trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"HouseSafezone sur '{gameObject.name}': Le Collider n'est pas marqué comme Trigger! Correction automatique.");
            col.isTrigger = true;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"HouseSafezone '{gameObject.name}' activé et prêt.");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Vérifier si c'est le joueur qui entre dans la zone
        if (IsPlayer(other))
        {
            // Informer tous les monstres que le joueur est dans la maison
            MonsterAI[] monsters = FindObjectsOfType<MonsterAI>();
            foreach (MonsterAI monster in monsters)
            {
                monster.isPlayerInHouse = true;
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"HouseSafezone: Joueur entré dans la zone '{gameObject.name}' - Monstres informés ({monsters.Length})");
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Vérifier si c'est le joueur qui sort de la zone
        if (IsPlayer(other))
        {
            // Informer tous les monstres que le joueur a quitté la maison
            MonsterAI[] monsters = FindObjectsOfType<MonsterAI>();
            foreach (MonsterAI monster in monsters)
            {
                monster.isPlayerInHouse = false;
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"HouseSafezone: Joueur sorti de la zone '{gameObject.name}' - Monstres informés ({monsters.Length})");
            }
        }
    }
    
    private bool IsPlayer(Collider other)
    {
        // Vérifier par tag
        if (other.CompareTag("Player"))
            return true;
        
        // Vérifier par component FirstPersonMovement
        if (other.GetComponent<FirstPersonMovement>() != null)
            return true;
        
        // Vérifier sur le parent
        if (other.transform.parent != null)
        {
            if (other.transform.parent.CompareTag("Player"))
                return true;
            
            if (other.transform.parent.GetComponent<FirstPersonMovement>() != null)
                return true;
        }
        
        return false;
    }
    
    // Afficher la zone dans l'éditeur
    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = gizmoColor;
            
            // Dessiner selon le type de collider
            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Matrix4x4 oldMatrix = Gizmos.matrix;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
                Gizmos.matrix = oldMatrix;
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = col as SphereCollider;
                Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
            }
            else if (col is CapsuleCollider)
            {
                CapsuleCollider capsule = col as CapsuleCollider;
                Gizmos.DrawSphere(transform.position + capsule.center, capsule.radius);
            }
            else if (col is MeshCollider)
            {
                // Pour un MeshCollider, dessiner la bounding box
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
            
            // Dessiner aussi les bordures pour plus de visibilité
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
