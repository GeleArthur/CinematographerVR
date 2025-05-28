#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class TextureArrayCreatorEditor : EditorWindow
{
    Texture2D[] textures = new Texture2D[1];
    TextureFormat textureFormat = TextureFormat.RGBA32;
    int textureArraySize = 1024;

    [MenuItem("Tools/Texture2DArray Creator")]
    public static void ShowWindow()
    {
        GetWindow<TextureArrayCreatorEditor>("Texture2DArray Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Create Texture2DArray Asset", EditorStyles.boldLabel);
        textureArraySize = EditorGUILayout.IntField("Texture Size", textureArraySize);
        textureFormat = (TextureFormat)EditorGUILayout.EnumPopup("Texture Format", textureFormat);

        GUILayout.Label("Textures", EditorStyles.boldLabel);
        int newLength = Mathf.Max(1, EditorGUILayout.IntField("Array Size", textures.Length));
        if (newLength != textures.Length)
        {
            System.Array.Resize(ref textures, newLength);
        }
        for (int i = 0; i < textures.Length; i++)
        {
            textures[i] = (Texture2D)EditorGUILayout.ObjectField($"Texture {i}", textures[i], typeof(Texture2D), false);
        }

        if (GUILayout.Button("Create Texture2DArray"))
        {
            CreateTextureArray();
        }
    }

    void CreateTextureArray()
    {
        if (textures == null || textures.Length == 0)
        {
            Debug.LogError("No textures provided to create a texture array.");
            return;
        }
        Texture2DArray textureArray = new Texture2DArray(textureArraySize, textureArraySize, textures.Length, textureFormat, false);
        for (int i = 0; i < textures.Length; i++)
        {
            Graphics.CopyTexture(textures[i], 0, 0, textureArray, i, 0);
        }
        string path = EditorUtility.SaveFilePanelInProject("Save Texture2DArray", "Texture2DArray", "asset", "Specify where to save the texture array.");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(textureArray, path);
            AssetDatabase.SaveAssets();
            Debug.Log("Texture array created and saved at: " + path);
        }
    }
}
#endif

