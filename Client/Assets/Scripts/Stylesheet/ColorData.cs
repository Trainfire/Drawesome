using UnityEngine;

namespace Stylesheet
{
    public class ColorData : ScriptableObject
    {
        public Color Color { get { return color; } }

        [SerializeField]
        Color color = new Color(255f, 255f, 255f, 1f);
    }
}
