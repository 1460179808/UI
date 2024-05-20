using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// ʵ�ָ���Ч�ľ��򽥱�Ч����ֻ���±�Ҫ������
public class RadialGradientEffect3 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public Image image; // UI�е�Image���
    public Color gradientColor = new Color(1, 1, 1, 0.6f); // ������ɫ
    public float radius = 0.5f; // ����뾶
    public int textureSize = 256; // �����С
    public float updateThreshold = 0.01f; // ������ֵ

    private Texture2D gradientTexture; // ��������
    private RectTransform rectTransform; // RectTransform���
    private Vector2 lastMousePosition; // �ϴ����λ��
    private bool isPointerOver = false; // ָ���Ƿ���ͣ��ͼ����

    void Start()
    {
        // ��ʼ��image���
        if (image == null)
        {
            image = GetComponent<Image>();
        }

        // ��ȡRectTransform���
        rectTransform = image.GetComponent<RectTransform>();

        // ������������
        gradientTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        gradientTexture.wrapMode = TextureWrapMode.Clamp;
        image.sprite = Sprite.Create(gradientTexture, new Rect(0, 0, gradientTexture.width, gradientTexture.height), new Vector2(0.5f, 0.5f));

        //UpdateGradient(new Vector2(0.5f, 0.5f)); // �����ĳ�ʼ��
    }

    void Update()
    {
        if (isPointerOver)
        {
            // �����λ��ת��Ϊ�ֲ�����
            Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
            // ��һ�����λ��
            Vector2 normMousePosition = new Vector2(
                Mathf.InverseLerp(-rectTransform.rect.width / 2, rectTransform.rect.width / 2, localMousePosition.x),
                Mathf.InverseLerp(-rectTransform.rect.height / 2, rectTransform.rect.height / 2, localMousePosition.y)
            );

            // �ж����λ���Ƿ�仯������ֵ
            if (Vector2.Distance(normMousePosition, lastMousePosition) > updateThreshold)
            {
                // ��¼�ϴε����λ��
                Vector2 prevMousePosition = lastMousePosition;
                lastMousePosition = normMousePosition;

                // �����ݶȣ�ֻ���±�Ҫ������
                UpdateGradientPartially(prevMousePosition, normMousePosition);
            }
        }
    }

    void UpdateGradientPartially(Vector2 prevCenter, Vector2 newCenter)
    {
        // ����ÿ��Բ��֮������ط�Χ�仯
        int prevCenterX = Mathf.RoundToInt(prevCenter.x * (textureSize - 1));
        int prevCenterY = Mathf.RoundToInt(prevCenter.y * (textureSize - 1));
        int newCenterX = Mathf.RoundToInt(newCenter.x * (textureSize - 1));
        int newCenterY = Mathf.RoundToInt(newCenter.y * (textureSize - 1));

        int minX = Mathf.Min(prevCenterX, newCenterX) - Mathf.CeilToInt(radius * textureSize) - 1;
        int maxX = Mathf.Max(prevCenterX, newCenterX) + Mathf.CeilToInt(radius * textureSize) + 1;
        int minY = Mathf.Min(prevCenterY, newCenterY) - Mathf.CeilToInt(radius * textureSize) - 1;
        int maxY = Mathf.Max(prevCenterY, newCenterY) + Mathf.CeilToInt(radius * textureSize) + 1;

        minX = Mathf.Clamp(minX, 0, textureSize - 1);
        maxX = Mathf.Clamp(maxX, 0, textureSize - 1);
        minY = Mathf.Clamp(minY, 0, textureSize - 1);
        maxY = Mathf.Clamp(maxY, 0, textureSize - 1);

        // ���±�Ҫ����
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float u = (float)x / (textureSize - 1);
                float v = (float)y / (textureSize - 1);
                Vector2 uv = new Vector2(u, v);

                float prevDist = Vector2.Distance(uv, prevCenter);
                float newDist = Vector2.Distance(uv, newCenter);

                if (prevDist > radius + 0.1f || newDist <= radius + 0.1f)
                {
                    Color color = Color.Lerp(Color.white, gradientColor, Mathf.SmoothStep(radius, radius + 0.1f, newDist));
                    gradientTexture.SetPixel(x, y, color);
                }
            }
        }

        gradientTexture.Apply();
    }

    // ��ָ�����ͼ��ʱ
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        lastMousePosition = Vector2.positiveInfinity; // ǿ�Ƹ��µ�һ�ν���ʱ�Ľ���
    }

    // ��ָ���뿪ͼ��ʱ
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        //UpdateGradient(new Vector2(0.5f, 0.5f)); // ��ָ���˳�ʱ�����ݶ�
    }

    // ָ���ƶ��¼�������Ҫ������ΪUpdate�����Ѿ������ݶȸ���
    public void OnPointerMove(PointerEventData eventData)
    {
        // ���ﲻ��Ҫ�κζ�����Update���������ݶȸ���
    }
}
