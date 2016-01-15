using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[CustomEditor(typeof(DrawingCanvas))]
public class DrawingCanvasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Save"))
            Save();

        if (GUILayout.Button("Load"))
            Load();
    }

    void Save()
    {
        DrawingCanvas instance = target as DrawingCanvas;
        File.WriteAllBytes(Application.dataPath + "/picture.png", instance.GetTexture.EncodeToPNG());
    }

    void Load()
    {
        var bytes = File.ReadAllBytes(Application.dataPath + "/picture.png");
        var instance = target as DrawingCanvas;
        instance.SetImage(bytes);
    }
}
