using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Script pour configurer les animations en mode loop
/// Menu: LockIn > Fix Animation Looping
/// </summary>
public class FixAnimationLooping : EditorWindow
{
    [MenuItem("LockIn/Fix Animation Looping NOW!")]
    public static void FixAnimationLoop()
    {
        Debug.Log("=== Configuration du looping des animations ===");

        string animPath = "Assets/MonsterMutant 7/Animations";
        
        // Liste des animations qui doivent boucler
        string[] loopFiles = new string[]
        {
            "MutantMonster2@idle1.fbx",
            "MutantMonster2@idle2.fbx",
            "MutantMonster2@idle3.fbx",
            "MutantMonster2@idle4.fbx",
            "MutantMonster2@walk2.fbx",
            "MutantMonster2@walk3.fbx",
            "MutantMonster2@walk4.fbx",
            "MutantMonster2@walkback.fbx",
            "MutantMonster2@run1.fbx",
            "MutantMonster2@run2.fbx",
            "MutantMonster2@run3.fbx"
        };

        int fixedCount = 0;
        int errorCount = 0;

        foreach (string fileName in loopFiles)
        {
            string path = Path.Combine(animPath, fileName);
            
            Debug.Log($"Traitement: {path}");
            
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            
            if (importer == null)
            {
                Debug.LogError($"❌ Impossible de charger: {path}");
                errorCount++;
                continue;
            }

            // Obtenir les clips par défaut
            ModelImporterClipAnimation[] clipAnimations = importer.defaultClipAnimations;
            
            if (clipAnimations.Length == 0)
            {
                Debug.LogWarning($"⚠️ Pas de clips trouvés dans: {fileName}");
                errorCount++;
                continue;
            }

            // Créer un nouveau tableau pour les clips modifiés
            ModelImporterClipAnimation[] newClips = new ModelImporterClipAnimation[clipAnimations.Length];
            
            for (int i = 0; i < clipAnimations.Length; i++)
            {
                newClips[i] = clipAnimations[i];
                newClips[i].loopTime = true;
                Debug.Log($"  ✓ Loop activé pour: {newClips[i].name}");
            }
            
            // Appliquer les nouveaux clips
            importer.clipAnimations = newClips;
            
            // Sauvegarder et réimporter
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            
            fixedCount++;
            Debug.Log($"✅ Sauvegardé: {fileName}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"=== ✅ {fixedCount} animations configurées ===");
        Debug.Log($"=== ❌ {errorCount} erreurs ===");

        if (fixedCount > 0)
        {
            EditorUtility.DisplayDialog(
                "Succès!",
                $"Configuration terminée!\n\n" +
                $"✅ {fixedCount} animations configurées en loop\n" +
                $"❌ {errorCount} erreurs\n\n" +
                "Les animations idle, walk et run vont maintenant boucler!\n\n" +
                "Testez avec Play ▶️",
                "Super!"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Aucune modification",
                $"Aucune animation n'a été modifiée.\n\n" +
                "Vérifiez la Console pour plus de détails.\n\n" +
                $"Erreurs: {errorCount}",
                "OK"
            );
        }
    }

    [MenuItem("LockIn/List All Animations")]
    public static void ListAnimations()
    {
        Debug.Log("=== Liste des animations ===");

        string animPath = "Assets/MonsterMutant 7/Animations";
        string[] allFiles = Directory.GetFiles(Path.Combine(Application.dataPath.Replace("Assets", ""), animPath), "*.fbx");
        
        foreach (string filePath in allFiles)
        {
            string assetPath = "Assets" + filePath.Replace(Application.dataPath, "").Replace("\\", "/");
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            
            Debug.Log($"\n📁 {fileName}:");
            
            // Charger tous les AnimationClips du FBX
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            
            foreach (Object asset in assets)
            {
                if (asset is AnimationClip)
                {
                    AnimationClip clip = asset as AnimationClip;
                    bool isLooping = clip.isLooping;
                    
                    Debug.Log($"  🎬 {clip.name}");
                    Debug.Log($"     Length: {clip.length:F2}s");
                    Debug.Log($"     Looping: {(isLooping ? "✅ OUI" : "❌ NON")}");
                }
            }
        }

        EditorUtility.DisplayDialog(
            "Liste des animations",
            "La liste complète des animations a été affichée dans la Console.\n\n" +
            "Ouvrez Window > General > Console pour voir les détails.",
            "OK"
        );
    }
}
