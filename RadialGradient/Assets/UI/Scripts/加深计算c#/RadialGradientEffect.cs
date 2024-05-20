/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//��㽥�䣬��˹ģ�������������ص㡣
public class RadialGradientEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public Image image;
    public Color[] gradientColors = new Color[3] { new Color(1, 1, 1, 0.8f), new Color(1, 1, 1, 0.6f), new Color(1, 1, 1, 0.4f) };
    public float[] radii = new float[3] { 0.3f, 0.5f, 0.7f };
    public float[] sigmas = new float[3] { 0.1f, 0.15f, 0.2f };
    public int textureSize = 256;
    public float updateThreshold = 0.01f;

    private Texture2D gradientTexture;
    private RectTransform rectTransform;
    private Vector2 lastMousePosition;
    private bool isPointerOver = false;

    void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }

        rectTransform = image.GetComponent<RectTransform>();
        gradientTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        gradientTexture.wrapMode = TextureWrapMode.Clamp;
        image.sprite = Sprite.Create(gradientTexture, new Rect(0, 0, gradientTexture.width, gradientTexture.height), new Vector2(0.5f, 0.5f));
        UpdateGradient(new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        if (isPointerOver)
        {
            Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
            Vector2 normMousePosition = new Vector2(
                Mathf.InverseLerp(-rectTransform.rect.width / 2, rectTransform.rect.width / 2, localMousePosition.x),
                Mathf.InverseLerp(-rectTransform.rect.height / 2, rectTransform.rect.height / 2, localMousePosition.y)
            );

            if (Vector2.Distance(normMousePosition, lastMousePosition) > updateThreshold)
            {
                lastMousePosition = normMousePosition;
                UpdateGradient(normMousePosition);
            }
        }
    }

    void UpdateGradient(Vector2 center)
    {
        Color[] pixels = new Color[textureSize * textureSize];

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float u = (float)x / (textureSize - 1);
                float v = (float)y / (textureSize - 1);
                Vector2 uv = new Vector2(u, v);

                Color finalColor = Color.white;

                for (int i = 0; i < gradientColors.Length; i++)
                {
                    float dist = Vector2.Distance(uv, center);
                    float weight = Mathf.Exp(-dist * dist / (2 * sigmas[i] * sigmas[i])) / (2 * Mathf.PI * sigmas[i] * sigmas[i]);
                    Color layerColor = Color.Lerp(Color.white, gradientColors[i], Mathf.SmoothStep(radii[i], radii[i] + 0.1f, dist) * weight);
                    finalColor = Color.Lerp(finalColor, layerColor, weight);
                }

                pixels[y * textureSize + x] = finalColor;
            }
        }

        gradientTexture.SetPixels(pixels);
        gradientTexture.Apply();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        UpdateGradient(new Vector2(0.5f, 0.5f));
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        // 
    }
}
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// ʵ�ֶ�㽥�䣬��˹ģ�������Ż����������ص���㷨
public class RadialGradientEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
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

    // ���½�������
    void UpdateGradient(Vector2 center)
    {
        Color[] pixels = new Color[textureSize * textureSize];

        // ����ÿ�����ص�
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
                    float dist = Vector2.Distance(uv, center);
                    float weight = Mathf.Exp(-dist * dist / (2 * sigmas[i] * sigmas[i])) / (2 * Mathf.PI * sigmas[i] * sigmas[i]);
                    Color layerColor = Color.Lerp(Color.white, gradientColors[i], Mathf.SmoothStep(radii[i], radii[i] + 0.1f, dist) * weight);
                    finalColor = Color.Lerp(finalColor, layerColor, weight);
                }

                pixels[y * textureSize + x] = finalColor;
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

    public void OnPointerMove(PointerEventData eventData)
    {
        // �����߼���Update�����д���
    }
}
