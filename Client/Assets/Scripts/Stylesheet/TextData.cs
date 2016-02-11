using UnityEngine;

namespace Stylesheet
{
    public class TextData : ScriptableObject
    {
        public int TextSize { get { return textSize; } }
        public float LineSpacing { get { return lineSpacing; } }
        public ColorData Color { get { return colorData; } }

        [SerializeField]
        private int textSize;
        [SerializeField]
        private float lineSpacing = 1f;
        [SerializeField]
        private ColorData colorData;
    }
}
