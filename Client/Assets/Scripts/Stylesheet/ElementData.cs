using UnityEngine;

namespace Stylesheet
{
    public class ElementData : ScriptableObject
    {
        public ColorData ColorData { get { return colorData; } }

        [SerializeField]
        ColorData colorData;
    }
}

