using System;
using UnityEngine;
using UnityEngine.UI;

namespace Stylesheet
{
    [ExecuteInEditMode]
    public class ColorComponent : StylesheetComponent
    {
        public ColorData ColorData;

        protected override void ApplyStyle(Graphic graphic)
        {
            if (ColorData != null)
                graphic.color = ColorData.Color;
        }
    }
}
