using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ConvertGIFToScreamer : EditorWindow
{
    private List<Texture2D> gifFrames = new List<Texture2D>();
    
    [MenuItem("LockIn/Import GIF for Screamer")]
    static void ShowWindow()
    {
        GetWindow<ConvertGIFToScreamer>("GIF Screamer");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Importer GIF Animé pour Screamer", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("Glisse tes frames de GIF ici (plusieurs images):", EditorStyles.boldLabel);
        GUILayout.Label("Les images doivent être nommées: frame_0, frame_1, frame_2, etc.");
        GUILayout.Space(10);
        
        // Zone de drop pour plusieurs textures
        Rect dropArea = GUILayoutUtility.GetRect(0f, 100f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Glisse toutes les frames du GIF ici");
        
        Event evt = Event.current;
        
        if (dropArea.Contains(evt.mousePosition))
        {
            if (evt.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                evt.Use();
            }
            else if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                
                gifFrames.Clear();
                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    if (obj is Texture2D)
                    {
                        gifFrames.Add(obj as Texture2D);
                    }
                }
                
                evt.Use();
            }
        }
        
        GUILayout.Space(10);
        GUILayout.Label($"Frames chargées: {gifFrames.Count}");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Convertir en Screamer Animé", GUILayout.Height(40)))
        {
            if (gifFrames.Count > 0)
            {
                ConvertToAnimatedSprites();
            }
            else
            {
                EditorUtility.DisplayDialog("Erreur", "Glisse d'abord les frames du GIF!", "OK");
            }
        }
        
        GUILayout.Space(20);
        GUILayout.Label("Instructions:", EditorStyles.boldLabel);
        GUILayout.Label("MÉTHODE 1 - Frames séparées:");
        GUILayout.Label("1. Exporte ton GIF en frames PNG (frame_0.png, frame_1.png, ...)");
        GUILayout.Label("2. Importe toutes les frames dans Unity");
        GUILayout.Label("3. Glisse-les toutes dans la zone ci-dessus");
        GUILayout.Label("4. Clique 'Convertir'");
        GUILayout.Space(10);
        GUILayout.Label("MÉTHODE 2 - GIF direct:");
        GUILayout.Label("1. Importe ton .gif dans Unity");
        GUILayout.Label("2. Sélectionne-le dans le Project");
        GUILayout.Label("3. Texture Type: Sprite (2D and UI)");
        GUILayout.Label("4. Sprite Mode: Multiple");
        GUILayout.Label("5. Ouvre le Sprite Editor et découpe les frames");
    }
    
    void ConvertToAnimatedSprites()
    {
        // Trier les frames par nom
        gifFrames.Sort((a, b) => string.Compare(a.name, b.name));
        
        List<Sprite> sprites = new List<Sprite>();
        
        foreach (Texture2D tex in gifFrames)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            
            if (string.IsNullOrEmpty(path)) continue;
            
            // Configurer l'import de la texture
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer == null) continue;
            
            // Configurer comme Sprite
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 2048;
            importer.filterMode = FilterMode.Point; // Pixel perfect
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            
            // Appliquer les changements
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            
            // Charger le sprite
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null)
            {
                sprites.Add(sprite);
            }
        }
        
        AssetDatabase.Refresh();
        
        if (sprites.Count == 0)
        {
            EditorUtility.DisplayDialog("Erreur", "Impossible de convertir les frames!", "OK");
            return;
        }
        
        // Trouver le joueur et son DeathScreamer
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            FirstPersonMovement fpm = FindObjectOfType<FirstPersonMovement>();
            if (fpm != null) player = fpm.gameObject;
        }
        
        if (player == null)
        {
            EditorUtility.DisplayDialog(
                "Sprites créés!",
                $"✓ {sprites.Count} frames converties en Sprites!\n\n" +
                $"⚠ Joueur non trouvé dans la scène.\n\n" +
                $"Pour l'assigner manuellement:\n" +
                $"1. Sélectionne le joueur\n" +
                $"2. Trouve 'DeathScreamer' component\n" +
                $"3. Configure 'Gif Frames' à {sprites.Count}\n" +
                $"4. Glisse toutes les frames dans l'array",
                "OK"
            );
            return;
        }
        
        // Ajouter DeathScreamer si nécessaire
        DeathScreamer screamer = player.GetComponent<DeathScreamer>();
        if (screamer == null)
        {
            screamer = player.AddComponent<DeathScreamer>();
        }
        
        // Assigner les sprites
        SerializedObject so = new SerializedObject(screamer);
        SerializedProperty framesProp = so.FindProperty("gifFrames");
        
        framesProp.arraySize = sprites.Count;
        for (int i = 0; i < sprites.Count; i++)
        {
            framesProp.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
        }
        
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(screamer);
        
        // Message de succès
        EditorUtility.DisplayDialog(
            "Succès!",
            $"✓ {sprites.Count} frames converties et assignées!\n" +
            $"✓ GIF animé prêt pour le screamer\n\n" +
            $"Lance le jeu et meurs pour voir l'animation! 💀",
            "Parfait!"
        );
        
        Debug.Log($"✓ {sprites.Count} frames de GIF assignées au screamer!");
        
        // Sélectionner le joueur
        Selection.activeGameObject = player;
    }
}
