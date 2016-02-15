using UnityEngine;
using UnityEngine.UI;

namespace Stylesheet
{
    [ExecuteInEditMode]
    public class ButtonComponent : MonoBehaviour
    {
        public ButtonData ButtonData;

        Button Button { get; set; }

        void Update()
        {
            if (Button == null)
                Button = GetComponent<Button>();

            if (Button != null)
                ApplyStyle(Button);
        }

        void ApplyStyle(Button graphic)
        {
            if (ButtonData != null)
            {
                var colorBlock = new ColorBlock();
                colorBlock.normalColor = ButtonData.Normal.Color;
                colorBlock.highlightedColor = ButtonData.Highlighted.Color;
                colorBlock.pressedColor = ButtonData.Pressed.Color;
                colorBlock.disabledColor = ButtonData.Disabled;
                colorBlock.colorMultiplier = ButtonData.ColorMultiplier;

                graphic.colors = colorBlock;
            }
        }
    }
}
