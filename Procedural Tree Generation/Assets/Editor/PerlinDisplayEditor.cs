using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CustomEditor (typeof(PerlinNoiseDisplay))]
[ExecuteInEditMode]
public class PerlinDisplayEditor : Editor
{
    PerlinNoiseDisplay mapGenerator;

    public void OnEnable()
    {
        mapGenerator = (PerlinNoiseDisplay)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty treeType = serializedObject.FindProperty("Trees");

        //DrawDefaultInspector();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("generationType"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mesh"), true);

        int generationTypeIndex = (int)mapGenerator.generationType;
        GenerationTypeEditor(generationTypeIndex);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Number of Tree Types: ");
        EditorGUILayout.IntField(mapGenerator.Trees.Count, GUILayout.Width(80));
        GUILayout.Space(200);
        GUILayout.EndHorizontal();

        foreach (TreeType tree in mapGenerator.Trees)
        {
            int currentIndex = mapGenerator.Trees.IndexOf(tree);
            EditorGUILayout.PropertyField(treeType.GetArrayElementAtIndex(currentIndex), true);

            if (GUILayout.Button("Delete Trees"))
            {
                mapGenerator.DeleteObjects(currentIndex);
            }

            if (GUILayout.Button("Move Trees Down on Mesh"))
            {
                mapGenerator.MoveGameObjects(currentIndex);
            }
        }
        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            mapGenerator.ProceduralGeneration();
        }
        serializedObject.ApplyModifiedProperties();
    }

    public void GenerationTypeEditor (int enumNumber)
    {
        switch (enumNumber)
        {
            //Perlin noise generator
            case 0:
                {
                    //Noise scale property
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Noise Scale: ");
                    mapGenerator.noiseScale = EditorGUILayout.FloatField(mapGenerator.noiseScale, GUILayout.Width(80));
                    GUILayout.Space(200);
                    GUILayout.EndHorizontal();

                    //Octaves property
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Octaves: ");
                    mapGenerator.octaves = EditorGUILayout.IntField(mapGenerator.octaves, GUILayout.Width(80));
                    GUILayout.Space(200);
                    GUILayout.EndHorizontal();

                    //Persistance property
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Persistance: ");
                    GUILayout.Space(20);
                    mapGenerator.persistance = EditorGUILayout.Slider(mapGenerator.persistance, 0, 1, GUILayout.Width(200));
                    GUILayout.Space(200);
                    GUILayout.EndHorizontal();

                    //Luncarity property
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Lcunarity: ");
                    mapGenerator.lacunarity = EditorGUILayout.FloatField(mapGenerator.lacunarity, GUILayout.Width(80));
                    GUILayout.Space(200);
                    GUILayout.EndHorizontal();

                    //Seed property
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Seed: ");
                    mapGenerator.seed = EditorGUILayout.IntField(mapGenerator.seed, GUILayout.Width(80));
                    GUILayout.Space(200);
                    GUILayout.EndHorizontal();

                    //Offset property
                    EditorGUILayout.Vector2Field("Offset: ", mapGenerator.offset);
                }
                break;

            //Texture genrerator
            case 1:
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("textureMap"), true);
                }
                break;
        }
    }
}
