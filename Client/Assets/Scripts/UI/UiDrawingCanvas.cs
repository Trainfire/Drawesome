using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Stylesheet;

[RequireComponent(typeof(RawImage))]
public class UiDrawingCanvas : MonoBehaviour
{
    public ColorData[] PlayerColors = new ColorData[8];
    public Color Color; // TODO: Load from player
    public int BrushSize = 8; // TODO: Load from settings
    public bool AllowDrawing = true;

    Vector2 MousePosition { get; set; }
    Texture2D Texture { get; set; }
    RawImage RawImage { get; set; }

    enum StraightLineDirection
    {
        Horizontal,
        Vertical,
    }
    StraightLineDirection lockDirection = StraightLineDirection.Horizontal;

    public Texture2D GetTexture
    {
        get
        {
            return RawImage.texture as Texture2D;
        }
    }

    void Start()
    {
        RawImage = GetComponent<RawImage>();
        RawImage.texture = GenerateTexture();

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
                DrawCircle((int)result.x, (int)result.y, BrushSize, Color);
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

    public byte[] GetEncodedImage
    {
        get
        {
            return Texture.EncodeToPNG();
        }
    }

    public void SetImage(byte[] data)
    {
        var texture = RawImage.texture as Texture2D;
        texture.LoadImage(data);
        texture.Apply();
        RawImage.texture = texture;
    }

    public void SetBrushColor(uint colorId)
    {
        int index = (int)colorId;
        Color = PlayerColors[index].Color;
    }

    public void Recolor(Color color)
    {
        Color = color;

        var pixels = Texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i] != Color.white)
                pixels[i] = Color;
        }

        Texture.SetPixels(pixels);
        Texture.Apply();
    }
}
