using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[CustomEditor(typeof(UiDrawingCanvas))]
public class DrawingCanvasEditor : Editor
{
    UiDrawingCanvas instance;
    UiDrawingCanvas Instance
    {
        get
        {
            if (instance == null)
                instance = target as UiDrawingCanvas;
            return instance;
        }
    }

    Color Color { get; set; }

    void Start()
    {
        Color = Instance.Color;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Save"))
            Save();

        if (GUILayout.Button("Load"))
            Load();

        Color = EditorGUILayout.ColorField(Color);

        if (GUILayout.Button("Recolor"))
            Recolor(Color);
    }

    void Save()
    {
        File.WriteAllBytes(Application.dataPath + "/picture.png", Instance.GetTexture.EncodeToPNG());
    }

    void Load()
    {
        var bytes = File.ReadAllBytes(Application.dataPath + "/picture.png");
        Instance.SetImage(bytes);
    }

    void Recolor(Color color)
    {
        Instance.Recolor(color);
    }
}
