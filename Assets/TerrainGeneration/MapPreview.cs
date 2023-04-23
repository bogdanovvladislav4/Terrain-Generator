
using UnityEngine;

namespace TerrainProceduralGenerator
{
    public class MapPreview : MonoBehaviour
    {
        public Renderer textureRenderer;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public enum DrawMode
        {
            NoiseMap,
            Mesh,
            FalloffMap
        }

        public DrawMode drawMode;

        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureData;

        public Material terrainMaterial;

        public bool autoUpdate;


        [Range(0, MeshSettings.NumSupportedLoDs - 1)]
        public int editorPreviewLOD;

        void OnTextureValuesUpdated()
        {
            textureData.ApplyToMaterial(terrainMaterial);
        }

        public void DrawMapInEditor()
        {
            textureData.ApplyToMaterial(terrainMaterial);
            textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
            HeightMap heightMap = HeightMapGenerator.GeneratorHeightMap(meshSettings.numVertsPerLine,
                meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);
            if (drawMode == DrawMode.NoiseMap)
            {
                DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                DrawMesh(MeshGenerator.GenerationTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
            }
            else if (drawMode == DrawMode.FalloffMap)
            {
                DrawTexture(
                    TextureGenerator.TextureFromHeightMap(new HeightMap(
                        FallOfGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine), 0, 1)));
            }
        }

        public void DrawTexture(Texture2D texture)
        {
            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

            textureRenderer.gameObject.SetActive(true);
            meshFilter.gameObject.SetActive(false);
        }

        public void DrawMesh(MeshData meshData)
        {
            meshFilter.sharedMesh = meshData.CreateMesh();

            textureRenderer.gameObject.SetActive(false);
            meshFilter.gameObject.SetActive(true);
        }

        void OnValuesUpdate()
        {
            if (!Application.isPlaying)
            {
                DrawMapInEditor();
            }
        }


        private void OnValidate()
        {
            if (meshSettings != null)
            {
                meshSettings.OnValuesUpdated -= OnValuesUpdate;
                meshSettings.OnValuesUpdated += OnValuesUpdate;
            }

            if (heightMapSettings != null)
            {
                heightMapSettings.OnValuesUpdated -= OnValuesUpdate;
                heightMapSettings.OnValuesUpdated += OnValuesUpdate;
            }

            if (textureData != null)
            {
                textureData.OnValuesUpdated -= OnTextureValuesUpdated;
                textureData.OnValuesUpdated += OnTextureValuesUpdated;
            }
        }
    }
}