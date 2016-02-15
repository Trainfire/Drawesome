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

    DrawingCanvas controller;
    DrawingCanvas Controller
    {
        get
        {
            if (controller == null)
                controller = new DrawingCanvas(Instance);
            return controller;
        }
    }

    Color Color { get; set; }

    void Start()
    {
        Color = Instance.BrushColor;
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
        File.WriteAllBytes(Application.dataPath + "/picture.png", Controller.GetEncodedImage());
    }

    void Load()
    {
        var bytes = File.ReadAllBytes(Application.dataPath + "/picture.png");
        Controller.SetImage(bytes);
    }

    void Recolor(Color color)
    {
        Controller.Recolor(color);
    }
}
