using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RawImage))]
public class DrawingCanvas : MonoBehaviour
{
    public Color Color;
    public int BrushSize = 16;

    Texture2D texture;
    RawImage rawImage;
    Vector3 lastDrawPosition;

    Vector3 lockPosition = Vector3.zero;
    bool straightLinesOnly = false;

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
            return rawImage.texture as Texture2D;
        }
    }

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        var rect = transform as RectTransform;

        // Generate texture.
        texture = new Texture2D((int)rect.rect.width, (int)rect.rect.height, TextureFormat.RGB24, false);

        // Make a Colours array the size of the texture
        var colors = new Color[texture.GetPixels().Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }

        texture.SetPixels(colors);
        texture.Apply();

        rawImage.texture = texture;
    }

    void Update()
    {
        var mousePosition = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            straightLinesOnly = true;
            lockPosition = mousePosition;
            lockDirection = GetStraightLineDirection();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            straightLinesOnly = false;
        }

        // Lock mouse position if left-shift modifier is held.
        var x = straightLinesOnly && lockDirection == StraightLineDirection.Vertical ? lockPosition.x : mousePosition.x;
        var y = straightLinesOnly && lockDirection == StraightLineDirection.Horizontal ? lockPosition.y : mousePosition.y;
        mousePosition = new Vector2(x, y);

        if (Input.GetMouseButton(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, mousePosition))
            {
                Vector2 result;
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, mousePosition, null, out result);
                DrawCircle((int)result.x, (int)result.y, BrushSize, Color.black);
                texture.Apply();
            }
        }

        // Cache mouse position for next frame.
        lastDrawPosition = mousePosition;
    }

    StraightLineDirection GetStraightLineDirection()
    {
        var direction = (Input.mousePosition - lastDrawPosition).normalized;

        if (Mathf.Abs(direction.x) > 0.5f)
        {
            return StraightLineDirection.Horizontal;
        }
        else if (Mathf.Abs(direction.y) > 0.5f)
        {
            return StraightLineDirection.Vertical;
        }

        return StraightLineDirection.Horizontal;
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

                texture.SetPixel(px, py, color);
                texture.SetPixel(nx, py, color);

                texture.SetPixel(px, ny, color);
                texture.SetPixel(nx, ny, color);
            }
        }
    }

    public void SetImage(byte[] data)
    {
        var texture = rawImage.texture as Texture2D;
        texture.LoadImage(data);
        texture.Apply();
        rawImage.texture = texture;
    }
}
