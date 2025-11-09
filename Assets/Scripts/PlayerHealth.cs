using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Nombre de coups avant de mourir")]
    public int maxHealth = 2;
    
    [Header("Audio")]
    [Tooltip("Son joué à la mort du joueur")]
    public AudioClip deathSound;
    [Range(0f, 1f)]
    public float deathSoundVolume = 1f;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private int currentHealth;
    private AudioSource audioSource;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Charger le son
        if (deathSound == null)
        {
            deathSound = Resources.Load<AudioClip>("Sounds/playerDie");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        if (showDebugInfo)
        {
            Debug.Log($"Player hit! Health: {currentHealth}/{maxHealth}");
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log("Player died!");
        
        // Désactiver les contrôles du joueur
        FirstPersonMovement fpm = GetComponent<FirstPersonMovement>();
        if (fpm) fpm.enabled = false;
        
        // Déclencher le screamer horrifique
        DeathScreamer screamer = GetComponent<DeathScreamer>();
        if (screamer == null)
        {
            screamer = gameObject.AddComponent<DeathScreamer>();
        }
        
        // Jouer le son de mort en fond
        if (deathSound && audioSource)
        {
            audioSource.PlayOneShot(deathSound, deathSoundVolume * 0.5f);
        }
        
        // Lancer le screamer puis recharger
        screamer.TriggerScreamer(() => {
            Invoke("ReloadScene", 0.5f);
        });
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Affichage UI simple
    void OnGUI()
    {
        if (!isDead)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.red;
            
            GUI.Label(new Rect(10, 10, 200, 30), $"Health: {currentHealth}/{maxHealth}", style);
        }
    }
}
