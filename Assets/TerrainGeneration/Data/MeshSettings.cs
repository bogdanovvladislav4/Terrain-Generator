
using UnityEngine;

namespace TerrainProceduralGenerator
{
    [CreateAssetMenu]
    public class MeshSettings : UpdatableData
    {
        public const int NumSupportedLoDs = 5;
        public const int NumSupportedChunkSizes = 9;
        public const int NumSupportedFlatsShadedChunkSizes = 3;
        public static readonly int[] SupportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

        public float meshScale = 2.5f;
        public bool useFlatShading;

        [Range(0, NumSupportedChunkSizes - 1)] public int chunksSizeIndex;

        [Range(0, NumSupportedFlatsShadedChunkSizes - 1)]
        public int flatShadedChunksSizeIndex;

        //num verts per line of mesh rendered at LOD = 0. Includes the 2 extra verts that are excluded fromm final mesh,
        //but used for calculating normals
        public int numVertsPerLine
        {
            get { return SupportedChunkSizes[(useFlatShading) ? flatShadedChunksSizeIndex : chunksSizeIndex] + 5; }
        }

        public float meshWorldsSize
        {
            get { return (numVertsPerLine - 3) * meshScale; }
        }
    }
}