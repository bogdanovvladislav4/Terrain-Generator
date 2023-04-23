
using UnityEngine;
using System.Linq;

namespace TerrainProceduralGenerator
{
    [CreateAssetMenu]
    public class TextureData : UpdatableData
    {
        private const int TextureSize = 512;
        private const TextureFormat TextureFormat = UnityEngine.TextureFormat.RGB565;
        public Layer[] layers;

        private float savedMinHeight;
        private float savedMaxHeight;

        public void ApplyToMaterial(Material material)
        {
            material.SetInt("layerCount", layers.Length);
            material.SetColorArray("baseColor", layers.Select(x => x.tint).ToArray());
            material.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());
            material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
            material.SetFloatArray("baseColourStrength", layers.Select(x => x.tintStrength).ToArray());
            material.SetFloatArray("baseTextureScales", layers.Select(x => x.textureScale).ToArray());

            Texture2DArray texture2DArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
            material.SetTexture("baseTextures", texture2DArray);

            UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
        }

        public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
        {
            savedMinHeight = minHeight;
            savedMaxHeight = maxHeight;
            material.SetFloat("minHeight", minHeight);
            material.SetFloat("maxHeight", maxHeight);
        }

        Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            Texture2DArray texture2DArray = new Texture2DArray(TextureSize, TextureSize, textures.Length,
                TextureFormat, true);
            for (int i = 0; i < textures.Length; i++)
            {
                texture2DArray.SetPixels(textures[i].GetPixels(), i);
            }

            texture2DArray.Apply();
            return texture2DArray;
        }

        [System.Serializable]
        public class Layer
        {
            public Texture2D texture;
            public Color tint;
            [Range(0, 1)] public float tintStrength;
            [Range(0, 1)] public float startHeight;
            [Range(0, 1)] public float blendStrength;
            public float textureScale;
        }
    }
}