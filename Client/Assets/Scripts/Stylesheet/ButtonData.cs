using UnityEngine;

namespace Stylesheet
{
    public class ButtonData : ScriptableObject
    {
        public ColorData Normal { get { return normal; } }
        public ColorData Highlighted { get { return highlighted; } }
        public ColorData Pressed { get { return pressed; } }
        public ColorData Disabled { get { return disabled; } }
        public float ColorMultiplier { get { return colorMultiplier; } }

        [SerializeField]
        ColorData normal;
        [SerializeField]
        ColorData highlighted;
        [SerializeField]
        ColorData pressed;
        [SerializeField]
        ColorData disabled;
        [SerializeField]
        float colorMultiplier;
    }
}
