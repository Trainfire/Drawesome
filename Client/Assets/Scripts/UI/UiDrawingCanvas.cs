using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Stylesheet;

[RequireComponent(typeof(RawImage))]
public class UiDrawingCanvas : UiBase
{
    public ColorData[] PlayerColors = new ColorData[8];
    public Color BrushColor;
    public int BrushSize = 8; // TODO: Load from settings
    public bool AllowDrawing;

    Vector2 MousePosition { get; set; }

    RawImage rawImage;
    RawImage RawImage
    {
        get
        {
            if (rawImage == null)
            {
                rawImage = GetComponent<RawImage>();
                rawImage.texture = GenerateTexture();
            }
            return rawImage;
        }
    }

    public Texture2D Texture
    {
        get
        {
            return RawImage.texture as Texture2D;
        }
        set
        {
            RawImage.texture = value;
        }
    }

    void Start()
    {
        // Make sure anchor is set to top right, otherwise drawing will be very broken
        var rect = (RectTransform)transform;
        rect.pivot = new Vector2(0f, 1f);
    }

    Texture2D GenerateTexture()
    {
        var rect = transform as RectTransform;

        // Generate texture
        Texture = new Texture2D((int)rect.rect.width, (int)rect.rect.height, TextureFormat.RGB24, false);

        // Make a Colours array the size of the texture
        var colors = new Color[Texture.GetPixels().Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }

        Texture.SetPixels(colors);
        Texture.Apply();

        return Texture;
    }

    void Update()
    {
        MousePosition = Input.mousePosition;

        if (Input.GetMouseButton(0) && AllowDrawing)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, MousePosition))
            {
                Vector2 result;
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, MousePosition, null, out result);
                DrawCircle((int)result.x, (int)result.y, BrushSize, BrushColor);
                Texture.Apply();
            }
        }
    }

    void DrawCircle(int cx, int cy, int r, Color color)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                Texture.SetPixel(px, py, color);
                Texture.SetPixel(nx, py, color);

                Texture.SetPixel(px, ny, color);
                Texture.SetPixel(nx, ny, color);
            }
        }
    }
}
