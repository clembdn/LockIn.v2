using UnityEngine;

/// <summary>
/// Script pour spawner le monstre à une position spécifique par rapport au joueur
/// </summary>
public class MonsterSpawner : MonoBehaviour
{
    [Header("Préfab du monstre")]
    [Tooltip("Le prefab du MonsterMutant7 à spawner")]
    public GameObject monsterPrefab;
    
    [Header("Position de spawn")]
    [Tooltip("Décalage par rapport au joueur (x=droite, y=haut, z=avant)")]
    public Vector3 spawnOffset = new Vector3(5f, 0f, 0f);
    
    [Tooltip("Le joueur (trouvé automatiquement si non assigné)")]
    public Transform player;
    
    [Header("Options")]
    [Tooltip("Spawner automatiquement au démarrage")]
    public bool spawnOnStart = true;
    
    private GameObject spawnedMonster;

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnMonster();
        }
    }

    public void SpawnMonster()
    {
        // Trouver le joueur si non assigné
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                FirstPersonMovement fpm = FindFirstObjectByType<FirstPersonMovement>();
                if (fpm != null)
                {
                    player = fpm.transform;
                }
                else
                {
                    Debug.LogError("MonsterSpawner: Impossible de trouver le joueur!");
                    return;
                }
            }
        }

        // Vérifier que nous avons un prefab
        if (monsterPrefab == null)
        {
            Debug.LogError("MonsterSpawner: Aucun prefab de monstre assigné!");
            return;
        }

        // Calculer la position de spawn
        Vector3 spawnPosition = player.position + player.TransformDirection(spawnOffset);
        
        // Spawner le monstre
        spawnedMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        
        // S'assurer que le monstre a le script MonsterAI
        MonsterAI monsterAI = spawnedMonster.GetComponent<MonsterAI>();
        if (monsterAI == null)
        {
            monsterAI = spawnedMonster.AddComponent<MonsterAI>();
        }
        
        // Assigner le joueur au monstre
        monsterAI.player = player;
        
        Debug.Log($"MonsterSpawner: Monstre spawné à la position {spawnPosition}");
    }

    // Pour spawner depuis l'éditeur ou via code
    [ContextMenu("Spawn Monster Now")]
    public void SpawnMonsterNow()
    {
        if (Application.isPlaying)
        {
            SpawnMonster();
        }
        else
        {
            Debug.LogWarning("MonsterSpawner: Cette fonction ne peut être utilisée qu'en mode Play!");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player == null)
        {
            FirstPersonMovement fpm = FindFirstObjectByType<FirstPersonMovement>();
            if (fpm != null)
            {
                player = fpm.transform;
            }
        }

        if (player != null)
        {
            Vector3 spawnPosition = player.position + player.TransformDirection(spawnOffset);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPosition, 0.5f);
            Gizmos.DrawLine(player.position, spawnPosition);
        }
    }
}
