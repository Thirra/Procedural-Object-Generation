using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CustomEditor (typeof(TreeGenerationDisplay))]
[ExecuteInEditMode]
public class ProceduralGenerationDisplayEditor : Editor
{
    TreeGenerationDisplay mapGenerator;

    public void OnEnable()
    {
        mapGenerator = (TreeGenerationDisplay)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty treeType = serializedObject.FindProperty("Trees");

        //DrawDefaultInspector();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("generationType"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mesh"), true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("meshWidth"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("meshLength"), true);

        int generationTypeIndex = (int)mapGenerator.generationType;
        GenerationTypeEditor(generationTypeIndex);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Game Objects to Generate: ");
        if (GUILayout.Button("Add Tree Type"))
        {
            mapGenerator.Trees.Add(new TreeType());
            serializedObject.Update();
        }
        GUILayout.Space(200);
        GUILayout.EndHorizontal();

        foreach (TreeType tree in mapGenerator.Trees)
        {
            int currentIndex = mapGenerator.Trees.IndexOf(tree);
            EditorGUILayout.PropertyField(treeType.GetArrayElementAtIndex(currentIndex), true);

            GUILayout.BeginHorizontal();
            if (mapGenerator.generationType == TreeGenerationDisplay.GenerationType.PerlinNoise)
            {
                if (GUILayout.Button("Generate PerlinNoise Trees"))
                {
                    mapGenerator.DrawNoiseMap(currentIndex);
                }
            }
            else
            {
                if (GUILayout.Button("Generate Texture Mapped Trees"))
                {
                    mapGenerator.DrawTextureMap(currentIndex);
                }
            }

            if (GUILayout.Button("Move Trees Down on Mesh"))
            {
                mapGenerator.MoveGameObjects(currentIndex);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete Generated Trees"))
            {
                mapGenerator.DeleteObjects(currentIndex);
            }

            if (GUILayout.Button("- Delete Tree Type -"))
            {
                mapGenerator.Trees.RemoveAt(currentIndex);
                serializedObject.Update();
            }
            GUILayout.EndHorizontal();
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
