using System;
using UnityEngine.UI;

namespace Stylesheet
{
    public class ElementComponent : StylesheetComponent
    {
        public ElementData ElementData;

        protected override void ApplyStyle(Graphic graphic)
        {
            if (ElementData != null)
            {
                graphic.color = ElementData.Color;
            }
        }
    }
}
