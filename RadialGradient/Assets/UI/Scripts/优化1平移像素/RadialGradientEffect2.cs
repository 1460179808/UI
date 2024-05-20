using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//ƽ�����ص�
public class RadialGradientEffect2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
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
    private Color[] precomputedGradient; // Ԥ�ȼ���Ľ�����ɫ����

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

        // Ԥ�ȼ����ݶ�
        precomputedGradient = new Color[textureSize * textureSize];
        PrecomputeGradient(new Vector2(0.5f, 0.5f)); // ��ʼ��Ԥ������ݶ�
        UpdateGradient(new Vector2(0.5f, 0.5f)); // �����ĳ�ʼ��
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
                lastMousePosition = normMousePosition;
                // ���½���
                UpdateGradient(normMousePosition);
            }
        }
    }

    // Ԥ�����ݶ���ɫ
    void PrecomputeGradient(Vector2 center)
    {
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float u = (float)x / (textureSize - 1);
                float v = (float)y / (textureSize - 1);
                Vector2 uv = new Vector2(u, v);

                float dist = Vector2.Distance(uv, center);
                Color color = Color.Lerp(Color.white, gradientColor, Mathf.SmoothStep(radius, radius + 0.1f, dist));
                precomputedGradient[y * textureSize + x] = color;
            }
        }
    }

    // ���½�������
    void UpdateGradient(Vector2 center)
    {
        Color[] pixels = new Color[textureSize * textureSize];

        // ������������ĵ�����λ��
        int offsetX = (int)((center.x - 0.5f) * textureSize);
        int offsetY = (int)((center.y - 0.5f) * textureSize);

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                int shiftedX = x - offsetX;
                int shiftedY = y - offsetY;

                if (shiftedX >= 0 && shiftedX < textureSize && shiftedY >= 0 && shiftedY < textureSize)
                {
                    pixels[y * textureSize + x] = precomputedGradient[shiftedY * textureSize + shiftedX];
                }
                else
                {
                    pixels[y * textureSize + x] = Color.white; // ��������߽磬��Ĭ�ϱ�����ɫ
                }
            }
        }

        // Ӧ���������ݲ���������
        gradientTexture.SetPixels(pixels);
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
        UpdateGradient(new Vector2(0.5f, 0.5f)); // ��ָ���˳�ʱ�����ݶ�
    }

    // ָ���ƶ��¼�������Ҫ������ΪUpdate�����Ѿ������ݶȸ���
    public void OnPointerMove(PointerEventData eventData)
    {
        // ���ﲻ��Ҫ�κζ�����Update���������ݶȸ���
    }
}
