using UnityEngine;

namespace Stylesheet
{
    public class TextData : ScriptableObject
    {
        public int TextSize { get { return textSize; } }
        public float LineSpacing { get { return lineSpacing; } }

        [SerializeField]
        private int textSize;
        [SerializeField]
        private float lineSpacing = 1f;
    }
}
