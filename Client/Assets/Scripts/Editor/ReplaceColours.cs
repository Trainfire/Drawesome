using UnityEditor;
using UnityEngine;
using Stylesheet;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// WARNING: REALLY POORLY WRITTEN CLASS.
/// </summary>
public class ReplaceColours : ScriptableWizard
{
    public ColorData[] OldColors;
    public ColorData[] NewColors;

    [MenuItem("GameObject/Replace Colours")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ReplaceColours>("Replace Colours", "Apply");
    }

    void OnWizardCreate()
    {

        var comps = Resources.FindObjectsOfTypeAll<ColorComponent>();
        Debug.LogFormat("Found {0} components", comps.Length);

        // Make key value pairs
        var colorSet = new List<KeyValuePair<ColorData, ColorData>>();

        for (int i = 0; i < OldColors.Length; i++)
        {
            colorSet.Add(new KeyValuePair<ColorData, ColorData>(OldColors[i], NewColors[i]));
        }

        // Replace components
        foreach (var comp in comps)
        {
            foreach (var colorPair in colorSet)
            {
                // find match for old colour
                if (comp.ColorData == colorPair.Key)
                {
                    // replace old colour with new color
                    comp.ColorData = colorPair.Value;
                    break;
                }
            }
        }
    }

    // When the user pressed the "Apply" button OnWizardOtherButton is called.
    void OnWizardOtherButton()
    {
        if (Selection.activeTransform != null)
        {
            Light lt = Selection.activeTransform.GetComponent<Light>();

            if (lt != null)
            {
                lt.color = Color.red;
            }
        }
    }

    public static Object[] GetAssetsOfType(System.Type type, string fileExtension)
    {
        List<Object> tempObjects = new List<Object>();
        DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
        FileInfo[] goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);

        int i = 0; int goFileInfoLength = goFileInfo.Length;
        FileInfo tempGoFileInfo; string tempFilePath;
        Object tempGO;
        for (; i < goFileInfoLength; i++)
        {
            tempGoFileInfo = goFileInfo[i];
            if (tempGoFileInfo == null)
                continue;

            tempFilePath = tempGoFileInfo.FullName;
            tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");

            Debug.Log(tempFilePath + "\n" + Application.dataPath);

            tempGO = AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(Object)) as Object;
            if (tempGO == null)
            {
                continue;
            }
            else if (tempGO.GetType() != type)
            {
                continue;
            }

            tempObjects.Add(tempGO);
        }

        return tempObjects.ToArray();
    }
}
