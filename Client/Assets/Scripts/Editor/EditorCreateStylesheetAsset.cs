using UnityEditor;

namespace Stylesheet
{
    public class EditorCreateStylesheetAsset
    {
        [MenuItem("Assets/Create/Stylesheet/FontData")]
        public static void CreateStylesheetFont()
        {
            EditorScriptableObjectUtility.CreateAsset<TextData>();
        }

        [MenuItem("Assets/Create/Stylesheet/ColorData")]
        public static void CreateStylesheetColor()
        {
            EditorScriptableObjectUtility.CreateAsset<ColorData>();
        }

        [MenuItem("Assets/Create/Stylesheet/ButtonData")]
        public static void CreateStylesheetButton()
        {
            EditorScriptableObjectUtility.CreateAsset<ButtonData>();
        }

        [MenuItem("Assets/Create/Stylesheet/ElementData")]
        public static void CreateStylesheetElement()
        {
            EditorScriptableObjectUtility.CreateAsset<ElementData>();
        }
    }

}
