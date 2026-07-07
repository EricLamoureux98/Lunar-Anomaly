using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public static class TerrainSettingsCopier
{
    [MenuItem("Tools/Terrain/Copy Settings To Neighbors")]
    static void CopyToNeighbors()
    {
        Terrain source = Selection.activeGameObject?.GetComponent<Terrain>();
        if (source == null)
        {
            Debug.LogWarning("Select a Terrain first");
            return;
        }

        // Flood fill outward from the source too all connected terrain tiles
        HashSet<Terrain> visited = new HashSet<Terrain> { source };
        Queue<Terrain> queue = new Queue<Terrain>();
        queue.Enqueue(source);

        List<Object> objectsToUndo = new List<Object>();
        List<Terrain> targetsToUpdate = new List<Terrain>();

        while (queue.Count > 0)
        {
            Terrain current = queue.Dequeue();
            Terrain[] neighbors =
            {
                current.leftNeighbor,
                current.rightNeighbor,
                current.topNeighbor,
                current.bottomNeighbor
            };

            foreach (var n in neighbors)
            {
                if (n == null || visited.Contains(n)) continue;

                visited.Add(n);
                queue.Enqueue(n);
                targetsToUpdate.Add(n);

                // Collect for Undo history
                objectsToUndo.Add(n);
                if (n.terrainData != null)
                {
                    objectsToUndo.Add(n.terrainData);
                }
            }
        }

        if (targetsToUpdate.Count > 0)
        {
            // Register all targets for Undo functionality before modifying them
            Undo.RecordObjects(objectsToUndo.ToArray(), "Copy Terrain Settings");

            foreach (var target in targetsToUpdate)
            {
                CopySettings(source, target);
                EditorUtility.SetDirty(target);
                if (target.terrainData != null)
                {
                    EditorUtility.SetDirty(target.terrainData);
                }
            }

            Debug.Log($"Successfully copied ALL settings from '{source.name}' to {targetsToUpdate.Count} tile(s). Undo is available");
        }
        else
        {
            Debug.Log("No connected neighbor tiles found to update");
        }
    }

    static void CopySettings(Terrain src, Terrain dst)
    {
        if (src.terrainData == null || dst.terrainData == null) return;

        // ==========================================
        // 1. TERRAIN INSTANCE SETTINGS
        // ==========================================
        dst.materialTemplate = src.materialTemplate;
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
        dst.drawHeightmap = src.drawHeightmap;
        dst.drawTreesAndFoliage = src.drawTreesAndFoliage;
        dst.preserveTreePrototypeLayers = src.preserveTreePrototypeLayers;
        dst.enableHeightmapRayTracing = src.enableHeightmapRayTracing;

        // ==========================================
        // 2. SAFELY COPY DIMENSIONS
        // ==========================================
        dst.terrainData.size = src.terrainData.size;

        // ==========================================
        // 3. TERRAIN DATA RESOLUTION CRITICAL SAFETY WALL
        // ==========================================
        if (dst.terrainData.heightmapResolution != src.terrainData.heightmapResolution)
        {
            Debug.LogWarning($"[Skipped] {dst.name} heightmapResolution mismatch ({dst.terrainData.heightmapResolution} vs {src.terrainData.heightmapResolution}). Skipping to prevent asset data wipe.");
        }
        if (dst.terrainData.alphamapResolution != src.terrainData.alphamapResolution)
        {
            Debug.LogWarning($"[Skipped] {dst.name} alphamapResolution (splat map texture size) mismatch. Skipping to prevent texture paint wipe.");
        }
        if (dst.terrainData.detailResolution != src.terrainData.detailResolution)
        {
            Debug.LogWarning($"[Skipped] {dst.name} detailResolution (grass/foliage density grid) mismatch. Skipping to prevent grass density wipe.");
        }

        // ==========================================
        // 4. SHARED PROTOTYPES & PAINT LAYERS
        // ==========================================
        dst.terrainData.terrainLayers = src.terrainData.terrainLayers;
        dst.terrainData.detailPrototypes = src.terrainData.detailPrototypes;
        dst.terrainData.treePrototypes = src.terrainData.treePrototypes;

        // ==========================================
        // 5. ENVIRONMENT & RENDERING DATA
        // ==========================================
        dst.terrainData.wavingGrassStrength = src.terrainData.wavingGrassStrength;
        dst.terrainData.wavingGrassAmount = src.terrainData.wavingGrassAmount;
        dst.terrainData.wavingGrassSpeed = src.terrainData.wavingGrassSpeed;
        dst.terrainData.wavingGrassTint = src.terrainData.wavingGrassTint;

        dst.terrainData.SetDetailScatterMode(src.terrainData.detailScatterMode);
    }
}