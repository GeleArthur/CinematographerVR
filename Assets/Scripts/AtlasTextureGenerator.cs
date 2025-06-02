using System;
using UnityEngine;

[ExecuteInEditMode]
public class AtlasTextureGenerator : MonoBehaviour
{
    [SerializeField] private Material[] _materials = Array.Empty<Material>();
    [SerializeField] private Texture[] _textures = Array.Empty<Texture>();
    private Texture2DArray _textureArray = null;

    private void OnEnable()
    {
        if(_textures.Length <= 0) return;
        if(_textureArray == null) return;
        _textureArray = new Texture2DArray(_textures[0].width, _textures[0].height, _textures.Length, TextureFormat.BC7, false);
        for (int i = 0; i < _textures.Length; i++)
        {
            Graphics.CopyTexture(_textures[i], 0, 0, _textureArray, i, 0);
        }

        foreach (Material material in _materials)
        {
            material.SetTexture("_MainTex", _textureArray);
        }
    }
    
    

 
}
