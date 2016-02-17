using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[CustomEditor(typeof(Test))]
public class EditorTest : Editor
{
    Test instance;
    Test Instance
    {
        get
        {
            if (instance == null)
                instance = target as Test;
            return instance;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Do Test"))
            Instance.PerformTest();

    }
}
