using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(PerlinNoiseDisplay))]
public class PerlinDisplayEditor : Editor
{
    PerlinNoiseDisplay mapGenerator;

    public void OnEnable()
    {
        mapGenerator = (PerlinNoiseDisplay)target;
    }

    public override void OnInspectorGUI()
    {
        //if (DrawDefaultInspector())
        //{
        //    if (mapGenerator.autoUpdate)
        //    {
        //        mapGenerator.DrawNoiseMap();
        //    }
        //}
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            mapGenerator.DrawNoiseMap();
        }
    }
}
