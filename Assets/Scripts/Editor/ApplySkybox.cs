using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

/// <summary>
/// Applique la skybox Cartoon Base NightSky à la scène active
/// Menu: LockIn > Apply NightSky Skybox
/// </summary>
public class ApplySkybox : EditorWindow
{
    [MenuItem("LockIn/Apply NightSky Skybox NOW!")]
    public static void ApplyNightSky()
    {
        Debug.Log("=== APPLICATION DE LA SKYBOX ===\n");

        // Trouver le matériau de la skybox
        string[] guids = AssetDatabase.FindAssets("Cartoon Base NightSky t:Material", new[] { "Assets/AllSkyFree/Cartoon Base NightSky" });
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Matériau 'Cartoon Base NightSky' non trouvé!\n\n" +
                "Vérifiez que le dossier AllSkyFree/Cartoon Base NightSky existe.",
                "OK"
            );
            return;
        }

        Material skyboxMaterial = null;
        
        // Chercher le bon matériau (pas l'Equirect)
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            
            // Prendre le matériau "Cartoon Base NightSky.mat" (pas Equirect)
            if (fileName == "Cartoon Base NightSky")
            {
                skyboxMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
                Debug.Log($"✅ Matériau trouvé: {path}");
                break;
            }
        }

        if (skyboxMaterial == null)
        {
            EditorUtility.DisplayDialog(
                "Erreur",
                "Impossible de charger le matériau de la skybox!",
                "OK"
            );
            return;
        }

        // Appliquer la skybox aux Render Settings
        RenderSettings.skybox = skyboxMaterial;
        Debug.Log("✅ Skybox appliquée aux Render Settings");

        // Activer l'ambient lighting de la skybox
        RenderSettings.ambientMode = AmbientMode.Skybox;
        Debug.Log("✅ Ambient Mode défini sur Skybox");

        // Ajuster l'intensité de l'ambient
        RenderSettings.ambientIntensity = 1f;
        Debug.Log("✅ Ambient Intensity = 1.0");

        // Forcer la mise à jour de la skybox
        DynamicGI.UpdateEnvironment();
        Debug.Log("✅ Environnement mis à jour");

        // Sauvegarder la scène
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );

        Debug.Log("\n═══════════════════════════════════════");
        Debug.Log("✅ SKYBOX APPLIQUÉE AVEC SUCCÈS!");
        Debug.Log("═══════════════════════════════════════\n");

        EditorUtility.DisplayDialog(
            "✅ Succès!",
            "La skybox 'Cartoon Base NightSky' a été appliquée!\n\n" +
            "La scène devrait maintenant avoir un ciel nocturne étoilé.\n\n" +
            "Regardez vers le haut dans la Scene View pour la voir! 🌙✨",
            "Super!"
        );
    }

    [MenuItem("LockIn/Remove Skybox")]
    public static void RemoveSkybox()
    {
        RenderSettings.skybox = null;
        RenderSettings.ambientMode = AmbientMode.Trilight;
        DynamicGI.UpdateEnvironment();
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );

        EditorUtility.DisplayDialog(
            "Skybox supprimée",
            "La skybox a été retirée de la scène.",
            "OK"
        );
        
        Debug.Log("Skybox supprimée");
    }
}
