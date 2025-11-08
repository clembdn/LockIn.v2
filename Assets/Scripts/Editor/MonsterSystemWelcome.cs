using UnityEngine;
using UnityEditor;

/// <summary>
/// Message de bienvenue et aide pour le système de monstre
/// Apparaît au démarrage de Unity
/// </summary>
[InitializeOnLoad]
public class MonsterSystemWelcome
{
    private const string PREFS_KEY = "MonsterSystem_WelcomeShown";
    
    static MonsterSystemWelcome()
    {
        EditorApplication.delayCall += ShowWelcomeMessage;
    }

    private static void ShowWelcomeMessage()
    {
        // Vérifier si le message a déjà été montré
        if (EditorPrefs.GetBool(PREFS_KEY, false))
        {
            return;
        }

        // Vérifier si le système de monstre est installé
        string[] monsterScripts = AssetDatabase.FindAssets("MonsterAI t:Script");
        if (monsterScripts.Length == 0)
        {
            return; // Scripts pas encore créés
        }

        // Afficher le message de bienvenue
        bool setup = EditorUtility.DisplayDialog(
            "🎮 Système de Monstre LockIn",
            "Bienvenue dans le système de monstre MonsterMutant7!\n\n" +
            "Voulez-vous configurer le monstre maintenant?\n\n" +
            "Cela va:\n" +
            "• Chercher le prefab du monstre\n" +
            "• L'ajouter à votre scène\n" +
            "• Tout configurer automatiquement\n\n" +
            "Vous pouvez aussi le faire plus tard via:\n" +
            "Menu LockIn > Complete Setup: Monster + Spawner",
            "Configurer maintenant",
            "Plus tard"
        );

        // Marquer comme montré
        EditorPrefs.SetBool(PREFS_KEY, true);

        if (setup)
        {
            DirectMonsterAdder.CompleteSetup();
        }
        else
        {
            Debug.Log("💡 Astuce: Utilisez le menu LockIn pour configurer le monstre quand vous êtes prêt!");
        }
    }

    [MenuItem("LockIn/Show Welcome Message Again")]
    public static void ResetWelcome()
    {
        EditorPrefs.DeleteKey(PREFS_KEY);
        Debug.Log("Message de bienvenue réinitialisé. Il apparaîtra au prochain démarrage de Unity.");
        
        bool showNow = EditorUtility.DisplayDialog(
            "Message réinitialisé",
            "Le message de bienvenue a été réinitialisé.\n\nVoulez-vous le voir maintenant?",
            "Oui",
            "Non"
        );

        if (showNow)
        {
            ShowWelcomeMessage();
        }
    }

    [MenuItem("LockIn/Help/Quick Start Guide")]
    public static void OpenQuickStartGuide()
    {
        string guidePath = "Assets/../AJOUTER_MONSTRE_SCENE.md";
        
        if (System.IO.File.Exists(guidePath))
        {
            Application.OpenURL("file://" + System.IO.Path.GetFullPath(guidePath));
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Guide de démarrage rapide",
                "📘 AJOUTER_MONSTRE_SCENE.md\n\n" +
                "Méthode la plus simple:\n" +
                "Menu: LockIn > Complete Setup: Monster + Spawner\n\n" +
                "Méthode manuelle:\n" +
                "1. GameObject > LockIn > Add Monster Here\n" +
                "2. Ajoutez QuickMonsterSetup\n" +
                "3. Configurez et créez le prefab\n" +
                "4. Ajoutez MonsterSpawner\n" +
                "5. Play!\n\n" +
                "Le fichier de guide complet se trouve à la racine du projet.",
                "OK"
            );
        }
    }

    [MenuItem("LockIn/Help/View All Documentation")]
    public static void ShowAllDocs()
    {
        EditorUtility.DisplayDialog(
            "📚 Documentation du système de monstre",
            "Fichiers de documentation disponibles:\n\n" +
            "📘 AJOUTER_MONSTRE_SCENE.md\n" +
            "   → Guide rapide pour ajouter le monstre\n\n" +
            "📗 GUIDE_RAPIDE_MONSTRE.md\n" +
            "   → Configuration détaillée en 5 minutes\n\n" +
            "📙 MONSTER_SETUP.md\n" +
            "   → Documentation complète pas à pas\n\n" +
            "📄 FICHIERS_CREES.md\n" +
            "   → Vue d'ensemble du système\n\n" +
            "Tous les fichiers sont à la racine du projet.",
            "OK"
        );
    }

    [MenuItem("LockIn/Help/Troubleshooting")]
    public static void ShowTroubleshooting()
    {
        EditorUtility.DisplayDialog(
            "🔧 Dépannage rapide",
            "Problèmes courants:\n\n" +
            "❌ Prefab non trouvé\n" +
            "→ Vérifiez que Assets/MonsterMutant 7/ existe\n\n" +
            "❌ Joueur non trouvé\n" +
            "→ Ajoutez le tag 'Player' à votre joueur\n\n" +
            "❌ Le monstre ne bouge pas\n" +
            "→ Vérifiez l'Animator Controller\n" +
            "→ Vérifiez que MonsterAI est présent\n\n" +
            "❌ Animations ne jouent pas\n" +
            "→ Ouvrez l'Animator window\n" +
            "→ Vérifiez les paramètres (Speed, IsRunning)\n\n" +
            "Pour plus d'aide, consultez:\n" +
            "GUIDE_RAPIDE_MONSTRE.md\n" +
            "Section '🐛 Problèmes courants'",
            "OK"
        );
    }

    [MenuItem("LockIn/Help/About Monster System")]
    public static void ShowAbout()
    {
        EditorUtility.DisplayDialog(
            "À propos du système de monstre",
            "🎮 Système de Monstre LockIn\n" +
            "Version 1.0\n\n" +
            "Créé pour le projet LockIn\n" +
            "Utilise MonsterMutant7 assets\n\n" +
            "Fonctionnalités:\n" +
            "✓ IA de poursuite du joueur\n" +
            "✓ Animations (idle, course)\n" +
            "✓ Système de spawn\n" +
            "✓ Configuration automatique\n" +
            "✓ Outils de test et debug\n\n" +
            "5 scripts C# créés\n" +
            "4 guides de documentation\n" +
            "3 outils d'édition Unity\n\n" +
            "Consultez FICHIERS_CREES.md pour\n" +
            "une vue d'ensemble complète.",
            "Cool!"
        );
    }
}
