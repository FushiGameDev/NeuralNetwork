using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DrawingScript : MonoBehaviour
{
    public GameObject panel;
    [SerializeField] int brushSize = 20;

    RawImage rawImage;
    Texture2D texture;
    
    Recognizer recognizer;
    Vector2? lastPos = null;

    private void Start()
    {
        recognizer = GameObject.FindAnyObjectByType<Recognizer>();

        // create texture
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        texture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height);
        
        var colors = new Color[texture.width * texture.height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.white;

        texture.SetPixels(colors);
        texture.Apply();

        rawImage = panel.GetComponent<RawImage>();
        rawImage.texture = texture;
    }

    private void Update()
    {
        if (Input.touchCount > 0) 
        {
            Draw(new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
        }
        else
        {
            lastPos = null;
        }
    }

    void Draw(Vector2 touchPos)
    {
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touchPos, null, out localPos);

        // guardian
        if (localPos.x < rectTransform.rect.xMin || localPos.x > rectTransform.rect.xMax || localPos.y < rectTransform.rect.yMin || localPos.y > rectTransform.rect.yMax) 
            return;

        int x = (int)((localPos.x / rectTransform.rect.width) * texture.width + texture.width / 2);
        int y = (int)((localPos.y / rectTransform.rect.height) * texture.height + texture.height / 2);

        if (lastPos.HasValue) // check if cursor jumped 
        {
            DrawLine((int)lastPos.Value.x, (int)lastPos.Value.y, x, y);
        }
        else
        {
            DrawCircle(x, y);
        }

        lastPos = new Vector2(x,y);
        texture.Apply();
    }

    void DrawLine(int x0, int y0, int x1, int y1) // Algorithm Bresenhama
    {
        //diffs
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        //direction
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            DrawCircle(x0, y0);

            if (x0 == x1 && y0 == y1) break; // if line is on destination - break
            int e2 = 2 * err; // algorithm
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }

    void DrawCircle(int cx, int cy)
    {
        // calculate field of pixels
        int startX = cx - brushSize / 2;
        int startY = cy - brushSize / 2;
        int endX = cx + brushSize / 2;
        int endY = cy + brushSize / 2;

        // check if brushsize can be fully painted
        bool isWithinBounds = startX >= 0 && startY >= 0 && endX < texture.width && endY < texture.height;

        if (isWithinBounds) // if so then faster solution
        {
            Color[] colors = new Color[brushSize * brushSize];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.black;

            texture.SetPixels(startX, startY, brushSize, brushSize, colors);
        }
        else // if not then slower solution
        {
            for (int i = 0; i < brushSize; i++)
            {
                for (int j = 0; j < brushSize; j++)
                {
                    int pixelX = startX + i;
                    int pixelY = startY + j;

                    if (pixelX >= 0 && pixelX < texture.width && pixelY >= 0 && pixelY < texture.height)
                    {
                        texture.SetPixel(pixelX, pixelY, Color.black);
                    }
                }
            }
        }
    }


    public void RecognizeDrawing()
    {
        recognizer.InputImage = rawImage.texture as Texture2D;
        ClearTexture();
    }

    void ClearTexture()
    {
        Color[] colors = new Color[texture.width * texture.height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.white;
        texture.SetPixels(colors);
        texture.Apply();
    }
}
