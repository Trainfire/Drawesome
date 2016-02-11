using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace Stylesheet
{
    public class TextComponent : StylesheetComponent
    {
        public TextData TextData;

        protected override void ApplyStyle(Graphic graphic)
        {
            var text = graphic as Text;

            if (text != null && TextData != null)
            {
                text.fontSize = TextData.TextSize;
                text.lineSpacing = TextData.LineSpacing;

                if (TextData.Color != null)
                    text.color = TextData.Color.Color;
            }
        }
    }

}
