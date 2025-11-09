using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inventaire du joueur pour stocker les objets ramassés
/// À attacher sur le GameObject Player
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [Tooltip("Liste des objets dans l'inventaire")]
    public List<InventoryItem> items = new List<InventoryItem>();

    [Header("Hand Transform (Optional)")]
    [Tooltip("Transform de la main pour afficher les objets")]
    public Transform handTransform;

    private GameObject currentItemInHand;

    void Start()
    {
        Debug.Log("✅ Inventaire du joueur initialisé");
    }

    /// <summary>
    /// Ajouter un objet à l'inventaire
    /// </summary>
    public void AddItem(string itemName, ItemType itemType, GameObject prefab = null)
    {
        InventoryItem newItem = new InventoryItem
        {
            itemName = itemName,
            itemType = itemType,
            prefab = prefab,
            pickupTime = Time.time
        };

        items.Add(newItem);
        Debug.Log($"📦 Inventaire : {itemName} ajouté (Total: {items.Count} objets)");

        // Afficher dans la console tous les objets
        ShowInventory();
    }

    /// <summary>
    /// Retirer un objet de l'inventaire
    /// </summary>
    public bool RemoveItem(string itemName)
    {
        InventoryItem item = items.Find(i => i.itemName == itemName);
        if (item != null)
        {
            items.Remove(item);
            Debug.Log($"📦 Inventaire : {itemName} retiré (Total: {items.Count} objets)");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Vérifier si un objet est dans l'inventaire
    /// </summary>
    public bool HasItem(string itemName)
    {
        return items.Exists(i => i.itemName == itemName);
    }

    /// <summary>
    /// Obtenir un objet par son nom
    /// </summary>
    public InventoryItem GetItem(string itemName)
    {
        return items.Find(i => i.itemName == itemName);
    }

    /// <summary>
    /// Obtenir tous les objets d'un certain type
    /// </summary>
    public List<InventoryItem> GetItemsByType(ItemType itemType)
    {
        return items.FindAll(i => i.itemType == itemType);
    }

    /// <summary>
    /// Afficher l'inventaire dans la console (debug)
    /// </summary>
    public void ShowInventory()
    {
        if (items.Count == 0)
        {
            Debug.Log("📦 Inventaire vide");
            return;
        }

        Debug.Log($"📦 Inventaire ({items.Count} objets) :");
        foreach (var item in items)
        {
            Debug.Log($"   • {item.itemName} ({item.itemType})");
        }
    }

    /// <summary>
    /// Afficher un objet dans la main du joueur
    /// </summary>
    public void ShowItemInHand(string itemName)
    {
        InventoryItem item = GetItem(itemName);
        if (item == null || item.prefab == null)
        {
            Debug.LogWarning($"⚠️ Impossible d'afficher {itemName} dans la main");
            return;
        }

        // Supprimer l'objet actuel dans la main
        if (currentItemInHand != null)
        {
            Destroy(currentItemInHand);
        }

        // Instancier le nouvel objet
        if (handTransform != null)
        {
            currentItemInHand = Instantiate(item.prefab, handTransform);
            currentItemInHand.transform.localPosition = Vector3.zero;
            currentItemInHand.transform.localRotation = Quaternion.identity;
            Debug.Log($"👋 {itemName} affiché dans la main");
        }
    }

    /// <summary>
    /// Cacher l'objet dans la main
    /// </summary>
    public void HideItemInHand()
    {
        if (currentItemInHand != null)
        {
            Destroy(currentItemInHand);
            currentItemInHand = null;
        }
    }

    /// <summary>
    /// Vider l'inventaire
    /// </summary>
    public void ClearInventory()
    {
        items.Clear();
        HideItemInHand();
        Debug.Log("📦 Inventaire vidé");
    }
}

/// <summary>
/// Structure pour représenter un objet dans l'inventaire
/// </summary>
[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public ItemType itemType;
    public GameObject prefab;
    public float pickupTime;
}
