using UnityEditor;
using UnityEngine;

public static class TerrainSettingsCopier
{
    [MenuItem("Tools/Terrain/Copy Settings To Neighbors")]
    static void CopyToNeighbors()
    {
        Terrain source = Selection.activeGameObject?.GetComponent<Terrain>();
        if (source == null)
        {
            Debug.LogWarning("Select a Terrain first.");
            return;
        }

        Terrain[] neighbors = { source.leftNeighbor, source.rightNeighbor, source.topNeighbor, source.bottomNeighbor };

        foreach (var n in neighbors)
        {
            if (n == null) continue;
            CopySettings(source, n);
            EditorUtility.SetDirty(n);
            EditorUtility.SetDirty(n.terrainData);
        }
    }

    static void CopySettings(Terrain src, Terrain dst)
    {
        dst.materialTemplate = src.materialTemplate;
        //dst.materialType = src.materialType;
        dst.heightmapPixelError = src.heightmapPixelError;
        dst.basemapDistance = src.basemapDistance;
        dst.shadowCastingMode = src.shadowCastingMode;
        dst.reflectionProbeUsage = src.reflectionProbeUsage;
        dst.drawInstanced = src.drawInstanced;
        dst.detailObjectDistance = src.detailObjectDistance;
        dst.detailObjectDensity = src.detailObjectDensity;
        dst.treeDistance = src.treeDistance;
        dst.treeBillboardDistance = src.treeBillboardDistance;
        dst.treeCrossFadeLength = src.treeCrossFadeLength;
        dst.treeMaximumFullLODCount = src.treeMaximumFullLODCount;
        dst.heightmapMaximumLOD = src.heightmapMaximumLOD;
        dst.allowAutoConnect = src.allowAutoConnect;
        dst.groupingID = src.groupingID;

        // Shared TerrainData assets — same layers/prototypes across the grid
        dst.terrainData.terrainLayers = src.terrainData.terrainLayers;
        dst.terrainData.detailPrototypes = src.terrainData.detailPrototypes;
        dst.terrainData.treePrototypes = src.terrainData.treePrototypes;
        dst.terrainData.wavingGrassStrength = src.terrainData.wavingGrassStrength;
        dst.terrainData.wavingGrassAmount = src.terrainData.wavingGrassAmount;
        dst.terrainData.wavingGrassSpeed = src.terrainData.wavingGrassSpeed;
        dst.terrainData.wavingGrassTint = src.terrainData.wavingGrassTint;
    }
}