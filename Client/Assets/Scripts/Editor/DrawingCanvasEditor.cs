using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[CustomEditor(typeof(UiDrawingCanvas))]
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
        UiDrawingCanvas instance = target as UiDrawingCanvas;
        File.WriteAllBytes(Application.dataPath + "/picture.png", instance.GetTexture.EncodeToPNG());
    }

    void Load()
    {
        var bytes = File.ReadAllBytes(Application.dataPath + "/picture.png");
        var instance = target as UiDrawingCanvas;
        instance.SetImage(bytes);
    }
}
