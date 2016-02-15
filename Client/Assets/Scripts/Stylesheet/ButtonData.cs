using UnityEngine;

namespace Stylesheet
{
    public class ButtonData : ScriptableObject
    {
        public ColorData Normal { get { return normal; } }
        public ColorData Highlighted { get { return highlighted; } }
        public ColorData Pressed { get { return pressed; } }
        public Color Disabled { get { return new Color(disabled.Color.r, disabled.Color.g, disabled.Color.b, disabledAlpha); } }
        public float DisabledAlpha { get { return disabledAlpha; } }
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
        float disabledAlpha = 1f;
        [SerializeField]
        float colorMultiplier;
    }
}
