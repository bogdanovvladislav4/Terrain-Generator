
using System.Collections.Generic;
using UnityEngine;

namespace TerrainProceduralGenerator
{
    public class TerrainGenerator : MonoBehaviour
    {
        private const float viewerMoveThresholdForChunkUpdate = 25f;

        private const float sqrMoveThresholdForChunkUpdate =
            viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

        public int colliderLODIndex;
        public LODInfo[] detailLevels;

        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureSettings;

        public Transform viewer;

        public Material mapMaterial;
        private float meshWorldsSize;
        private int _chunksVisibleInViewDst;

        private Vector2 ViewerPosition;
        private Vector2 viewerPosiionOld;

        private Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        private List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

        private void Start()
        {
            textureSettings.ApplyToMaterial(mapMaterial);
            textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

            float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
            meshWorldsSize = meshSettings.meshWorldsSize;
            _chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldsSize);
            UpdateVisibleChunks();
        }

        private void Update()
        {
            ViewerPosition = new Vector2(viewer.position.x, viewer.position.z);

            if (ViewerPosition != viewerPosiionOld)
            {
                foreach (TerrainChunk terrainChunk in visibleTerrainChunks)
                {
                    terrainChunk.UpdateCollisionMesh();
                }
            }

            if ((viewerPosiionOld - ViewerPosition).sqrMagnitude > sqrMoveThresholdForChunkUpdate)
            {
                viewerPosiionOld = ViewerPosition;
                UpdateVisibleChunks();
            }
        }

        void UpdateVisibleChunks()
        {
            HashSet<Vector2> alreadyUpdatesChunkCoords = new HashSet<Vector2>();
            for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
            {
                alreadyUpdatesChunkCoords.Add(visibleTerrainChunks[i].coord);
                visibleTerrainChunks[i].UpdateTerrainChunk();
            }

            int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / meshWorldsSize);
            int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / meshWorldsSize);

            for (int yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++)
            {
                for (int xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                    if (!alreadyUpdatesChunkCoords.Contains(viewedChunkCoord))
                    {
                        if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                        {
                            _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                        }
                        else
                        {
                            TerrainChunk terrainChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings,
                                meshSettings,
                                detailLevels, colliderLODIndex, transform, viewer, mapMaterial);

                            _terrainChunkDictionary.Add(viewedChunkCoord, terrainChunk);
                            terrainChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                            terrainChunk.Load();
                        }
                    }
                }
            }
        }

        void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
        {
            if (isVisible)
            {
                visibleTerrainChunks.Add(chunk);
            }
            else
            {
                visibleTerrainChunks.Remove(chunk);
            }
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        [Range(0, MeshSettings.NumSupportedLoDs - 1)]
        public int lod;

        public float visibleDstThreshold;

        public float sqrVisibleDstThreshold => visibleDstThreshold * visibleDstThreshold;
    }
}