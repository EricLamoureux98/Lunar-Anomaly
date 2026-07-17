using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RockScatterTool : EditorWindow
{
    private List<GameObject> rockPrefabs = new List<GameObject>() { null };
    private Terrain terrain;
    private int count = 50;
    private float minSpacing = 3f;
    private float maxSlopeDeg = 35f;
    private int groundLayer;
    private int maxAttemptsPerRock = 30;

    [MenuItem("Tools/Rock Scatter Tool")]
    private static void Open() => GetWindow<RockScatterTool>("Rock Scatter");

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Rock Prefabs", EditorStyles.boldLabel);
        for (int i = 0; i < rockPrefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            rockPrefabs[i] = (GameObject)EditorGUILayout.ObjectField(rockPrefabs[i], typeof(GameObject), false);
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                rockPrefabs.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+ Add Prefab Slot"))
            rockPrefabs.Add(null);

        EditorGUILayout.Space();
        terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", terrain, typeof(Terrain), true);
        count = EditorGUILayout.IntField("Count", count);
        minSpacing = EditorGUILayout.FloatField("Min Spacing", minSpacing);
        maxSlopeDeg = EditorGUILayout.FloatField("Max Slope (deg)", maxSlopeDeg);
        groundLayer = EditorGUILayout.LayerField("Ground Layer", groundLayer);

        EditorGUILayout.Space();
        if (GUILayout.Button("Scatter Rocks"))
            Scatter();
    }

    private void Scatter()
    {
        var validPrefabs = rockPrefabs.FindAll(p => p != null);
        if (validPrefabs.Count == 0 || terrain == null)
        {
            Debug.LogWarning("Assign at least one prefab and a terrain first.");
            return;
        }

        var container = new GameObject("Scattered_Rocks");
        Undo.RegisterCreatedObjectUndo(container, "Scatter Rocks");

        var placed = new List<Vector3>();
        var bounds = terrain.terrainData.bounds;
        var terrainPos = terrain.transform.position;
        int mask = 1 << groundLayer;

        int placedCount = 0;
        int safety = count * maxAttemptsPerRock;

        while (placedCount < count && safety-- > 0)
        {
            float x = Random.Range(terrainPos.x, terrainPos.x + bounds.size.x);
            float z = Random.Range(terrainPos.z, terrainPos.z + bounds.size.z);

            Vector3 rayOrigin = new Vector3(x, terrainPos.y + bounds.size.y + 50f, z);

            if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, bounds.size.y + 100f, mask))
                continue;

            float slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope > maxSlopeDeg)
                continue;

            bool tooClose = false;
            foreach (var p in placed)
            {
                if (Vector3.Distance(p, hit.point) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose)
                continue;

            var prefab = validPrefabs[Random.Range(0, validPrefabs.Count)];
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);
            instance.transform.position = hit.point;
            instance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal)
                * Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            Undo.RegisterCreatedObjectUndo(instance, "Scatter Rocks");
            placed.Add(hit.point);
            placedCount++;
        }

        Debug.Log($"Placed {placedCount}/{count} rocks.");
    }
}