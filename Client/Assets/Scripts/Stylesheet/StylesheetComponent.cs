using UnityEngine;
using UnityEngine.UI;

namespace Stylesheet
{
    [ExecuteInEditMode]
    public abstract class StylesheetComponent : MonoBehaviour
    {
        Graphic Graphic { get; set; }

        void Update()
        {
            if (Graphic == null)
                Graphic = GetComponent<Graphic>();

            if (Graphic != null)
                ApplyStyle(Graphic);
        }

        protected abstract void ApplyStyle(Graphic graphic);
    }
}


