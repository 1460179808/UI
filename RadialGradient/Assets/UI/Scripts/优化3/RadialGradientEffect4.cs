using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// ʵ�ֶ�㽥�䣬��˹ģ�������Ż����������ص���㷨
public class RadialGradientEffect4 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    // ͼ�����
    public Image image;
    // ������ɫ����
    public Color[] gradientColors = new Color[3] { new Color(1, 1, 1, 0.8f), new Color(1, 1, 1, 0.6f), new Color(1, 1, 1, 0.4f) };
    // �뾶����
    public float[] radii = new float[3] { 0.3f, 0.5f, 0.7f };
    // ��˹ģ����������
    public float[] sigmas = new float[3] { 0.1f, 0.15f, 0.2f };
    // �����С
    public int textureSize = 256;
    // ������ֵ
    public float updateThreshold = 0.01f;

    // ��������
    private Texture2D gradientTexture;
    // ��ɫ����
    private Texture2D solidColorTexture;
    // RectTransform���
    private RectTransform rectTransform;
    // �ϴ����λ��
    private Vector2 lastMousePosition;
    // ָ���Ƿ���ͣ��ͼ����
    private bool isPointerOver = false;
    // Ԥ�������ɫ����
    private Color[] precomputedColors;

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

        // ������ɫ����
        solidColorTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        solidColorTexture.SetPixel(0, 0, Color.white);
        solidColorTexture.Apply();

        // ���ó�ʼΪ��ɫ����
        image.sprite = Sprite.Create(solidColorTexture, new Rect(0, 0, solidColorTexture.width, solidColorTexture.height), new Vector2(0.5f, 0.5f));

        // Ԥ������ɫ
        precomputedColors = new Color[textureSize * textureSize];
        PrecomputeColors();
    }

    void Update()
    {
        // ���ָ����ͣ��ͼ����
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

    // Ԥ������ɫ
    void PrecomputeColors()
    {
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float u = (float)x / (textureSize - 1);
                float v = (float)y / (textureSize - 1);
                Vector2 uv = new Vector2(u, v);

                // ��ʼ��ɫΪ��ɫ
                Color finalColor = Color.white;

                // ����ÿ�㽥��
                for (int i = 0; i < gradientColors.Length; i++)
                {
                    float dist = Vector2.Distance(uv, new Vector2(0.5f, 0.5f)); // ������Ϊ��׼�������
                    if (dist <= radii[i])
                    {
                        float weight = Mathf.Exp(-dist * dist / (2 * sigmas[i] * sigmas[i])) / (2 * Mathf.PI * sigmas[i] * sigmas[i]);
                        Color layerColor = Color.Lerp(Color.white, gradientColors[i], Mathf.SmoothStep(radii[i], radii[i] + 0.1f, dist) * weight);
                        finalColor = Color.Lerp(finalColor, layerColor, weight);
                    }
                }

                precomputedColors[y * textureSize + x] = finalColor;
            }
        }
    }

    // ���½�������
    void UpdateGradient(Vector2 center)
    {
        Color[] pixels = new Color[textureSize * textureSize];

        // ������������ĵ�����λ��
        int centerX = Mathf.RoundToInt(center.x * (textureSize - 1));
        int centerY = Mathf.RoundToInt(center.y * (textureSize - 1));

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                int shiftedX = x - centerX + textureSize / 2;
                int shiftedY = y - centerY + textureSize / 2;

                if (shiftedX >= 0 && shiftedX < textureSize && shiftedY >= 0 && shiftedY < textureSize)
                {
                    pixels[y * textureSize + x] = precomputedColors[shiftedY * textureSize + shiftedX];
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
        image.sprite = Sprite.Create(gradientTexture, new Rect(0, 0, gradientTexture.width, gradientTexture.height), new Vector2(0.5f, 0.5f));
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
        // �ָ�Ϊ��ɫ����
        image.sprite = Sprite.Create(solidColorTexture, new Rect(0, 0, solidColorTexture.width, solidColorTexture.height), new Vector2(0.5f, 0.5f));
    }

    // ָ���ƶ��¼�������Ҫ������ΪUpdate�����Ѿ����������
    public void OnPointerMove(PointerEventData eventData)
    {
        // ���ﲻ��Ҫ�κζ�����Update���������ݶȸ���
    }
}
