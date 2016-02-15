using UnityEngine;
using UnityEngine.UI;
using Stylesheet;

[RequireComponent(typeof(RawImage))]
public class UiDrawingCanvas : MonoBehaviour
{
    public ColorData[] PlayerColors = new ColorData[8];
    public Color BrushColor;
    public int BrushSize = 8; // TODO: Load from settings
    public int EraserSize = 8; // TODO: Load from settings
    public bool AllowDrawing;

    Vector2 CurrentMousePosition { get; set; }

    Vector2 previousMousePosition;
    Vector2 PreviousMousePosition
    {
        get
        {
            if (previousMousePosition == null)
                previousMousePosition = GetMousePosition();
            return previousMousePosition;
        }
        set
        {
            previousMousePosition = value;
        }
    }

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
        if (!AllowDrawing)
            return;

        if (Input.GetMouseButton(0))
        {
            Draw(PreviousMousePosition, GetMousePosition(), BrushColor, BrushSize);
            Texture.Apply();
        }

        if (Input.GetMouseButton(1))
        {
            Draw(PreviousMousePosition, GetMousePosition(), Color.white, EraserSize);
            Texture.Apply();
        }

        PreviousMousePosition = GetMousePosition();
    }

    Vector2 GetMousePosition()
    {
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, Input.mousePosition, null, out result);
        return result;
    }

    /// <summary>
    /// Draws a series of circles between two positions. This ensure that lines are correctly drawn during fast mouse movements.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    void Draw(Vector2 from, Vector2 to, Color color, int size)
    {
        var dist = Vector2.Distance(from, to);
        var dir = (to - from).normalized;
        var iterations = (int)(dist / size);

        Vector2[] positions = new Vector2[iterations + 2];
        positions[0] = from;
        positions[positions.Length - 1] = to;

        for (int i = 1; i < positions.Length - 1; i++)
        {
            positions[i] = positions[i - 1] + (dir * size);
        }

        for (int i = 0; i < positions.Length; i++)
        {
            DrawCircle((int)positions[i].x, (int)positions[i].y, size, color);
        }
    }

    /// <summary>
    /// Stolen from internets.
    /// </summary>
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
