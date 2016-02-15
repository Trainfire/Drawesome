using UnityEngine;

namespace Stylesheet
{
    public class ElementData : ScriptableObject
    {
        public Color Color
        {
            get
            {
                return new Color(ColorData.Color.r, ColorData.Color.g, ColorData.Color.b, alpha);
            }
        }
        public ColorData ColorData { get { return colorData; } }
        public float Alpha { get { return alpha;} }

        [SerializeField]
        ColorData colorData;
        [SerializeField]
        float alpha = 1f;
    }
}

